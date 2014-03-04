using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genes
{
    public class Genome
    {
        public List<double> Weights = new List<double>();
        public double Fitness = 0;

        public Genome()
        {

        }

        public Genome(List<double> weights, double fitness)
        {
            Weights = weights;
            Fitness = fitness;
        }

        public static bool operator <(Genome argument1, Genome argument2)
        {
            return argument1.Fitness < argument2.Fitness;
        }

        public static bool operator >(Genome argument1, Genome argument2)
        {
            return argument1.Fitness > argument2.Fitness;
        }
    }
}
