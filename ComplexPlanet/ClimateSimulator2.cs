using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPlanet
{
    public class ClimateSimulator2
    {
        private enum DistanceTypes
        {
            POLES,
            EQUATOR,
            WATER
        }

        private float[,] heightmap, climate, humidity, tpoles, tequator, mountainyness;
        private int width;
        private int height;
        private float factor;

        public ClimateSimulator2(float[,] hm)
        {
            heightmap = hm;
            width = heightmap.GetLength(0);
            height = heightmap.GetLength(1);
            factor = (float)width / (float)height;
        }
       
        /************************************************************************************************/
        public void Compute()
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();
            //Ready the Climate Map
            climate = new float[width, height];
            //long startPoles = System.currentTimeMillis();
            tpoles = DistanceFrom(DistanceTypes.POLES, 25);
            sw.Stop();
            //Writing Execution Time
            sw.Stop(); Debug.WriteLine("Poles: " + sw.Elapsed);

            sw.Start();
            tequator = DistanceFrom(DistanceTypes.EQUATOR, 25);
            sw.Stop();
            //Writing Execution Time
            sw.Stop(); Debug.WriteLine("Equator: " + sw.Elapsed);

            sw.Start();
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    float distToEq = (0.5f - Math.Abs(((h / (float)height) * 2) - 1)) + 0.25f;

                    climate[w, h] = tpoles[w, h] * (1 - distToEq) + distToEq * tequator[w, h] - 1;
                }
            }
            normalize(climate);
            OverlayHeight(25, 0);
            sw.Stop(); Debug.WriteLine("Compose: " + sw.Elapsed);


            sw.Start();
            humidity = DistanceFrom(DistanceTypes.WATER, 20);
            sw.Stop(); Debug.WriteLine("Humidity: " + sw.Elapsed);

            sw.Start();
            //ready steepness
            steepSides();
            sw.Stop(); Debug.WriteLine("Steep sides: " + sw.Elapsed);

            //normalizing
            sw.Start();
            normalize(climate);
            normalize(humidity);
            normalize(mountainyness);
            sw.Stop(); Debug.WriteLine("Normalizing: " + sw.Elapsed);

        }

        private void normalize(float[,] array)
        {
            float min = float.MaxValue, max = float.MinValue;
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    float curr = array[w, h];
                    min = min < curr ? min : curr;
                    max = max > curr ? max : curr;
                }
            }
            float span = max - min;
            Parallel.For(0, width, w =>
            {
                for (int h = 0; h < height; h++)
                {
                    array[w, h] = (array[w, h] - min) / span;
                }
            });
        }

        private void steepSides()
        {
            mountainyness = new float[width, height];
            //System.out.println("starting steepness");
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    float currHeight = heightmap[w, h];

                    float[] s = new float[8];
                    s[0] = heightmap[(w + 1) % width, h] - currHeight;
                    s[1] = heightmap[w, (h + 1) % height] - currHeight;
                    s[2] = heightmap[(w - 1 + width) % width, h] - currHeight;
                    s[3] = heightmap[w, (h - 1 + height) % height] - currHeight;
                    s[4] = heightmap[(w + 1) % width, (h + 1) % height] - currHeight;
                    s[5] = heightmap[(w - 1 + width) % width, (h + 1) % height] - currHeight;
                    s[6] = heightmap[(w + 1) % width, (h - 1 + height) % height] - currHeight;
                    s[7] = heightmap[(w - 1 + width) % width, (h - 1 + height) % height] - currHeight;

                    float slider = 0.8f; //value to be determined
                    float temp = computeAbsMax(s) * 2 * slider + (1 - slider) * currHeight * currHeight * 0.1f;
                    temp -= 1;
                    temp = temp < 0 ? 0 : temp;
                    mountainyness[w, h] = (float)Math.Pow(temp, 1.4);
                }
            }

        }


        private float[,] InitDist(DistanceTypes fromWhat)
        {

            float[,] distArr = new float[width, height];
            switch (fromWhat)
            {
                case DistanceTypes.WATER:

                    for (int w = 0; w < width; w++)
                    {
                        for (int h = 0; h < height; h++)
                        {
                            float heightFactor = heightmap[w, h] - 1;

                            if (heightFactor < 0)
                            {  // sea
                                distArr[w, h] = 0;
                            }
                            else
                            {  // land
                                distArr[w, h] = float.MaxValue;
                            }
                        }
                    }
                    break;
                case DistanceTypes.POLES:
                    for (int w = 0; w < width; w++)
                    {
                        for (int h = 0; h < height; h++)
                        {

                            if (h == 0)
                            {  // topOfTheMap
                                distArr[w, h] = 0;
                            }
                            else
                            {
                                distArr[w, h] = float.MaxValue;
                            }
                        }
                    }
                    break;
                case DistanceTypes.EQUATOR:

                    for (int w = 0; w < width; w++)
                    {
                        for (int h = 0; h < height; h++)
                        {

                            if (h == height / 2)
                            {  // equator level
                                distArr[w, h] = 0;
                            }
                            else
                            {
                                distArr[w, h] = float.MaxValue;
                            }
                        }
                    }
                    break;
            }
            return distArr;
        }

        private float[,] DistanceFrom(DistanceTypes fromWhat, float heightInfluence)
        {
            float[,] distArr = InitDist(fromWhat);
            float currentDistance = 0;

            Console.WriteLine("Starting distance calculation: " + fromWhat);
            while (currentDistance < Math.Max(width, height))
            {
                for (int w = 0; w < width; w++)
                {
                    for (int h = 0; h < height; h++)
                    {
                        float currHeight = heightmap[w, h];
                        if (distArr[w, h] == float.MaxValue)
                        { //Block could update
                            if (distArr[(w + 1) % width, h] + (heightmap[(w + 1) % width, h] - currHeight) * heightInfluence - currentDistance <= 0 ||
                                distArr[w, (h + 1) % height] + (heightmap[w, (h + 1) % height] - currHeight) * heightInfluence - currentDistance <= 0 ||
                                distArr[(w - 1 + width) % width, h] + (heightmap[(w - 1 + width) % width, h] - currHeight) * heightInfluence - currentDistance <= 0 ||
                                distArr[w, (h - 1 + height) % height] + (heightmap[w, (h - 1 + height) % height] - currHeight) * heightInfluence - currentDistance <= 0)
                            {

                                //System.out.println("In case 1");
                                distArr[w, h] = currentDistance + 1;
                            }
                            else if (
                                    distArr[(w + 1) % width, (h + 1) % height] + (heightmap[(w + 1) % width, (h + 1) % height] - currHeight) * heightInfluence - (currentDistance + 0.41421) <= 0 ||
                                    distArr[(w - 1 + width) % width, (h + 1) % height] + (heightmap[(w - 1 + width) % width, (h + 1) % height] - currHeight) * heightInfluence - (currentDistance + 0.41421) <= 0 ||
                                    distArr[(w + 1) % width, (h - 1 + height) % height] + (heightmap[(w + 1) % width, (h - 1 + height) % height] - currHeight) * heightInfluence - (currentDistance + 0.41421) <= 0 ||
                                    distArr[(w - 1 + width) % width, (h - 1 + height) % height] + (heightmap[(w - 1 + width) % width, (h - 1 + height) % height] - currHeight) * heightInfluence - (currentDistance + 0.41421) <= 0)
                            {
                                distArr[w, h] = currentDistance + 1.41421f;
                                //System.out.println("In case 2");
                            }
                        }
                    }
                }
                currentDistance++;
                //System.out.println("I was here " + currentDistance + " times");
            }

            //normalize Array
            float max = 0;
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    max = distArr[w, h] > max ? distArr[w, h] : max;
                }
            }
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    distArr[w, h] /= max;
                }
            }

            //invert if necessary
            if (fromWhat == DistanceTypes.EQUATOR)
            {
                for (int w = 0; w < width; w++)
                {
                    for (int h = 0; h < height; h++)
                    {
                        distArr[w, h] = 1 - distArr[w, h];
                    }
                }
            }

            return distArr;
        }


        private void OverlayHeight(int strength, int threshold)
        {
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    float distToEq = 1 - Math.Abs(((h / (float)height) * 2) - 1);
                    float heightFactor = heightmap[w, h] - 1;

                    if (heightFactor < 0)
                    {  // sea
                        climate[w, h] = (distToEq + 0.2f) * 0.6f;
                    }
                    else
                    {                // land
                        heightFactor -= 4;
                        if (heightFactor >= 0)
                        {
                            float candidate = climate[w, h] - (strength * (heightFactor * 0.05f) * (heightmap[w, h] - 4) * 0.4f) * 0.001f;
                            climate[w, h] = candidate < 0 ? 0 : candidate;
                        }
                        //((distToEq*locationInfluence + (100-locationInfluence)*0.5f)*0.01f - heightFactor*0.05f))*0.01f;

                        //climate[height , width] = ((100-strength)*climate[height , width] + strength *((distToEq*locationInfluence + (100-locationInfluence)*0.5f)*0.01f - heightFactor*0.05f))*0.01f;
                    }
                }
            }
        }

        private float computeAbsMax(float[] arr)
        {
            float max = 0;
            foreach (float a in arr)
            {
                max = a > max ? a : max;
                max = (-a) > max ? (-a) : max;
            }
            return max;
        }

        public float[,] Climate
        {
            get { return climate; }
        }


        public float[,] Tpoles
        {
            get { return tpoles; }
        }

        public float[,] Tequator
        {
            get { return tequator; }
        }

        public float[,] Mountainyness
        {
            get { return mountainyness; }
        }

        public float[,] Humidity
        {
            get { return humidity; }
        }
    }
}
