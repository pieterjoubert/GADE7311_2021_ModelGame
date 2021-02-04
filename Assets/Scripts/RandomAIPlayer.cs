using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAIPlayer : ScriptableObject, IAIPlayable
{
    public GameState Move(GameState gameState, Teams team, int currentMove)
    {
        //DO COOL AI STUFF HERE
        //STEP 1: CHOOSE (RANDOM) VALID PIECE TO MOVE
        List<Tile> possibleTiles = new List<Tile>();
        for(int x = 0; x < gameState.Width; x++)
        {
            for(int y = 0; y < gameState.Height; y++)
            {
                if(gameState.Tiles[x,y].unit.team == team && gameState.Tiles[x,y].unit.energy > 0)
                {
                    possibleTiles.Add(gameState.Tiles[x, y]);
                }
            }
        }

        int chosenPos = Random.Range(0, possibleTiles.Count);
        //Debug.Log(chosenPos + "," + (possibleTiles.Count));
        Tile chosenTile = possibleTiles[chosenPos];
        gameState.SetPossibleMoves(chosenTile.x, chosenTile.y, team, chosenTile.unit.energy);

        //STEP 2: MOVE A (RANDOM) VALID POSITION
        List<Tile> possibleMoves = new List<Tile>();
        for(int x = 0; x < gameState.Width; x++)
        {
            for(int y = 0; y < gameState.Height; y++)
            {
                if(gameState.Tiles[x,y].possibleMove == team)
                {
                    possibleMoves.Add(gameState.Tiles[x, y]);
                   // Debug.Log("Possible x,y: " + x + ", " + y);
                }
            }
        }

        if (possibleMoves.Count == 0)
        {
           // Debug.Log("Blocked unit: " + team);
            gameState.ClearPossibleMoves();
            if(chosenTile.unit.energy <= gameState.GetScore(team))
            {
               // Debug.Log("Blocked Loss");
                return gameState;
            }
            else
            {
                Move(gameState, team, currentMove); // skip blocked units
            }
        }

        chosenPos = Random.Range(0, possibleMoves.Count);
        //Debug.Log(chosenPos + "," + (possibleMoves.Count));
        Tile moveToTile = possibleMoves[chosenPos];
        //Debug.Log("Chosen x,y: " + moveToTile.x + ", " + moveToTile.y);
        gameState.Move(moveToTile.x, moveToTile.y, true);
        //Debug.Log("Move gameState team: " + gameState.Tiles[moveToTile.x, moveToTile.y].unit.team);
        return gameState;
    }
}
