using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Threading;
using System.Globalization;

public class NNTrainer : MonoBehaviour
{
    public bool train;
    public string trainPath;
    public string testPath;

    // Start is called before the first frame update
    static List<DataSet> trainingSets = new List<DataSet>();
    static List<DataSet> testSets = new List<DataSet>();
    static  int numTrainingRecords = 50000;
    static int numTestRecords = 1000;

    static double learningRate = 0.0085;
    static double momentum = 0.005;

    bool trained = false;

    static int inputLayerSize = 64; //8 * 8 board
    static int[] hiddenLayersSizes = {128, 64};
    static int outputLayerSize = 32; //8 * 4 for possible moves

    public NeuralNetwork neuralNetwork;


    void Start()
    {
       //To train or not to train
       if(train)
       {

           LoadData(trainPath, trainingSets, numTrainingRecords);
           LoadData(testPath, testSets, numTestRecords);

            //neuralNetwork = new NeuralNetwork(28 * 28, new int[]{ 128, 128 }, 10, 0.2, 0.9); //75%
            //neuralNetwork = new NeuralNetwork(28 * 28, new int[]{ 392, 196 }, 10, 0.2, 0.9); //69%
            //neuralNetwork = new NeuralNetwork(28 * 28, new int[]{ 392, 98 }, 10, 0.2, 0.9); //75%
            neuralNetwork = new NeuralNetwork(inputLayerSize, hiddenLayersSizes, outputLayerSize, learningRate, momentum);

            StartCoroutine(TrainNeuralNetwork());
        }

    }

     void LoadData(string path, List<DataSet> dataSet, int numberOfRecords) {

        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");
        Debug.Log("Starting to read from: " + path);
        string[] readText = File.ReadAllLines(path);
        for(int i = 3; i < readText.Length; i += 4)
        {
            if(Convert.ToDouble(readText[i]) >= 0) //if the score is above zero
            {
                string[] boardString = readText[i - 2].Split(',');
                string[] moveString = readText[i - 1].Split(',');
               // Debug.Log(boardString.Length + ", " + moveString.Length);

                double[] values = new double[64];
                double[] targets = new double[32];
                for(int j = 0; j < 64; j++)
                {
                    try
                    {
                        values[j] = Convert.ToDouble(boardString[j]);
                    } catch(FormatException f) {}

                }

               // Debug.Log("Values read");
                for(int j = 0; j < 32; j++)
                {
                    try
                    {
                        targets[j] = Convert.ToDouble(moveString[j]);

                    } catch(FormatException f) {}
                }
               // Debug.Log("Targets read");
                DataSet set = new DataSet(values, targets);
                dataSet.Add(set);
               // Debug.Log(set.Values.Length + ", " + set.Targets.Length);

            }
        }   
        Debug.Log("Finished reading from: " + path);
     }

  void TestNeuralNetwork() {
        GameState gameState = GameObject.Find("Board").GetComponent<Board>().GetGameState();
            double numberCorrect = 0;
            foreach (DataSet dataSet in testSets) {
                double[] results = neuralNetwork.Compute(dataSet.Values);

                int resultNum = results.ToList().IndexOf(results.Max());
                int targetNum = dataSet.Targets.ToList().IndexOf(dataSet.Targets.Max());

                string s = "result: ";
                foreach (double result in results) {
                    s += string.Format("{0:0.00000}", result) + " ";
                }
                Move resultMove = gameState.GetMoveFromNN(results); 
                s += " (" + resultMove + ")";
                s += "\ntarget: ";
                foreach (double target in dataSet.Targets) {
                    s += string.Format("{0:0.00000}", target) + " ";
                }
                Move targetMove = gameState.GetMoveFromNN(dataSet.Targets); 
                s += " (" + targetMove + ")\n";

                bool isCorrect = false;
                if(resultMove.startX == targetMove.startX &&
                    resultMove.startY == targetMove.startY &&
                    resultMove.endX == targetMove.endX &&
                    resultMove.endY == targetMove.endY )
                    numberCorrect += 1.0;

                Debug.Log(s);
            }
            Debug.Log("Total correct: " + numberCorrect + "/" + testSets.Count);

            double percentageCorrect = numberCorrect/testSets.Count * 100.0;
            Debug.Log("Percentage correct: " + percentageCorrect.ToString("F2") + "%");
        }

        IEnumerator TrainNeuralNetwork() {
            Debug.Log("Training Started");
            List<double> errors = new List<double>();
            int count = 0;

            foreach (DataSet dataSet in trainingSets) {
                neuralNetwork.Train(dataSet, errors);
                count++;
                Debug.Log(count + "/" + trainingSets.Count);
                yield return null;
            }
            trained = true;
            Debug.Log("Training Completed");
        }


    // Update is called once per frame
    void Update()
    {
       if(trained)
        {
            TestNeuralNetwork();
            trained = false;
        }
    }
}
