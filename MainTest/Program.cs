// 
// Copyright (c) 2013 Agustin Santos
// 
// Permission is hereby granted, free of charge, to any person obtaining a 
// copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//


using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
using System;

namespace MainTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Tutorial1();
            //Tutorial2();
            //Tutorial3();
            Tutorial4();
            //Tutorial5();
            Console.ReadLine();
        }

        static void Tutorial1()
        {
            Console.WriteLine("Begin Tutorial 1");
            Perlin myModule = new Perlin();
            double value = myModule.GetValue(1.25, 0.75, 0.5);
            Console.WriteLine("Perlin value = " + value);
            Console.WriteLine("End Tutorial 1");
        }

        static void Tutorial2()
        {
            Console.WriteLine("Begin Tutorial 2");
            Perlin myModule = new Perlin();
            NoiseMap heightMap = new NoiseMap();
            NoiseMapBuilderPlane heightMapBuilder = new NoiseMapBuilderPlane();
            heightMapBuilder.SetSourceModule(myModule);
            heightMapBuilder.SetDestNoiseMap(heightMap);
            heightMapBuilder.SetDestSize(256, 256);
            heightMapBuilder.SetBounds(2.0, 6.0, 1.0, 5.0);
            heightMapBuilder.Build();

            RendererImage renderer = new RendererImage();
            NoiseImage image = new NoiseImage();
            renderer.SetSourceNoiseMap(heightMap);
            renderer.SetDestImage(image);
            renderer.Render();

            WriterBMP writer = new WriterBMP();
            writer.SetSourceImage(image);
            writer.SetDestFilename("tutorial2-1.bmp");
            writer.WriteDestFile();

            renderer.ClearGradient();
            renderer.AddGradientPoint(-1.0000, new Color(0, 0, 128, 255)); // deeps
            renderer.AddGradientPoint(-0.2500, new Color(0, 0, 255, 255)); // shallow
            renderer.AddGradientPoint(0.0000, new Color(0, 128, 255, 255)); // shore
            renderer.AddGradientPoint(0.0625, new Color(240, 240, 64, 255)); // sand
            renderer.AddGradientPoint(0.1250, new Color(32, 160, 0, 255)); // grass
            renderer.AddGradientPoint(0.3750, new Color(224, 224, 0, 255)); // dirt
            renderer.AddGradientPoint(0.7500, new Color(128, 128, 128, 255)); // rock
            renderer.AddGradientPoint(1.0000, new Color(255, 255, 255, 255)); // snow
            renderer.Render();
            writer.SetDestFilename("tutorial2-2.bmp");
            writer.WriteDestFile();

            renderer.EnableLight();
            renderer.Render();
            writer.SetDestFilename("tutorial2-3.bmp");
            writer.WriteDestFile();

            renderer.SetLightContrast(3.0); // Triple the contrast
            renderer.Render();
            writer.SetDestFilename("tutorial2-4.bmp");
            writer.WriteDestFile();

            renderer.SetLightBrightness(2.0); // Double the brightness
            renderer.Render();
            writer.SetDestFilename("tutorial2-5.bmp");
            writer.WriteDestFile();

            heightMapBuilder.SetDestSize(256 * 2, 256);
            heightMapBuilder.SetBounds(2.0, 10.0, 1.0, 5.0);
            heightMapBuilder.Build();
            renderer.Render();
            writer.SetDestFilename("tutorial2-6.bmp");
            writer.WriteDestFile();

            Console.WriteLine("End Tutorial 2");
        }

        static void Tutorial3()
        {
            Console.WriteLine("Begin Tutorial 3");
            Perlin myModule = new Perlin();
            NoiseMap heightMap = new NoiseMap();
            NoiseMapBuilderPlane heightMapBuilder = new NoiseMapBuilderPlane();
            heightMapBuilder.SetSourceModule(myModule);
            heightMapBuilder.SetDestNoiseMap(heightMap);
            heightMapBuilder.SetDestSize(256, 256);
            heightMapBuilder.SetBounds(6.0, 10.0, 1.0, 5.0);

            RendererImage renderer = new RendererImage();
            NoiseImage image = new NoiseImage();
            renderer.SetSourceNoiseMap(heightMap);
            renderer.SetDestImage(image);

            WriterBMP writer = new WriterBMP();
            writer.SetSourceImage(image);

            renderer.ClearGradient();
            renderer.AddGradientPoint(-1.0000, new Color(0, 0, 128, 255)); // deeps
            renderer.AddGradientPoint(-0.2500, new Color(0, 0, 255, 255)); // shallow
            renderer.AddGradientPoint(0.0000, new Color(0, 128, 255, 255)); // shore
            renderer.AddGradientPoint(0.0625, new Color(240, 240, 64, 255)); // sand
            renderer.AddGradientPoint(0.1250, new Color(32, 160, 0, 255)); // grass
            renderer.AddGradientPoint(0.3750, new Color(224, 224, 0, 255)); // dirt
            renderer.AddGradientPoint(0.7500, new Color(128, 128, 128, 255)); // rock
            renderer.AddGradientPoint(1.0000, new Color(255, 255, 255, 255)); // snow
            renderer.EnableLight();
            renderer.SetLightContrast(3.0); // Triple the contrast
            renderer.SetLightBrightness(2.0); // Double the brightness
            for (int octave = 1; octave <= 6; octave++)
            {
                for (int frecuency = 1; frecuency <= 8; frecuency = frecuency * 2)
                {
                    for (int persistence = 1; persistence <= 3; persistence++)
                    {
                        myModule.OctaveCount = octave;
                        myModule.Frequency = frecuency;
                        myModule.Persistence = persistence / 4.0f;
                        heightMapBuilder.Build();
                        renderer.Render();
                        writer.SetDestFilename("tutorial3-" + octave + "-" + frecuency + "-" + persistence + ".bmp");
                        writer.WriteDestFile();
                    }
                }
            }

            Console.WriteLine("End Tutorial 3");
        }

        static void Tutorial4()
        {
            Console.WriteLine("Begin Tutorial 4");

            RiggedMultifractal mountainTerrain = new RiggedMultifractal();
            Billow baseFlatTerrain = new Billow();
            baseFlatTerrain.Frequency = 2.0;

            ScaleBias flatTerrain = new ScaleBias();
            flatTerrain[0] = baseFlatTerrain;
            flatTerrain.Scale = 0.125;
            flatTerrain.Bias = -0.75;

            Perlin terrainType = new Perlin();
            terrainType.Frequency = 0.5;
            terrainType.Persistence = 0.25;

            Select finalTerrain = new Select();
            finalTerrain[0] = flatTerrain;
            finalTerrain[1] = mountainTerrain;
            finalTerrain.Controller = terrainType;
            finalTerrain.SetBounds(0.0, 1000.0);
            finalTerrain.FallOff = 0.125;

            NoiseMap heightMap = new NoiseMap();
            NoiseMapBuilderPlane heightMapBuilder = new NoiseMapBuilderPlane(); ;
            heightMapBuilder.SetSourceModule(finalTerrain);
            heightMapBuilder.SetDestNoiseMap(heightMap);
            heightMapBuilder.SetDestSize(256, 256);
            heightMapBuilder.SetBounds(6.0, 10.0, 1.0, 5.0);
            heightMapBuilder.Build();

            RendererImage renderer = new RendererImage();
            NoiseImage image = new NoiseImage();
            renderer.SetSourceNoiseMap(heightMap);
            renderer.SetDestImage(image);
            renderer.ClearGradient();
            renderer.AddGradientPoint(-1.00, new Color(32, 160, 0, 255)); // grass
            renderer.AddGradientPoint(-0.25, new Color(224, 224, 0, 255)); // dirt
            renderer.AddGradientPoint(0.25, new Color(128, 128, 128, 255)); // rock
            renderer.AddGradientPoint(1.00, new Color(255, 255, 255, 255)); // snow
            renderer.EnableLight();
            renderer.SetLightContrast(3.0);
            renderer.SetLightBrightness(2.0);
            renderer.Render();

            WriterBMP writer = new WriterBMP();
            writer.SetSourceImage(image);
            writer.SetDestFilename("tutorial4.bmp");
            writer.WriteDestFile();

            Console.WriteLine("End Tutorial 4");
        }

        static void Tutorial5()
        {
            Console.WriteLine("Begin Tutorial 5");

            RiggedMultifractal mountainTerrain = new RiggedMultifractal();
            Billow baseFlatTerrain = new Billow();
            baseFlatTerrain.Frequency = 2.0;

            ScaleBias flatTerrain = new ScaleBias();
            flatTerrain[0] = baseFlatTerrain;
            flatTerrain.Scale = 0.125;
            flatTerrain.Bias = -0.75;

            Perlin terrainType = new Perlin();
            terrainType.Frequency = 0.5;
            terrainType.Persistence = 0.25;

            Select terrainSelector = new Select();
            terrainSelector[0] = flatTerrain;
            terrainSelector[1] = mountainTerrain;
            terrainSelector.Controller = terrainType;
            terrainSelector.SetBounds(0.0, 1000.0);
            terrainSelector.FallOff = 0.125;

            Turbulence finalTerrain = new Turbulence();
            finalTerrain[0] = terrainSelector;
            finalTerrain.Frequency = 4.0;
            finalTerrain.Power = 0.125;

            NoiseMap heightMap = new NoiseMap();
            NoiseMapBuilderPlane heightMapBuilder = new NoiseMapBuilderPlane(); ;
            heightMapBuilder.SetSourceModule(finalTerrain);
            heightMapBuilder.SetDestNoiseMap(heightMap);
            heightMapBuilder.SetDestSize(256, 256);
            heightMapBuilder.SetBounds(6.0, 10.0, 1.0, 5.0);
            heightMapBuilder.Build();

            RendererImage renderer = new RendererImage();
            NoiseImage image = new NoiseImage();
            renderer.SetSourceNoiseMap(heightMap);
            renderer.SetDestImage(image);
            renderer.ClearGradient();
            renderer.AddGradientPoint(-1.00, new Color(32, 160, 0, 255)); // grass
            renderer.AddGradientPoint(-0.25, new Color(224, 224, 0, 255)); // dirt
            renderer.AddGradientPoint(0.25, new Color(128, 128, 128, 255)); // rock
            renderer.AddGradientPoint(1.00, new Color(255, 255, 255, 255)); // snow
            renderer.EnableLight();
            renderer.SetLightContrast(3.0);
            renderer.SetLightBrightness(2.0);
            renderer.Render();

            WriterBMP writer = new WriterBMP();
            writer.SetSourceImage(image);
            writer.SetDestFilename("tutorial5.bmp");
            writer.WriteDestFile();

            Console.WriteLine("End Tutorial 5");
        }
    }
}
