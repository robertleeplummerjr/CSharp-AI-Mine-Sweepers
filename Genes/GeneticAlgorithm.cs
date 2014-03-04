using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace Genes
{
    public class GeneticAlgorithm
    {
        //this holds the entire population of chromosomes
        private List<Genome> Population = new List<Genome>();

        //amount of weights per chromo
        private int ChromoLength = 0;

        //total fitness of population
        private double TotalFitness = 0;

        //best fitness this population
        public double BestFitness = 0;

	    //worst
        double WorstFitness = 99999999;

	    //keeps track of the best genome
	    int		FittestGenome;

	    //probability that a chromosones bits will mutate.
	    //Try figures around 0.05 to 0.3 ish
	    double MutationRate;

	    //probability of chromosones crossing over bits
	    //0.7 is pretty good
	    double CrossoverRate;

	    //generation counter
	    int Generation;

        public GeneticAlgorithm(
            int populationSize,
            double mutationRate,
            double crossoverRate,
            int chromoLength)
        {
            MutationRate = mutationRate;
            CrossoverRate = crossoverRate;
            ChromoLength = chromoLength;

            //initialise population with chromosomes consisting of random
            //weights and all fitnesses set to zero
            for (int i = 0; i < populationSize; ++i)
            {
                Population.Add(new Genome());

                for (int j = 0; j < ChromoLength; ++j)
                {
                    Population[i].Weights.Add(Math.RandomClamped());
                }
            }
        }

        private void Crossover(
            List<double> mum,
            List<double> dad,
            ref List<double> baby1,
            ref List<double> baby2)
        {
            //just return parents as offspring dependent on the rate
            //or if parents are the same
            if ((Math.Rand() > CrossoverRate) || (mum == dad))
            {
                baby1 = mum;
                baby2 = dad;
            }
            else
            {

                //determine a crossover point
                int cp = Math.Rand(0, ChromoLength - 1);

                //create the offspring
                for (int i = 0; i < cp; ++i)
                {
                    baby1.Add(mum[i]);
                    baby2.Add(dad[i]);
                }

                for (var i = cp; i < mum.Count; ++i)
                {
                    baby1.Add(dad[i]);
                    baby2.Add(mum[i]);
                }
            }
        }

        //	mutates a chromosome by perturbing its weights by an amount not 
        //	greater than CParams::dMaxPerturbation
        private void Mutate(List<double> chromo)
        {
            //traverse the chromosome and mutate each weight dependent
	        //on the mutation rate
	        for (int i=0; i<chromo.Count; ++i)
	        {
		        //do we perturb this weight?
		        if (Math.Rand() < MutationRate)
		        {
			        //add or subtract a small value to the weight
			        chromo[i] += (Math.RandomClamped() * Properties.Settings.Default.MaxPerturbation);
		        }
	        }
        }

        private Genome GetChromoRoulette()
        {
            //generate a random number between 0 & total fitness count
            var slice = Math.Rand() * TotalFitness;

            //this will be set to the chosen chromosome
            Genome theChosenOne = null;

            //go through the chromosones adding up the fitness so far
            double fitnessSoFar = 0;

            for (int i = 0; i < Population.Count; ++i)
            {
                fitnessSoFar += Population[i].Fitness;

                //if the fitness so far > random number return the chromo at 
                //this point
                if (fitnessSoFar >= slice)
                {
                    theChosenOne = Population[i];

                    break;
                }

            }

            return theChosenOne;
        }

        //	This works like an advanced form of elitism by inserting NumCopies
        //  copies of the NBest most fittest genomes into a population vector
        //use to introduce elitism
        private void GrabNBest(
            int best,
            int copies,
            ref List<Genome> population)
        {
            //add the required amount of copies of the n most fittest 
            //to the supplied vector
            while (best-- > 0)
            {
                for (int i = 0; i < copies; ++i)
                {
                    population.Add(Population[(Population.Count - 1) - best]);
                }
            }
        }


        private void CalculateBestWorstAvTot()
        {
            TotalFitness = 0;

            double highestSoFar = 0;
            double lowestSoFar = 9999999;

            for (int i = 0; i < Population.Count; ++i)
            {
                //update fittest if necessary
                if (Population[i].Fitness > highestSoFar)
                {
                    highestSoFar = Population[i].Fitness;

                    FittestGenome = i;

                    BestFitness = highestSoFar;
                }

                //update worst if necessary
                if (Population[i].Fitness < lowestSoFar)
                {
                    lowestSoFar = Population[i].Fitness;

                    WorstFitness = lowestSoFar;
                }

                TotalFitness += Population[i].Fitness;


            }
        }

        private void Reset()
        {
            TotalFitness = 0;
            BestFitness = 0;
            WorstFitness = 9999999;
        }

        //	takes a population of chromosones and runs the algorithm through one
        //	 cycle.
        //	Returns a new population of chromosones.
        //this runs the GA for one generation.
        public List<Genome> Epoch(List<Genome> old_pop)
        {
            //assign the given population to the classes population
            Population = old_pop;

            //reset the appropriate variables
            Reset();

            //sort the population (for scaling and elitism)
            //Math.sort(Population.First(), Population.Last());

            //calculate best, worst, average and total fitness
            CalculateBestWorstAvTot();
  
            //create a temporary vector to store new chromosones
            var newPop = new List<Genome>();

            //Now to add a little elitism we shall add in some copies of the
            //fittest genomes. Make sure we add an EVEN number or the roulette
            //wheel sampling will crash
            if (!((Properties.Settings.Default.CopiesElite * Properties.Settings.Default.Elite % 2) > 0))
            {
                GrabNBest(Properties.Settings.Default.Elite, Properties.Settings.Default.CopiesElite, ref newPop);
            }
	

            //now we enter the GA loop
	
            //repeat until a new population is generated
            while (newPop.Count < Population.Count)
            {
                //grab two chromosones
                var mum = GetChromoRoulette();
                var dad = GetChromoRoulette();

                //create some offspring via crossover
                List<double> baby1 = null, baby2 = null;

                Crossover(mum.Weights, dad.Weights, ref baby1, ref baby2);

                //now we mutate
                Mutate(baby1);
                Mutate(baby2);

                //now copy into vecNewPop population
                newPop.Add(new Genome(baby1, 0));
                newPop.Add(new Genome(baby2, 0));
            }

            //finished so assign new pop back into m_vecPop
            Population = newPop;

            return Population;
        }


        //-------------------accessor methods
        public List<Genome> GetChromos()
        {
            return Population;
        }

        public double AverageFitness()
        {
            return TotalFitness / Population.Count;
        }
    }
}
