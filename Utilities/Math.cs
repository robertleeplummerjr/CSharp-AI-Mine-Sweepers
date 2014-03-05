using System;

namespace Utilities
{
    public static class Math
    {
        //returns a random integer between x and y
        public static int Rand(int x, int y)
        {
            var result = (new Random()).Next(x,y);
            return result;
        }

        //returns a random float between zero and 1
        public static double Rand()
        {
            var rand = (new Random()).NextDouble() * (1 - 0) + 0;
            return rand;
        }

        public static double RandomClamped()
        {
            var result = Rand() - Rand();
            return result;
        }

        public static bool RandBool()
        {
            bool result;
            if (Rand(0, 1) >= .5)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public static void Clamp(ref double arg, double min, double max)
        {
	        if (arg < min)
	        {
		        arg = min;
	        }

	        if (arg > max)
	        {
		        arg = max;
	        }
        }
    }
}
