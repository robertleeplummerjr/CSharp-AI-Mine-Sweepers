using System;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using Brain;
using Utilities;
using System.Windows;
using System.Drawing;

namespace smart_sweepers
{
    internal class MineSweeper
    {
        private NeuralNet Brain = new NeuralNet();
        private Vector _Position;
        private Vector LookAt;
        private double Rotation;
        private Double Speed;

        private double LTrack, RTrack;

        private double Scale;

        private int _ClosestMine;

        private int Width;
        private int Height;

        private Graphics Whiteboard;

        public MineSweeper(Graphics whiteboard, int width, int height)
        {
            Whiteboard = whiteboard;
            Rotation = Utilities.Math.Rand()*(System.Math.PI*2);
            LTrack = 0.16;
            RTrack = 0.16;
            Fitness = 0;
            Scale = Properties.Settings.Default.SweeperScale;
            _ClosestMine = 0;


            _Position = new Vector(Utilities.Math.Rand() * width, Utilities.Math.Rand() * height);
            Width = width;
            Height = height;
        }

        //	First we take sensor readings and feed these into the sweepers brain.
        //
        //	The inputs are:
        //	
        //	A vector to the closest mine (x, y)
        //	The sweepers 'look at' vector (x, y)
        //
        //	We receive two outputs from the brain.. lTrack & rTrack.
        //	So given a force for each track we calculate the resultant rotation 
        //	and acceleration and apply to current velocity vector.
        public bool Update(List<Vector> mines)
        {
            //this will store all the inputs for the NN
            var inputs = new List<double>();	

            //get vector to closest mine
            var closestMine = ClosestMine(mines);
  
            //normalise it
            closestMine.Normalize();
  
            //add in vector to closest mine

            inputs.Add(closestMine.X);
            inputs.Add(closestMine.Y);

            //add in sweepers look at vector
            inputs.Add(LookAt.X);
            inputs.Add(LookAt.Y);

  
            //update the brain and get feedback
            List<double> output = Brain.Update(inputs);

            //make sure there were no errors in calculating the 
            //output
            if (output.Count < Properties.Settings.Default.Ouputs) 
            {
                return false;
            }

            //assign the outputs to the sweepers left & right tracks
            LTrack = output[0];
            RTrack = output[1];

            //calculate steering forces
            double rotForce = LTrack - RTrack;

            //clamp rotation
            Utilities.Math.Clamp(ref rotForce, -Properties.Settings.Default.MaxTurnRate, Properties.Settings.Default.MaxTurnRate);

            Rotation += rotForce;
	
            Speed = (LTrack + RTrack);	

            //update Look At 
            LookAt.X = -System.Math.Sin(Rotation);
            LookAt.Y = System.Math.Cos(Rotation);

            //update position
            Position += (LookAt * Speed);

            //wrap around window limits
            if (Position.X > Width) Position.X = 0;
            if (Position.X < 0) Position.X = Width;
            if (Position.Y > Height) Position.Y = 0;
            if (Position.Y < 0) Position.Y = Height;

            return true;
        }

        //	sets up a translation matrix for the sweeper according to its
        public void WorldTransform(ref List<Point> sweeper)
        {
            //create the world transformation matrix
            var transform = new Matrix();

            //scale
            transform.Scale(Scale, Scale);

            //rotate
            transform.Rotate(Rotation);

            //and translate
            transform.Translate(Position.X, Position.Y);

            //now transform the ships vertices
            transform.TransformPoints(ref sweeper);
        }

        //	returns the vector from the sweeper to the closest mine
        public Vector ClosestMine(List<Vector> mines)
        {
            double closestSoFar = 99999;
            var closest = new Vector(0,0);

	        //cycle through mines to find closest
	        for (int i=0; i<mines.Count; i++)
	        {
	            double distance = (mines[i] - Position).Length();

                if (distance < closestSoFar)
		        {
                    closestSoFar = distance;
			
			        closest = Position - mines[i];

                    _ClosestMine = i;
		        }
	        }

	        return closest;
        }


        //  this function checks for collision with its closest mine (calculated
        //  earlier and stored in _ClosestMine)
        public int CheckForMine(List<Vector> mines, double size)
        {
            var distance = Position - mines[_ClosestMine];

            if (distance.Length() < (size + 5))
            {
                return _ClosestMine;
            }

            return -1;
        }

        //	Resets the sweepers position, fitness and rotation
        public void Reset(int width, int height)
        {
            //reset the sweepers positions
            Position = new Vector(Utilities.Math.Rand() * width, Utilities.Math.Rand() * height);
	
            //and the fitness
            Fitness = 0;

            //and the rotation
            Rotation = Utilities.Math.Rand() * (System.Math.PI * 2);

            Width = width;
            Height = height;
        }

        public Vector Position { get; set; }

        public void IncrementFitness()
        {
            Fitness++;
        }

        public double Fitness { get; set; }

        public void PutWeights(List<double> w)
        {
            Brain.PutWeights(w);
        }

        public int GetNumberOfWeights()
        {
            return Brain.GetNumberOfWeights();
        }

        private Rectangle Visual;
        public void Draw(Pen pen)
        {
            Visual = new Rectangle((int)_Position.X, (int)_Position.Y, 6, 12);
            using (var matrix = new System.Drawing.Drawing2D.Matrix())
            {
                matrix.RotateAt((float)Rotation, new PointF(Visual.Left + (Visual.Width / 2), Visual.Top + (Visual.Height / 2)));
                Whiteboard.Transform = matrix;
                Whiteboard.DrawRectangle(pen, Visual);
                Whiteboard.ResetTransform();
            }
        }

        internal void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
