using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;
using Brain;
using Utilities;
using System.Windows;
using System.Drawing;
using Genes;
using System.Drawing.Drawing2D;

namespace smart_sweepers
{
    internal class MineSweeper
    {
        private NeuralNet Brain = new NeuralNet();
        private Vector LookAt = new Vector(0,0);
        private double Rotation;
        private Double Speed;

        private double LTrack, RTrack;

        private double Scale;

        private Mine _ClosestMine;

        private int Width;
        private int Height;
        private bool Elite;
        
        public Pen Color;
        public Genome Genes;

        private Graphics Whiteboard;
        private System.Drawing.Drawing2D.Matrix DrawingMatrix;

        public MineSweeper(ref Graphics whiteboard, int width, int height, bool elite)
        {
            Whiteboard = whiteboard;
            Rotation = Utilities.Math.Rand360();
            LTrack = 0.16;
            RTrack = 0.16;
            Scale = Properties.Settings.Default.SweeperScale;

            var x = Utilities.Math.Rand(width);
            var y = Utilities.Math.Rand(height);
            Position = new Vector(x, y);
            Width = width;
            Height = height;
            Elite = elite;
            Color = elite ? Pens.Red : Pens.Black;
            Visual = new Rectangle((int)x, (int)y, 6, 12);
        }

        public void InsertGenes(Genome genes)
        {
            Genes = genes;
            Brain.PutWeights(genes.Weights);
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
        public bool Update(ref List<Mine> mines)
        {
            //this will store all the inputs for the NN
            var inputs = new List<double>();	

            //get vector to closest mine
            var closestMine = ClosestMine(ref mines);
  
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
            if (output.Count < Properties.Settings.Default.Outputs) 
            {
                return false;
            }

            //assign the outputs to the sweepers left & right tracks
            LTrack = output[0];
            RTrack = output[1];

            if (double.IsNaN(LTrack) || double.IsNaN(RTrack))
            {
                throw new Exception("Tracks are messed up");
            }

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
        public Vector ClosestMine(ref List<Mine> mines)
        {
            double closestSoFar = 99999;
            Vector closest = null;

	        //cycle through mines to find closest
            foreach (var mine in mines)
            {
	            double distance = (mine.Position - Position).Length();

                if (distance < closestSoFar)
		        {
                    closestSoFar = distance;
			
			        closest = Position - mine.Position;

                    _ClosestMine = mine;
		        }
	        }

	        return closest;
        }


        //  this function checks for collision with its closest mine (calculated
        //  earlier and stored in _ClosestMine)
        public Mine CheckForMine(ref List<Mine> mines, double size)
        {
            var distance = Position - _ClosestMine.Position;

            if (distance.Length() < (size + 5))
            {
                return _ClosestMine;
            }

            return null;
        }

        //	Resets the sweepers position, fitness and rotation
        public void Reset()
        {
            //reset the sweepers positions
            Position = new Vector(Utilities.Math.Rand(Width), Utilities.Math.Rand(Height));
	
            //and the fitness
            Genes.Fitness = 0;

            //and the rotation
            Rotation = Utilities.Math.Rand360();
        }

        public Vector Position;

        public void IncrementFitness()
        {
            Genes.Fitness++;
        }

        public double Fitness()
        {
            return Genes.Fitness;
        }

        public int GetNumberOfWeights()
        {
            return Brain.GetNumberOfWeights();
        }

        private Rectangle Visual;
        public void Draw()
        {
            /*System.Drawing.Drawing2D.Matrix transform = System.Drawing.Drawing2D.Matrix.CreateTranslation(new Vector3(-pivot, 0.0f)) *
            System.Drawing.Drawing2D.Matrix.CreateRotationZ(angle) *
            System.Drawing.Drawing2D.Matrix.CreateTranslation(new Vector3(position, 0.0f));*/
            DrawingMatrix = new System.Drawing.Drawing2D.Matrix();
            Visual.X = (int)Position.X;
            Visual.Y = (int)Position.Y;
            DrawingMatrix.Rotate((float)Rotation, MatrixOrder.Append);
            Whiteboard.Transform = DrawingMatrix;
            Whiteboard.DrawRectangle(Color, Visual);
        }
    }
}
