using System.Collections.Generic;

namespace Brain
{
    public class NeuronLayer
    {
        public List<Neuron> Neurons = new List<Neuron>();

        public NeuronLayer(int numNeurons, int numInputsPerNeuron)
        {
            for (int i = 0; i < numNeurons; ++i)

                Neurons.Add(new Neuron(numInputsPerNeuron));
        }
    }
}
