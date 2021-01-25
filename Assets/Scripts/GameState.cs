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
}
public class GameState : ScriptableObject
{
    public Tile[,] Tiles;

    public int Width { get; set; }
    public int Height { get; set; }
    public ProcGenTypes ProcGen { get; set; }
    public GameState(int width, int height, ProcGenTypes procGen)
    {
        Width = width;
        Height = height;
        Tiles = new Tile[Width, Height];
        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                Tiles[x, y].unit.energy = 0;
                Tiles[x, y].unit.team = Teams.NONE;
            }
        }
        ProceduralGeneration(procGen);
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
                    if(Random.Range(0f,1f) >= 0.5f)
                    {
                        Tiles[x, y].lightValue = true;
                    }
                }
            }
        }

    }

}
