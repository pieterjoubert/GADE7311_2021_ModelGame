using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public enum PlayerOptions
{
    HUMAN,
    RANDOM,
    MINIMAX,
    NEURALNETWORK,
    EVOLUTIONARY,
    QLEARNING
}
public class GameManager : MonoBehaviour
{
    PlayerOptions currentAI;
    Teams currentTeam;
    IAIPlayable AI1;
    IAIPlayable AI2;
    int currentMove;
    GameState prevGameState;

    public PlayerOptions playerOptions1; 
    public PlayerOptions playerOptions2;
    public bool Simulate;
    public int Simulations;
    public string path;
    public float simulationInterval;

    // Start is called before the first frame update
    void Start()
    {
        if (!GameObject.Find("NNTrainer").GetComponent<NNTrainer>().train)
        {

            currentAI = playerOptions1;
            currentTeam = Teams.RED;
            currentMove = 0;

            if (playerOptions1 == PlayerOptions.RANDOM)
            {
                AI1 = new RandomAIPlayer();
            }
            else if (playerOptions1 == PlayerOptions.MINIMAX)
            {
                AI1 = new AIMinimax();
            }
            else if (playerOptions1 == PlayerOptions.NEURALNETWORK)
            {
                AI1 = new NeuralNetworkAI();
            }
            else if (playerOptions1 == PlayerOptions.HUMAN)
            {
            }

            if (playerOptions2 == PlayerOptions.RANDOM)
            {
                AI2 = new RandomAIPlayer();
            }
            else if (playerOptions2 == PlayerOptions.MINIMAX)
            {
                AI2 = new AIMinimax();
            }
            else if (playerOptions2 == PlayerOptions.NEURALNETWORK)
            {
                AI2 = new NeuralNetworkAI();
            }
            else if (playerOptions2 == PlayerOptions.HUMAN)
            {
            }

            if (playerOptions1 != PlayerOptions.HUMAN)
            {
                StartCoroutine(TakeTurn(new Tile(), 0, 0));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator TakeTurn(Tile tile, int x, int z)
    {
        Board board = GameObject.Find("Board").GetComponent<Board>();
        board.GetGameState().UpdateScore();
        //Debug.Log(board.GetGameState().GetWinner());
        if (Simulations > 0)
        {

            if (board.GetGameState().GetWinner() == Teams.NONE)
            {
                if (currentAI == PlayerOptions.HUMAN) //HUMAN PLAYER MOVE
                {
                    if (tile.possibleMove == Teams.NONE)
                    {
                        //Select Unit
                        if (tile.unit.team != Teams.NONE)
                        {
                            board.GetGameState().SetPossibleMoves(x, z, tile.unit.team, tile.unit.energy);
                        }
                    }
                    else
                    {
                        //Make Move
                        board.GetGameState().Move(x, z, true);
                        currentAI = currentTeam == Teams.RED ? playerOptions2 : playerOptions1;
                        currentTeam = currentTeam == Teams.RED ? Teams.BLUE : Teams.RED;
                        if(!Simulate) board.Draw();
                        currentMove++;
                        //Debug.Log(board.GetGameState().OutputCurrentBoard());
                        File.AppendAllText(path,board.GetGameState().OutputCurrentBoard());
                        StartCoroutine(TakeTurn(tile, x, z));
                    }
                    board.Draw();
                }
                else if (currentAI != PlayerOptions.HUMAN) //AI PLAYER
                {
                    if (currentTeam == Teams.RED)
                    {
                        board.SetGameState(AI1.Move(board.GetGameState(), currentTeam, currentMove));
                        if (!Simulate) board.Draw();
                        yield return new WaitForSeconds(simulationInterval);
                        currentTeam = Teams.BLUE;
                        currentAI = playerOptions2;
                        currentMove++;
                        //Debug.Log(board.GetGameState().OutputCurrentBoard());
                        File.AppendAllText(path,board.GetGameState().OutputCurrentBoard());
                        if (playerOptions2 != PlayerOptions.HUMAN) StartCoroutine(TakeTurn(new Tile(), 0, 0));
                    }
                    else if (currentTeam == Teams.BLUE)
                    {
                        board.SetGameState(AI2.Move(board.GetGameState(), currentTeam, currentMove));
                        if(!Simulate) board.Draw();
                        yield return new WaitForSeconds(simulationInterval);
                        currentTeam = Teams.RED;
                        currentAI = playerOptions1;
                        currentMove++;
                        //Debug.Log(board.GetGameState().OutputCurrentBoard());
                        File.AppendAllText(path,board.GetGameState().OutputCurrentBoard());
                        if (playerOptions1 != PlayerOptions.HUMAN) StartCoroutine(TakeTurn(new Tile(), 0, 0));
                    }
                }

            }
            else
            {
                if (Simulate)
                {
                    board.Draw();
                    board.SetGameState(new GameState(board.boardWidth, board.boardHeight, board.procGen, 6));

                    currentAI = playerOptions1;
                    currentTeam = Teams.RED;
                    currentMove = 0;

                    if(playerOptions1 == PlayerOptions.RANDOM)
                    {
                        AI1 = new RandomAIPlayer();
                    }
                    else if (playerOptions1 == PlayerOptions.MINIMAX)
                    {
                        AI1 = new AIMinimax();
                    }
                    else if (playerOptions1 == PlayerOptions.HUMAN)
                    {
                    }

                    if(playerOptions2 == PlayerOptions.RANDOM)
                    {
                        AI2 = new RandomAIPlayer();
                    }
                    else if (playerOptions2 == PlayerOptions.MINIMAX)
                    {
                        AI2 = new AIMinimax();
                    }
                    else if (playerOptions2 == PlayerOptions.HUMAN)
                    {
                    }

                    if(playerOptions1 != PlayerOptions.HUMAN)
                    {
                        StartCoroutine(TakeTurn(new Tile(), 0, 0));
                    }

                    Simulations--;
                }
            }
        }

    }

   }
