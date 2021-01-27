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
    public int x;
    public int y;
}
public class GameState : ScriptableObject
{
    public Tile[,] Tiles;

    public int Width { get; set; }
    public int Height { get; set; }
    public ProcGenTypes ProcGen { get; set; }
    public int SelectedX { get; set; }
    public int SelectedZ { get; set; }
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
                Tiles[x, y].x = x;
                Tiles[x, y].y = y;
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

        //SET SELECTED PIECE
        SelectedX = x;
        SelectedZ = z;

        //NORTHERN PATH
        for (int t = z + 1; t < z + 1 + energy; t++) 
        {
            if (t < 0 || t >= Height) break;
            if (Tiles[x, t].unit.team == team) break;
            if (Tiles[x, t].unit.team != team && Tiles[x,t].unit.team != Teams.NONE)
            {
                Tiles[x, t].possibleMove = team;
                break;
            }
            Tiles[x, t].possibleMove = team;
        }
        //SOUTHERN PATH
        for (int t = z - 1; t > z - 1 - energy; t--) 
        {
            if (t < 0 || t >= Height) break;
            if (Tiles[x, t].unit.team == team) break;
            if (Tiles[x, t].unit.team != team && Tiles[x, t].unit.team != Teams.NONE)
            {
                Tiles[x, t].possibleMove = team;
                break;
            }
            Tiles[x, t].possibleMove = team;

        }
        //EASTERN PATH
        for (int t = x + 1; t < x + 1 + energy; t++) 
        {
            if (t < 0 || t >= Width) break;
            if (Tiles[t, z].unit.team == team) break;
            if (Tiles[t, z].unit.team != team && Tiles[t, z].unit.team != Teams.NONE)
            {
                Tiles[t, z].possibleMove = team;
                break;
            }
            Tiles[t, z].possibleMove = team;
        }
        //WESTERN PATH
        for (int t = x - 1; t > x - 1 - energy; t--) 
        {
            if (t < 0 || t >= Width) break;
            if (Tiles[t, z].unit.team == team) break;
            if (Tiles[t, z].unit.team != team && Tiles[t, z].unit.team != Teams.NONE)
            {
                Tiles[t, z].possibleMove = team;
                break;
            }
            Tiles[t, z].possibleMove = team;
        }
    }

    public void Move(int x, int z)
    {
        int distance = Mathf.Abs(x - SelectedX) + Mathf.Abs(z - SelectedZ);
        if (Tiles[x, z].unit.team == Teams.NONE)
        {
            Tiles[x, z].unit.team = Tiles[SelectedX, SelectedZ].unit.team;
            Tiles[SelectedX, SelectedZ].unit.team = Teams.NONE;
            Tiles[x, z].unit.energy = Tiles[SelectedX, SelectedZ].unit.energy - distance;
            Tiles[SelectedX, SelectedZ].unit.energy = 0;
        }
        else if (Tiles[x, z].unit.team != Tiles[SelectedX, SelectedZ].unit.team)
        {
            Teams opposingTeam = FindOpposingTeam(Tiles[SelectedX, SelectedZ].unit.team);
            if(z - SelectedZ > 0 && Tiles[x, z + 1].unit.team == Teams.NONE) // North
            {
                Tiles[x, z + 1].unit.team = opposingTeam;
                Tiles[x, z + 1].unit.energy = Tiles[x, z].unit.energy;
                Tiles[x, z].unit.energy = Tiles[SelectedX, SelectedZ].unit.energy - distance;
                Tiles[x, z].unit.team = Tiles[SelectedX, SelectedZ].unit.team;
                Tiles[SelectedX, SelectedZ].unit.team = Teams.NONE;
                Tiles[SelectedX, SelectedZ].unit.energy = 0;
            }
            if(z - SelectedZ < 0 && Tiles[x, z - 1].unit.team == Teams.NONE) // South
            {
                Tiles[x, z - 1].unit.team = opposingTeam;
                Tiles[x, z - 1].unit.energy = Tiles[x, z].unit.energy;
                Tiles[x, z].unit.energy = Tiles[SelectedX, SelectedZ].unit.energy - distance;
                Tiles[x, z].unit.team = Tiles[SelectedX, SelectedZ].unit.team;
                Tiles[SelectedX, SelectedZ].unit.team = Teams.NONE;
                Tiles[SelectedX, SelectedZ].unit.energy = 0;
            }
            if(x - SelectedX > 0 &&  Tiles[x + 1, z].unit.team == Teams.NONE) // East
            {
                Tiles[x + 1, z].unit.team = opposingTeam;
                Tiles[x + 1, z].unit.energy = Tiles[x, z].unit.energy;
                Tiles[x, z].unit.energy = Tiles[SelectedX, SelectedZ].unit.energy - distance;
                Tiles[x, z].unit.team = Tiles[SelectedX, SelectedZ].unit.team;
                Tiles[SelectedX, SelectedZ].unit.team = Teams.NONE;
                Tiles[SelectedX, SelectedZ].unit.energy = 0;
            }
            if(x - SelectedX < 0 && Tiles[x - 1, z].unit.team == Teams.NONE) // West
            {
                Tiles[x - 1, z].unit.team = opposingTeam;
                Tiles[x - 1, z].unit.energy = Tiles[x, z].unit.energy;
                Tiles[x, z].unit.energy = Tiles[SelectedX, SelectedZ].unit.energy - distance;
                Tiles[x, z].unit.team = Tiles[SelectedX, SelectedZ].unit.team;
                Tiles[SelectedX, SelectedZ].unit.team = Teams.NONE;
                Tiles[SelectedX, SelectedZ].unit.energy = 0;
            }

        }

        ClearPossibleMoves();
    }

    private Teams FindOpposingTeam(Teams team)
    {
        if (team == Teams.RED)
        {
            return Teams.BLUE;
        }
        else if (team == Teams.BLUE)
        {
            return Teams.RED;
        }
        else
        {
            return Teams.NONE;
        }
    }

}
