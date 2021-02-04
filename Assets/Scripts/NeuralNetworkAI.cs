using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetworkAI : IAIPlayable 
{
    NeuralNetwork neuralNetwork = GameObject.Find("NNTrainer").GetComponent<NNTrainer>().neuralNetwork; //new NeuralNetwork(inputLayerSize, hiddenLayersSizes, outputLayerSize, learningRate, momentum);
    static double learningRate = 0.0085;
    static double momentum = 0.005;

    static int inputLayerSize = 64; //8 * 8 board
    static int[] hiddenLayersSizes = {98, 80};
    static int outputLayerSize = 32; //8 * 4 for possible moves

    //Use trained data

    public GameState Move(GameState gameState, Teams team, int currentMove)
    {
        //DO COOL NN STUFF HERE
        double[] results = neuralNetwork.Compute(gameState.OutputCurrentBoardAsArray());

        Move move = gameState.GetMoveFromNN(results);
        Debug.Log(move);

        gameState.SetPossibleMoves(move.startX, move.startY, team, gameState.Tiles[move.startX, move.startY].unit.energy);
        gameState.Move(move.endX, move.endY, true);

        return gameState;
    }
}
