using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Genes;
using System.Drawing;

namespace smart_sweepers
{
    public partial class Controller : Form
    {
        private List<Genome> Population = new List<Genome>();
        private List<MineSweeper> Sweepers = new List<MineSweeper>();
        List<Mine> Mines = new List<Mine>();
        private int Weights;
        
        //vertex buffer for the sweeper shape's vertices
        private List<Point> SweeperVerticesBuffer = new List<Point>();

        //vertex buffer for the mine shape's vertices
        private List<Point> MineVerticesBuffer = new List<Point>();

        //stores the average fitness per generation for use 
        //in graphing.
        private List<double> AverageFitness = new List<double>();
        
        //stores the best fitness per generation
        private List<double> BestFitness = new List<double>();

        private int Ticks = 0;
        private int Generations = 0;

        private Point[] Sweeper = new Point[16]
        {
            new Point(-1, -1),
            new Point(-1, 1),
            new Point(-0.5, 1),
            new Point(-0.5, -1),

            new Point(0.5, -1),
            new Point(1, -1),
            new Point(1, 1),
            new Point(0.5, 1),
            
            new Point(-0.5, -0.5),
            new Point(0.5, -0.5),
                                        
            new Point(-0.5, 0.5),
            new Point(-0.25, 0.5),
            new Point(-0.25, 1.75),
            new Point(0.25, 1.75),
            new Point(0.25, 0.5),
            new Point(0.5, 0.5)
        };

        Point[] Mine = new Point[4]
        {
            new Point(-1, -1),
            new Point(-1, 1),
            new Point(1, 1),
            new Point(1, -1)
        };

        private GeneticAlgorithm GA;
        private Graphics Whiteboard;

        public Controller()
        {
            FastRender = false;
            Width = ActiveForm.Width;
            Height = ActiveForm.Height;
            Whiteboard = CreateGraphics();
            Whiteboard.CompositingQuality = CompositingQuality.HighQuality;
            Whiteboard.SmoothingMode = SmoothingMode.HighQuality;

            //let's create the mine sweepers
            for (int i = 0; i < Sweeper.Length; ++i)
            {
                Sweepers.Add(new MineSweeper(ref Whiteboard, Width, Height, i == Properties.Settings.Default.Elite));
            }

            //get the total number of weights used in the sweepers
            //NN so we can initialise the GA
            Weights = Sweepers[0].GetNumberOfWeights();

            //initialize the Genetic Algorithm class
            GA = new GeneticAlgorithm(Sweepers.Count,
                Properties.Settings.Default.MutationRate,
                Properties.Settings.Default.CrossoverRate,
                Weights);
            //Get the weights from the Genetic Algorythm and insert into the sweepers brains
            Population = GA.GetChromos();

            for (var i = 0; i < Population.Count; i++)
            {
                Sweepers[i].InsertGenes(Population[i]);
            }

            //initialize mines in random positions within the application window
            for (var i = 0; i < Properties.Settings.Default.Mines; ++i)
            {
                var mine = new Mine(ref Whiteboard, Width, Height);
                Mines.Add(mine);
            }
	

            //fill the vertex buffers
            foreach (Point sweeper in Sweeper)
            {
                SweeperVerticesBuffer.Add(sweeper);
            }

            foreach (Point mine in Mine)
            {
                MineVerticesBuffer.Add(mine);
            }

            InitializeComponent();
            timer1.Enabled = true;
        }

        private void PlotStats()
        {
            string s = String.Format("Best Fitness:       {0}", GA.BestFitness) +
                Environment.NewLine +
                String.Format("Average Fitness: {0}", GA.AverageFitness());
            
        }

        public void Render()
        {
            //render the stats
            labelGeneration.Text = String.Format("Generation:          {0}", Generations);

	        //do not render if running at accelerated speed
	        if (!FastRender)
	        {
                //render the mines
                foreach (var mine in Mines)
                {
                    //grab the vertices for the mine shape
                    WorldTransform(ref MineVerticesBuffer, mine.Position);
                    //draw the mines
                    mine.Draw();
                }
       		
                //we want the fittest displayed in red
		        //render the sweepers
		        foreach (var sweeper in Sweepers)
		        {
                    //grab the sweeper vertices
                    //transform the vertex buffer
                    sweeper.WorldTransform(ref SweeperVerticesBuffer);
                    sweeper.Draw();
                }
                //Invalidate();
	        }
            else
            {
                PlotStats();
            }
        }

        public void WorldTransform(ref List<Point> verticalBuffer, Vector vPos)
        {
            //create the world transformation matrix
            var transform = new Matrix();
	
            //scale
            transform.Scale(Properties.Settings.Default.MinScale, Properties.Settings.Default.MinScale);
	
            //translate
            transform.Translate(vPos.X, vPos.Y);

            //transform the ships vertices
            transform.TransformPoints(ref verticalBuffer);
        }


        //	This is the main workhorse. The entire simulation is controlled from here.
        //
        //	The comments should explain what is going on adequately.
        public new bool Update()
        {
            //run the sweepers through CParams::iNumTicks amount of cycles. During
            //this loop each sweepers NN is constantly updated with the appropriate
            //information from its surroundings. The output from the NN is obtained
            //and the sweeper is moved. If it encounters a mine its fitness is
            //updated appropriately,
            if (Ticks++ < Properties.Settings.Default.Ticks)
            {
                foreach (var sweeper in Sweepers)
                {
                    //update the NN and position
                    if (!sweeper.Update(ref Mines))
                    {
                        //error in processing the neural net
                        MessageBox.Show("Wrong amount of NN inputs!", "Error");

                        return false;
                    }
				
                    //see if it's found a mine
                    var foundMine = sweeper.CheckForMine(ref Mines, Properties.Settings.Default.MinScale);

                    if (foundMine != null)
                    {
                        //we have discovered a mine so increase fitness
                        sweeper.IncrementFitness();

                        //mine found so replace the mine with another at a random 
                        //position
                        foundMine.Reposition();


                    }
                }
            }

            //Another generation has been completed.
  
            //Time to run the GA and update the sweepers with their new NNs
            else
            {
                //update the stats to be used in our stat window
                AverageFitness.Add(GA.AverageFitness());
                BestFitness.Add(GA.BestFitness);

                //increment the generation counter
                ++Generations;

                //reset cycles
                Ticks = 0;

                //run the GA to create a new population
                Population = GA.Epoch(Population);
			
                //insert the new (hopefully)improved brains back into the sweepers
                //and reset their positions etc
                for (int i=0; i<Sweepers.Count; ++i)
                {
                    Sweepers[i].Genes = Population[i];
                    Sweepers[i].Reset();
                }
            }

            Render();
            return true;
        }

        public bool FastRender { get; set; }

        private void ControllerLoad(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Update();
        }
    }
}
