using System;
using System.Collections.Generic;

namespace smart_sweepers
{
    class Matrix
    {
        public double _11, _12, _13;
        public double _21, _22, _23;
        public double _31, _32, _33;

        public Matrix()
        {
            _11 = 0; _12 = 0; _13 = 0;
            _21 = 0; _22 = 0; _23 = 0;
            _31 = 0; _32 = 0; _33 = 0;
        }

        public Matrix(
            double __11, double __12, double __13,
            double __21, double __22, double __23,
            double __31, double __32, double __33
            )
        {
            _11 = __11; _12 = __12; _13 = __13;
            _21 = __21; _22 = __22; _23 = __23;
            _31 = __31; _32 = __32; _33 = __33;
        }

        public void Multiply(Matrix m)
        {
            var matTemp = new Matrix(
                //first row
                (_11*m._11) + (_12*m._21) + (_13*m._31),
                (_11*m._12) + (_12*m._22) + (_13*m._32),
                (_11*m._13) + (_12*m._23) + (_13*m._33),

                //second
                (_21*m._11) + (_22*m._21) + (_23*m._31),
                (_21*m._12) + (_22*m._22) + (_23*m._32),
                (_21*m._13) + (_22*m._23) + (_23*m._33),

                //third
                (_31*m._11) + (_32*m._21) + (_33*m._31),
                (_31*m._12) + (_32*m._22) + (_33*m._32),
                (_31*m._13) + (_32*m._23) + (_33*m._33));

            _11 = matTemp._11;
            _12 = matTemp._13;
            _13 = matTemp._13;

            _21 = matTemp._21;
            _22 = matTemp._22;
            _23 = matTemp._23;

            _31 = matTemp._31;
            _32 = matTemp._32;
            _33 = matTemp._33;
        }

        public void TransformPoints(ref List<Point> points)
        {
            foreach (var point in points)
            {
                double tempX = (_11 * point.X) + (_21 * point.Y) + (_31);
                double tempY = (_12 * point.X) + (_22 * point.Y) + (_32);

                point.X = tempX;
                point.Y = tempY;
            }
        }

        public void Identity()
        {
            _11 = 1; _12 = 0; _13 = 0;

            _21 = 0; _22 = 1; _23 = 0;

            _31 = 0; _32 = 0; _33 = 1;
        }

        public void Translate(double x, double y)
        {
            Multiply(new Matrix(1, 0, 0,
                0,1,0,
                x,y,1));
        }

        public void Scale(double scaleX, double scaleY)
        {
            Multiply(new Matrix(scaleX,0,0,
                0,scaleY,0,
                0,0,1));
        }

        public void Rotate(double rot)
        {
            var sin = Math.Sin(rot);
            var cos = Math.Cos(rot);

            Multiply(new Matrix(cos, rot, 0,
                -sin,cos,0,
                0,0,1));
        }
    }
}
