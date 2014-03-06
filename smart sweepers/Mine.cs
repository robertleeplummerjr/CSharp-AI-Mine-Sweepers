using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace smart_sweepers
{
    class Mine
    {
        private Graphics Whiteboard;
        public int X;
        public int Y;
        private int widthConstraint;
        private int heightConstraint;
        private Rectangle Visual;
        public Vector Position;
        public bool Repositioned = false;
        
        public Pen Color = Pens.Green;

        public Mine(ref Graphics whiteboard, int width, int height)
        {
            Whiteboard = whiteboard;
            widthConstraint = width;
            heightConstraint = height;
            Reposition();
        }

        public void Reposition()
        {
            X = (int)Utilities.Math.Rand(widthConstraint);
            Y = (int)Utilities.Math.Rand(heightConstraint);
            Position = new Vector(X, Y);
            Repositioned = true;
        }

        public void Draw()
        {
            Visual = new Rectangle(X, Y, 5, 5);
            Whiteboard.DrawEllipse(Color, Visual);
            Repositioned = false;
        }
    }
}
