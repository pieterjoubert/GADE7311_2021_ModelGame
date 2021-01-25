using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Teams
{
    RED,
    BLUE,
    NONE
}

public struct Unit
{
    public Teams team;
    public int energy;
}

public struct Tile
{
    public bool lightValue;
    public Unit unit;
    public Teams possibleMove;
}
public class GameState : ScriptableObject
{
    public Tile[,] Tiles;

    public int Width { get; set; }
    public int Height { get; set; }
    public ProcGenTypes ProcGen { get; set; }
    public GameState(int width, int height, ProcGenTypes procGen, int startingEnergy)
    {
        Width = width;
        Height = height;
        Tiles = new Tile[Width, Height];
        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                Tiles[x, y].unit.energy = startingEnergy;
                Tiles[x, y].unit.team = Teams.NONE;
                Tiles[x, y].possibleMove = Teams.NONE;
            }
        }

        ProceduralGeneration(procGen);
        PlacePieces();
    }

     void ProceduralGeneration(ProcGenTypes procGen)
    {
        if(procGen == ProcGenTypes.DEFAULT) //Middle 4 squares open
        {
            for(int x = 0; x < Width; x++)
            {
                for(int y = 0; y < Height; y++)
                {
                    if(x > 2 && x < 5 && y > 2 && y < 5)
                    {
                        Tiles[x, y].lightValue = false;
                    }
                    else
                    {
                        Tiles[x, y].lightValue = true;
                    }
                }
            }


        }
        else if(procGen == ProcGenTypes.RANDOM)
        {
            for(int x = 0; x < Width; x++)
            {
                for(int y = 0; y < Height; y++)
                {
                    if(Random.Range(0f,1f) >= 0.25f)
                    {
                        Tiles[x, y].lightValue = true;
                    }
                }
            }
        }

    }

    void PlacePieces()
    {

        for (int x = 0; x < Width; x++)
        {
            Tiles[x, 0].unit.team = Teams.RED;
        }

        for (int x = 0; x < Width; x++)
        {
            Tiles[x, Height - 1].unit.team = Teams.BLUE;
        }
        
    }

    public void ClearPossibleMoves()
    {
        for(int x = 0; x < Width; x++)
            {
                for(int y = 0; y < Height; y++)
                {
                    Tiles[x, y].possibleMove = Teams.NONE;
                }
            }

    }

    public void SetPossibleMoves(int x, int z, Teams team, int energy)
    {
        //CLEAR CURRENT MOVES
        ClearPossibleMoves();

        //NORTHERN PATH
        for (int t = z; t < z + energy; t++) 
        {
            if (t < 0 || t >= Height) break;
            Tiles[x, t].possibleMove = team;
        }
        //SOUTHERN PATH
        for (int t = z; t > z - energy; t--) 
        {
            if (t < 0 || t >= Height) break;
            Tiles[x, t].possibleMove = team;

        }
        //EASTERN PATH
        for (int t = x; t < x + energy; t++) 
        {
            if (t < 0 || t >= Width) break;
            Tiles[t, z].possibleMove = team;
        }
        //WESTERN PATH
        for (int t = x; t > x - energy; t--) 
        {
            if (t < 0 || t >= Width) break;
            Tiles[t, z].possibleMove = team;
        }
    }



    

}
