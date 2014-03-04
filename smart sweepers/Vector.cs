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
        public double X, Y;

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        //overload the + operator
        public static Vector operator +(Vector lhs, Vector rhs)
        {
            lhs.X += rhs.X;
            lhs.Y += rhs.Y;
            return lhs;
        }

        //overload the - operator
        public static Vector operator -(Vector lhs, Vector rhs)
        {
            lhs.X -= rhs.X;
            lhs.Y -= rhs.Y;
            return lhs;
        }

        //overload the * operator
        public static Vector operator *(Vector lhs, Vector rhs)
        {
            lhs.X *= rhs.X;
            lhs.Y *= rhs.Y;
            return lhs;
        }

        //overload the * operator
        public static Vector operator *(Vector lhs, double rhs)
        {
            lhs.X *= rhs;
            lhs.Y *= rhs;
            return lhs;
        }

        //overload the / operator
        public static Vector operator /(Vector lhs, Vector rhs)
        {
            lhs.X /= rhs.X;
            lhs.Y /= rhs.Y;
            return lhs;
        }

        //	returns the length of a 2D vector
        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        //	normalizes a 2D Vector
        public void Normalize()
        {
            double length = Length();
            X = X/length;
            Y = Y/length;
        }

        //	calculates the dot product
        public double Dotify(Vector v1, Vector v2)
        {
            return v1.X*v2.X + v1.Y*v2.Y;
        }

        //  returns positive if v2 is clockwise of v1, minus if anticlockwise
        public int Signify(Vector v1, Vector v2)
        {
            return (v1.Y*v2.X > v2.X*v2.Y ? 1 : -1);
        }
    }
}
