using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using static System.Console;

    public class Program{

        static List<DataSet> trainingSets = new List<DataSet>();
        static List<DataSet> testSets = new List<DataSet>();
        static  int numTrainingRecords = 50000;
        static int numTestRecords = 1000;

        static double learningRate = 0.0085;
        static double momentum = 0.005;

        static int inputLayerSize = 64; // 8 * 8 board
        static int[] hiddenLayersSizes = {98, 80};
        static int outputLayerSize = 10;

        static NeuralNetwork neuralNetwork;

        public static void Main(){
          //  LoadData(MnistReader.ReadTrainingData(), trainingSets, numTrainingRecords);
           // LoadData(MnistReader.ReadTestData(), testSets, numTestRecords);

            //neuralNetwork = new NeuralNetwork(28 * 28, new int[]{ 128, 128 }, 10, 0.2, 0.9); //75%
            //neuralNetwork = new NeuralNetwork(28 * 28, new int[]{ 392, 196 }, 10, 0.2, 0.9); //69%
            //neuralNetwork = new NeuralNetwork(28 * 28, new int[]{ 392, 98 }, 10, 0.2, 0.9); //75%
            neuralNetwork = new NeuralNetwork(inputLayerSize, hiddenLayersSizes, outputLayerSize, learningRate, momentum);

            TrainNeuralNetwork();
            TestNeuralNetwork();
        }

       /* static void LoadData(IEnumerable iterator, List<DataSet> dataSet, int numberOfRecords) {
            int count = 0;

            foreach (ImageData image in iterator) {
                double[] inputs = new double[image.Data.GetLength(0) * image.Data.GetLength(1)];
                double[] outputs = new double[10];

                for (int r = 0; r < image.Data.GetLength(1); r++) {
                    for (int c = 0; c < image.Data.GetLength(0); c++) {
                        inputs[r*image.Data.GetLength(1) + c] = image.Data[r, c] / 255.0;
                    }
                }

                outputs[image.Label] = 1.0;
                dataSet.Add(new DataSet(inputs, outputs));

                count++;

                if (count >= numberOfRecords)
                    break;
            }
        }*/

        static void TestNeuralNetwork() {
            double numberCorrect = 0;
            foreach (DataSet dataSet in testSets) {
                double[] results = neuralNetwork.Compute(dataSet.Values);

                int resultNum = results.ToList().IndexOf(results.Max());
                int targetNum = dataSet.Targets.ToList().IndexOf(dataSet.Targets.Max());

                string s = "result: ";
                foreach (double result in results) {
                    s += string.Format("{0:0.00000}", result) + " ";
                }
                s += " (" + resultNum + ")";
                s += "\ntarget: ";
                foreach (double target in dataSet.Targets) {
                    s += string.Format("{0:0.00000}", target) + " ";
                }
                s += " (" + targetNum + ")\n";

                if(targetNum == resultNum)
                    numberCorrect += 1.0;

                WriteLine(s);
            }
            WriteLine("Total correct: " + numberCorrect + "/" + testSets.Count);

            double percentageCorrect = numberCorrect/testSets.Count * 100.0;
            WriteLine("Percentage correct: " + percentageCorrect.ToString("F2") + "%");
        }

        static void TrainNeuralNetwork() {
            WriteLine("Training Started");
            List<double> errors = new List<double>();
            int count = 0;

            foreach (DataSet dataSet in trainingSets) {
                neuralNetwork.Train(dataSet, errors);
                count++;
                WriteLine(count);
            }

            WriteLine("Training Completed");
        }
    }