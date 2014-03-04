using System;

namespace Utilities
{
    public static class Math
    {
        //returns a random integer between x and y
        public static int Rand(int x, int y)
        {
            return (new Random()).Next(x,y);
        }

        //returns a random float between zero and 1
        public static double Rand()
        {
            return new Random().Next(0,1);
        }

        public static double RandomClamped()
        {
            return Rand() - Rand();
        }

        public static bool RandBool()
        {
            if (Rand(0, 1) >= .5)
            {
                return true;
            }

            return false;
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
