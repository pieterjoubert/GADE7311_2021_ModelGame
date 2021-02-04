using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMinimax : IAIPlayable {

    public int MaxDepth{ get; set; } = 3;

    //Evaluator evaluator = new Evaluator();

    public GameState Move(GameState gameState, Teams player, int currentMove){
        Debug.Log(player);
        GameState temp = Minimax(gameState, MaxDepth, player, int.MinValue, int.MaxValue, currentMove);
        Debug.Log("After minimx");
        Move move = temp.Moves[0];
        Debug.Log(move);
        gameState.SetPossibleMoves(move.startX, move.startY, player, gameState.Tiles[move.startX,move.startY].unit.energy);
        gameState.Move(move.endX, move.endY, true);

        Debug.Log("Moves[]");
        foreach(Move m in gameState.Moves)
        {
            Debug.Log("MoveList: " + m);
        }
        Debug.Log("Moves END");

        return gameState;
    }

    GameState Minimax(GameState gameState, int depth, Teams player, int alpha, int beta, int currentMove){

        bool hasLoser = gameState.BlueScore == 0 || gameState.RedScore == 0;

        //the game is done
        if (hasLoser || depth == 0) {
            Move newMove = gameState.Moves[currentMove]; 
            gameState.SetPossibleMoves(newMove.startX, newMove.startY, player, 0);
            gameState.Move(newMove.endX, newMove.endY, true);
            Debug.Log("Exiting recursion here...");
            return gameState;
        }

        int bestEvaluation = player == Teams.RED ? int.MinValue : int.MaxValue;
        GameState bestGameState = gameState.Duplicate();

        List<GameState> nextGameStates = gameState.GetAllPossibleNextStates(player);
        Debug.Log("======= TESTING GAME STATES AT ( " + depth + " )==========");
        Debug.Log("CURRENT MOVE: " + currentMove);
        Debug.Log("NUM STATES: " + nextGameStates.Count);

        foreach (GameState currentState in nextGameStates) {

            //NEW MOVE MIGHT NOT BE CURRENTLY LEGAL ???
            GameState newGameState = Minimax(currentState, depth-1, gameState.FindOpposingTeam(player), alpha, beta, currentMove).Duplicate();
            /*Move newMove = newGameState.Moves[currentMove]; 
            newGameState.SetPossibleMoves(newMove.startX, newMove.startY, player, 0);
            newGameState.Move(newMove.endX, newMove.endY, true);*/
           // Debug.Log("Made move: " + newMove + " at tree depth: " + depth + " num moves: " + newGameState.Moves.Count);

            int newEvaluation;
            newEvaluation = newGameState.GetScore(player);

            if (player == Teams.RED) {
                if(newEvaluation > bestEvaluation) {
                    bestEvaluation = newEvaluation;
                    bestGameState = newGameState;
                }

                //alpha beta pruning for maximizing player
                alpha = Mathf.Max(alpha, bestEvaluation);
                if(alpha >= beta){
                    break;
                }
            }
            else {
                if (newEvaluation < bestEvaluation) {
                    bestEvaluation = newEvaluation;
                    bestGameState = newGameState;
                }

                //alpha beta pruning for minimizing player
                beta = Mathf.Min(beta, bestEvaluation);
                if(beta <= alpha){
                    break;
                }
            }
        }
        return bestGameState; 
    }
}
