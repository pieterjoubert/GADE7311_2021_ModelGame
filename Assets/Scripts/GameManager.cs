using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum AIOptions
{
    RANDOM,
    NEURALNETWORK,
    EVOLUTIONARY,
    QLEARNING
}
public class GameManager : MonoBehaviour
{
    Teams currentTeam;
    Teams humanPlayer;
    Teams AIPlayer;
    IAIPlayable AI;

    public AIOptions aiOptions; 
    // Start is called before the first frame update
    void Start()
    {
        currentTeam = Teams.RED; 
        humanPlayer = Teams.RED;
        AIPlayer = Teams.BLUE;

        if(aiOptions == AIOptions.RANDOM)
        {
            AI = new RandomAIPlayer();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeTurn(Tile tile, int x, int z)
    {
        Board board = GameObject.Find("Board").GetComponent<Board>();
        if(currentTeam == humanPlayer) //HUMAN PLAYER MOVE
        {
            if(tile.possibleMove == Teams.NONE)
            {
                //Select Unit
                if(tile.unit.team != Teams.NONE)
                {
                    board.GetGameState().SetPossibleMoves(x, z, tile.unit.team, tile.unit.energy);
                }
            }
            else
            {
                //Make Move
                board.GetGameState().Move(x, z);
                currentTeam = AIPlayer; 
                board.Draw();
                TakeTurn(tile, x, z);
            }
            board.Draw();
        }
        else if(currentTeam == AIPlayer) //AI PLAYER
        {
            Debug.Log("AI Moving...");
            board.SetGameState(AI.Move(board.GetGameState(), AIPlayer));
            board.Draw();
            currentTeam = humanPlayer;
        }
    }

   }
