using System;
using System.Collections.Generic;
using Brain.Properties;

namespace Brain
{
    public class NeuralNet
    {
        private int Inputs = Settings.Default.Inputs;
        private int Outputs = Settings.Default.Outputs;
        private int HiddenLayers = Settings.Default.HiddenLayers;
        private int NeuronsPerHiddenLayer = Settings.Default.NeuronsPerHiddenLayer;

        private List<NeuronLayer> Layers = new List<NeuronLayer>();

        public NeuralNet()
        {
            //create the layers of the network
            if (HiddenLayers > 0)
            {
                //create first hidden layer
                Layers.Add(new NeuronLayer(NeuronsPerHiddenLayer, Inputs));

                for (int i = 0; i < HiddenLayers - 1; ++i)
                {

                    Layers.Add(new NeuronLayer(NeuronsPerHiddenLayer,
                                                 NeuronsPerHiddenLayer));
                }

                //create output layer
                Layers.Add(new NeuronLayer(Outputs, NeuronsPerHiddenLayer));
            }

            else
            {
                //create output layer
                Layers.Add(new NeuronLayer(Outputs, Inputs));
            }
        }

        public List<double> GetWeights()
        {
            //this will hold the weights
            var weights =  new List<double>();

            //for each layer
            for (int i = 0; i < HiddenLayers + 1; ++i)
            {

                //for each neuron
                for (int j = 0; j < Layers[i].Neurons.Count; ++j)
                {
                    //for each weight
                    for (int k = 0; k < Layers[i].Neurons[j].Inputs; ++k)
                    {
                        weights.Add(Layers[i].Neurons[j].Weight[k]);
                    }
                }
            }

            return weights;
        }

        public void PutWeights(List<double> weights)
        {
            int weight = 0;

            //for each layer
            for (int i = 0; i < HiddenLayers + 1; ++i)
            {

                //for each neuron
                for (int j = 0; j < Layers[i].Neurons.Count; ++j)
                {
                    //for each weight
                    for (int k = 0; k < Layers[i].Neurons[j].Inputs; ++k)
                    {
                        Layers[i].Neurons[j].Weight[k] = weights[weight++];
                    }
                }
            }
        }

        public int GetNumberOfWeights()
        {
            int weights = 0;

            //for each layer
            for (int i = 0; i < HiddenLayers + 1; ++i)
            {

                //for each neuron
                for (int j = 0; j < Layers[i].Neurons.Count; ++j)
                {
                    //for each weight
                    for (int k = 0; k < Layers[i].Neurons[j].Inputs; ++k)

                        weights++;

                }
            }

            return weights;
        }

        public List<double> Update(List<double> inputs)
        {
            //stores the resultant outputs from each layer
            var outputs = new List<double>();
            var weight = 0;
	
            //first check that we have the correct amount of inputs
            if (inputs.Count != Inputs)
            {
                //just return an empty vector if incorrect.
                return outputs;
            }
	
	        //For each layer....
	        for (int i=0; i<HiddenLayers + 1; ++i)
	        {		
		        if ( i > 0 )
                {
                    inputs = outputs;
                }

		        outputs.Clear();
		
		        weight = 0;

		        //for each neuron sum the (inputs * corresponding weights).Throw 
		        //the total at our sigmoid function to get the output.
		        for (int j=0; j<Layers[i].Neurons.Count; ++j)
		        {
			        double netinput = 0;

		            int numInputs = Layers[i].Neurons[j].Inputs;
			
			        //for each weight
			        for (int k=0; k<numInputs - 1; ++k)
			        {
				        //sum the weights x inputs
				        netinput += Layers[i].Neurons[j].Weight[k] * inputs[weight++];
			        }

			        //add in the bias
		            netinput += Layers[i].Neurons[j].Weight[numInputs - 1] * Settings.Default.Bias;

                    //we can store the outputs from each layer as we generate them. 
                    //The combined activation is first filtered through the sigmoid 
                    //function
			        outputs.Add(Sigmoid(netinput, Settings.Default.ActivationResponse));

			        weight = 0;
		        }
	        }

	        return outputs;
        }

        public double Sigmoid(double activation, double response)
        {
            return (1/(1 + Math.Exp(-activation/response)));
        }
    }
}
