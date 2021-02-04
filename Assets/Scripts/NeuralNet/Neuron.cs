using System.Collections.Generic;
using System;

    public class Neuron{
		public List<Synapse> InputSynapses { get; set; }
		public List<Synapse> OutputSynapses { get; set; }
		public double Value { get; set; }
		public double Bias { get; set; }
		public double BiasDelta { get; set; }
		public double Gradient { get; set; }
		public double Error { get; set; }

		public Neuron()
		{
			InputSynapses = new List<Synapse>();
			OutputSynapses = new List<Synapse>();
			Bias = 0;
            Value = 0;
			Error = 0;
		}

		public virtual double CalculateValue(){
			double weightedSum = 0;

			foreach (Synapse synapse in InputSynapses) {
				weightedSum += synapse.InputNeuron.Value * synapse.Weight;
			}

			Value = ActivationFunctions.Sigmoid(weightedSum + Bias);
			return Value;
		}

		// Calculate error for output neuron
		public double CalculateError(double target) {
			Error = target - Value;
			return Error;
		}

		// Calculate error for any hidden neuron
		public double CalculateError(){
			Error = 0;
			foreach(Synapse synapse in OutputSynapses) {
				Error += synapse.Weight * synapse.OutputNeuron.Error;
			}
			return Error;
		}
		
		// Calculate gradient
		public double CalculateGradient() {
			Gradient = ActivationFunctions.SigmoidDerivative(Value);
			return Gradient;
		}

		//Update all the weights of synapses that are inputs to this neuron
		public void UpdateWeights(double learnRate, double momentum) {
			foreach (Synapse synapse in InputSynapses) {
				double prevWeightDelta = synapse.WeightDelta;
				synapse.WeightDelta = learnRate * Error * Gradient * synapse.InputNeuron.Value;
				synapse.Weight += synapse.WeightDelta + momentum * prevWeightDelta;
			}
		}

		//Update the bias or the neuron
		public void UpdateBias(double learnRate, double momentum){
			double prevBiasDelta = BiasDelta;
			BiasDelta = learnRate * Error * Gradient;
			Bias += BiasDelta + momentum * prevBiasDelta;
		}
    }