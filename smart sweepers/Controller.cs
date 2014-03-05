using System;
using System.Collections.Generic;
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
        List<Vector> Mines = new List<Vector>();
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
            Whiteboard = CreateGraphics();

            //let's create the mine sweepers
            for (int i = 0; i < Sweeper.Length; ++i)
            {
                Sweepers.Add(new MineSweeper(Whiteboard, 400, 400));
            }

            //get the total number of weights used in the sweepers
            //NN so we can initialise the GA
            Weights = Sweepers[0].GetNumberOfWeights();

            //initialize the Genetic Algorithm class
            GA = new GeneticAlgorithm(Sweepers.Count,
                Properties.Settings.Default.MutationRate,
                Properties.Settings.Default.CrossoverRate,
                Weights);

            //Get the weights from the GA and insert into the sweepers brains
            Population = GA.GetChromos();

            for (var i = 0; i < Sweepers.Count; i++)
            {
                Sweepers[i].PutWeights(Population[i].Weights);
            }

            //initialize mines in random positions within the application window
            for (var i=0; i<Properties.Settings.Default.Mines; ++i)
            {
                Mines.Add(new Vector(Utilities.Math.Rand() * Width, Utilities.Math.Rand() * Height));
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
	            Whiteboard.Clear(TransparencyKey);
        
                //render the mines
		        for (int i=0; i<Mines.Count; ++i)
		        {
			        //grab the vertices for the mine shape
			        WorldTransform(ref MineVerticesBuffer, Mines[i]);
			        //draw the mines
		            var mine = new Mine(Whiteboard, (int)Mine[0].X, (int)Mine[0].Y);
                    mine.Draw(Pens.Green);
		        }
       		
                //we want the fittest displayed in red
		        //render the sweepers
		        for (var i=0; i<Sweepers.Count; i++)
		        {
                    //grab the sweeper vertices
                    //transform the vertex buffer
                    Sweepers[i].WorldTransform(ref SweeperVerticesBuffer);

		            if (i == Properties.Settings.Default.Elite)
		                Sweepers[i].Draw(Pens.Red);
		            else
		                Sweepers[i].Draw(Pens.Black);
                }

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
        public bool Update()
        {
            //run the sweepers through CParams::iNumTicks amount of cycles. During
            //this loop each sweepers NN is constantly updated with the appropriate
            //information from its surroundings. The output from the NN is obtained
            //and the sweeper is moved. If it encounters a mine its fitness is
            //updated appropriately,
            if (Ticks++ < Properties.Settings.Default.Ticks)
            {
                for (int i=0; i<Sweepers.Count; ++i)
                {
                    //update the NN and position
                    if (!Sweepers[i].Update(Mines))
                    {
                        //error in processing the neural net
                        MessageBox.Show("Wrong amount of NN inputs!", "Error");

                        return false;
                    }
				
                    //see if it's found a mine
                    int GrabHit = Sweepers[i].CheckForMine(Mines, Properties.Settings.Default.MinScale);

                    if (GrabHit >= 0)
                    {
                        //we have discovered a mine so increase fitness
                        Sweepers[i].IncrementFitness();

                        //mine found so replace the mine with another at a random 
                        //position
                        Mines[GrabHit] = new Vector(Utilities.Math.Rand() * Width,
                        Utilities.Math.Rand() * Height);
                    }

                    //update the chromos fitness score
                    Population[i].Fitness = Sweepers[i].Fitness;

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
                    Sweepers[i].PutWeights(Population[i].Weights);
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
