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


namespace LibNoise
{
    public class MathHelper
    {
        /// Clamps a value onto a clamping range.
        ///
        /// @param value The value to clamp.
        /// @param lowerBound The lower bound of the clamping range.
        /// @param upperBound The upper bound of the clamping range.
        ///
        /// @returns
        /// - @a value if @a value lies between @a lowerBound and @a upperBound.
        /// - @a lowerBound if @a value is less than @a lowerBound.
        /// - @a upperBound if @a value is greater than @a upperBound.
        ///
        /// This function does not modify any parameters.
        public static int Clamp(int value, int lowerBound, int upperBound)
        {
            if (value < lowerBound)
            {
                return lowerBound;
            }
            else if (value > upperBound)
            {
                return upperBound;
            }
            else
            {
                return value;
            }
        }


        /// Swaps two values.
        ///
        /// @param a A variable containing the first value.
        /// @param b A variable containing the second value.
        ///
        /// @post The values within the the two variables are swapped.
        public static void SwapValues<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }
    }
}
