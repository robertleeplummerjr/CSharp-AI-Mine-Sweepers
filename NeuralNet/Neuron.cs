using System.Collections.Generic;
using Utilities;

namespace Brain
{
    public class Neuron
    {
        public int Inputs;
        public List<double> Weight = new List<double>();

        public Neuron(int inputs)
        {
            //we need an additional weight for the bias hence the +1
            for (int i = 0; i < inputs + 1; ++i)
            {
                //set up the weights with an initial random value
                Weight.Add(Math.RandomClamped());
            }
        }
    }
}
