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
        private int X;
        private int Y;
        private Rectangle Visual;

        public Mine(Graphics whiteboard, int x, int y)
        {
            Whiteboard = whiteboard;
            X = x;
            Y = y;
        }

        public void Draw(Pen pen)
        {
            Visual = new Rectangle(X, Y, 5, 5);
            Whiteboard.DrawEllipse(pen, Visual);
        }
    }
}
