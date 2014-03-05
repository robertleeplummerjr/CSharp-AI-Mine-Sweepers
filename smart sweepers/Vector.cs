using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace smart_sweepers
{
    public class Vector
    {
        public double X = 0, Y = 0;

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        //overload the + operator
        public static Vector operator +(Vector lhs, Vector rhs)
        {
            var x = lhs.X + rhs.X;
            var y = lhs.Y + rhs.Y;
            return new Vector(x, y);
        }

        //overload the - operator
        public static Vector operator -(Vector lhs, Vector rhs)
        {
            var x = lhs.X - rhs.X;
            var y = lhs.Y - rhs.Y;
            return new Vector(x, y);
        }

        //overload the * operator
        public static Vector operator *(Vector lhs, Vector rhs)
        {
            var x = lhs.X * rhs.X;
            var y = lhs.Y * rhs.Y;
            return new Vector(x, y);
        }

        //overload the * operator
        public static Vector operator *(Vector lhs, double rhs)
        {
            var x = lhs.X * rhs;
            var y = lhs.Y * rhs;
            return new Vector(x, y);
        }

        //overload the / operator
        public static Vector operator /(Vector lhs, Vector rhs)
        {
            var x = lhs.X / rhs.X;
            var y = lhs.Y / rhs.Y;
            return new Vector(x, y);
        }

        //	returns the length of a 2D vector
        public double Length()
        {
            var root = Math.Sqrt(X * X + Y * Y);
            if (Double.IsNaN(root))
            {
                throw new Exception("Can't root");
            }
            return root;
        }

        //	normalizes a 2D Vector
        public void Normalize()
        {
            double length = Length();
            var x = X/length;
            var y = Y/length;

            if (double.IsNaN(x) || double.IsNaN(y))
            {
                X = 0;
                Y = 0;
            }
            else
            {
                X = x;
                Y = y;
            }
        }

        //	calculates the dot product
        public double Dotify(Vector v1, Vector v2)
        {
            var result = v1.X*v2.X + v1.Y*v2.Y;
            return result;
        }

        //  returns positive if v2 is clockwise of v1, minus if anticlockwise
        public int Signify(Vector v1, Vector v2)
        {
            var result = (v1.Y*v2.X > v2.X*v2.Y ? 1 : -1);
            return result;
        }
    }
}
