using System;

    public static class ActivationFunctions{
        public static double Sigmoid(double value) {
            return 1.0 / (1.0 + Math.Exp(-value));
        }

        public static double SigmoidDerivative(double x)
		{
			return x * (1 - x);
		}

        public static double ReLU(double value){
            return Math.Max(0, value);
        }

        public static double ReLUDerivative(double value){
            return value > 0 ? 1 : 0;
        }

        public static double LeakyReLU(double value, double alpha = 0.01){
            return Math.Max(alpha * value, value);
        }

        public static double LeakyReLUDerivative(double value, double alpha = 0.01){
            return value > 0 ? 1 : alpha;
        }
    }