using System;

namespace Utilities
{
    public static class Math
    {
        private static Random rand = new Random(DateTime.Now.Millisecond);
        //returns a random float between zero and 1
        public static double Rand()
        {
            return Rand(1);
        }

        //returns a random integer between x and y
        public static int Rand(int x, int y)
        {
            var result = rand.Next(x,y);
            return result;
        }

        //returns a random float between zero and x
        public static double Rand(int x)
        {
            var result = rand.Next(x);
            return result;
        }

        //returns a random float between zero and x
        public static double Rand(double x)
        {
            var result = rand.Next((int)x);
            return result;
        }

        public static double Rand360()
        {
            var pi2 = System.Math.PI * 2;
            var result = rand.NextDouble() * pi2;
            return result;
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
