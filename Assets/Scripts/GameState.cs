using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Globalization;

public enum Teams
{
    RED,
    BLUE,
    NONE
}

public struct Move
{
    public int startX;
    public int startY;
    public int endX;
    public int endY;

    public override string ToString()
    {
        return "(" + startX + ", " + startY + ") -> (" + endX + ", " + endY + ")";
    }

    public string ToNNString()
    {
        string temp = "";
        for(int i = 0; i < 8; i++) temp += startX == i ? "1.0," : "0.0,";
        for(int i = 0; i < 8; i++) temp += startY == i ? "1.0," : "0.0,";
        for(int i = 0; i < 8; i++) temp += endX == i ? "1.0," : "0.0,";
        for(int i = 0; i < 8; i++) temp += endY == i ? "1.0," : "0.0,";
        return temp;
    }

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
    public int RedScore { get; set; }
    public int BlueScore { get; set; }

    public List<Move> Moves { get; set; }

    private Move lastMove;
    private GameState prevState;

    public Move LastMove
    {
        get { return lastMove; }
        set { lastMove = value; }
    }

    public GameState(int width, int height, ProcGenTypes procGen, int startingEnergy)
    {
        Moves = new List<Move>();
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

    public GameState(Tile[,] tiles){
        this.Tiles = tiles;
          
    }
   public GameState(){
    }


    public GameState Duplicate(){
        GameState newGameState = new GameState(Tiles.Clone() as Tile[,]);
        newGameState.Width = Width;
        newGameState.Height = Height;
        newGameState.Moves = new List<Move>(Moves);
        return newGameState;
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
                        Tiles[x, y].lightValue = true;
                    }
                    else
                    {
                        Tiles[x, y].lightValue = false;
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
                    if(Random.Range(0f,1f) <= 0.25f)
                    {
                        Tiles[x, y].lightValue = true;
                    }
                }
            }
        }
        else if(procGen == ProcGenTypes.PERLIN)
        {
            float scale = 2f;
            float xOrg = 20f * Random.Range(-10f, 10f);
            float yOrg = 20f * Random.Range(-10f, 10f);

            for(int x = 0; x < Width; x++)
            {
                for(int y = 0; y < Height; y++)
                {
                    float xCoord = ((xOrg + x) / Width) * scale;
                    float yCoord = ((yOrg + y) / Height) * scale;
                    float height = Mathf.PerlinNoise(xCoord, yCoord) * 10f;

                    if(height >= 4)
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
        //CAP ENERGY
        if (energy > 9) energy = 9;

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

    public void Move(int x, int z, bool realMove)
    {
        prevState = this.Duplicate();

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
            if(z - SelectedZ > 0 && z + 1 < Height && Tiles[x, z + 1].unit.team == Teams.NONE) // North
            {
                Tiles[x, z + 1].unit.team = opposingTeam;
                Tiles[x, z + 1].unit.energy = Tiles[x, z].unit.energy;
                Tiles[x, z].unit.energy = Tiles[SelectedX, SelectedZ].unit.energy - distance;
                Tiles[x, z].unit.team = Tiles[SelectedX, SelectedZ].unit.team;
                Tiles[SelectedX, SelectedZ].unit.team = Teams.NONE;
                Tiles[SelectedX, SelectedZ].unit.energy = 0;
            }
            if(z - SelectedZ < 0 && z - 1 > 0 && Tiles[x, z - 1].unit.team == Teams.NONE) // South
            {
                Tiles[x, z - 1].unit.team = opposingTeam;
                Tiles[x, z - 1].unit.energy = Tiles[x, z].unit.energy;
                Tiles[x, z].unit.energy = Tiles[SelectedX, SelectedZ].unit.energy - distance;
                Tiles[x, z].unit.team = Tiles[SelectedX, SelectedZ].unit.team;
                Tiles[SelectedX, SelectedZ].unit.team = Teams.NONE;
                Tiles[SelectedX, SelectedZ].unit.energy = 0;
            }
            if(x - SelectedX > 0 && x + 1 < Width && Tiles[x + 1, z].unit.team == Teams.NONE) // East
            {
                Tiles[x + 1, z].unit.team = opposingTeam;
                Tiles[x + 1, z].unit.energy = Tiles[x, z].unit.energy;
                Tiles[x, z].unit.energy = Tiles[SelectedX, SelectedZ].unit.energy - distance;
                Tiles[x, z].unit.team = Tiles[SelectedX, SelectedZ].unit.team;
                Tiles[SelectedX, SelectedZ].unit.team = Teams.NONE;
                Tiles[SelectedX, SelectedZ].unit.energy = 0;
            }
            if(x - SelectedX < 0 && x - 1 > 0 && Tiles[x - 1, z].unit.team == Teams.NONE) // West
            {
                Tiles[x - 1, z].unit.team = opposingTeam;
                Tiles[x - 1, z].unit.energy = Tiles[x, z].unit.energy;
                Tiles[x, z].unit.energy = Tiles[SelectedX, SelectedZ].unit.energy - distance;
                Tiles[x, z].unit.team = Tiles[SelectedX, SelectedZ].unit.team;
                Tiles[SelectedX, SelectedZ].unit.team = Teams.NONE;
                Tiles[SelectedX, SelectedZ].unit.energy = 0;
            }

        }
        UpdateEnergy();
        UpdateScore();
        ClearPossibleMoves();
        lastMove.startX = SelectedX;
        lastMove.startY = SelectedZ;
        lastMove.endX = x;
        lastMove.endY = z;
        if (realMove)
        {
            Moves.Add(LastMove);

            //Debug.Log("MADE MOVE: " + lastMove);
        }
    }
    public void UpdateEnergy()
    { 
        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                if(Tiles[x,y].lightValue && Tiles[x,y].unit.energy < 9)
                {
                    Tiles[x,y].unit.energy++;
                }
            }
        }
    }

    public void UpdateScore()
    {
        BlueScore = 0;
        RedScore = 0;

        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                if(Tiles[x,y].unit.team == Teams.BLUE)
                {
                    BlueScore += Tiles[x, y].unit.energy;
                    if (Tiles[x, y].lightValue)
                        BlueScore += 10;
                }
                else if(Tiles[x,y].unit.team == Teams.RED)
                {
                    RedScore += Tiles[x, y].unit.energy;
                    if (Tiles[x, y].lightValue)
                        RedScore += 10;
                }
            }
        }

    }

    public Teams GetWinner()
    {
        if(RedScore == 0)
        {
            return Teams.BLUE;
        }
        else if(BlueScore == 0)
        {
            return Teams.RED;
        }
        else
        {
            return Teams.NONE;
        }
    }

    public int GetScore(Teams team)
    {
        if (team == Teams.RED)
        {
            return RedScore;
        }
        else
        {
            return BlueScore;
        }
    }

   public List<GameState> GetAllPossibleNextStates(Teams team)
    {
        List<GameState> possibleStates = new List<GameState>();

        GameState temp = new GameState();
        temp = this.Duplicate();
        temp.Moves = new List<Move>();

        for(int x = 0; x < temp.Width; x++)
        {
            for(int y = 0; y < temp.Height; y++)
            {
                if(temp.Tiles[x,y].unit.team == team && temp.Tiles[x,y].unit.energy > 0)
                {
                    temp.SetPossibleMoves(x, y, team,temp.Tiles[x,y].unit.energy);
                    for(int x2 = 0; x2 < temp.Width; x2++)
                    {
                        for(int y2 = 0; y2 < temp.Height; y2++)
                        {
                            if(temp.Tiles[x2,y2].possibleMove == team)
                            {
                                temp.Move(x2, y2, true);
                                possibleStates.Add(temp);
                                temp = this.Duplicate();
                                temp.SetPossibleMoves(x, y, team,temp.Tiles[x,y].unit.energy);
                            }
                        }
                    }
                }
                //temp = this.Duplicate();
                temp.ClearPossibleMoves();
            }
        }

        return possibleStates;
    }

    public Teams FindOpposingTeam(Teams team)
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

    public string OutputCurrentBoard()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");

        string temp = Moves.Count - 1 + ",\n";
        for(int y = Height - 1; y >= 0; y--)
        {
            for(int x = 0; x < Width; x++)
            {
                if(prevState.Tiles[x,y].unit.team == Teams.RED)
                {
                    temp += (prevState.Tiles[x, y].unit.energy / 10f) + ",";
                }
                else if(Tiles[x,y].unit.team == Teams.BLUE)
                {
                    temp += (prevState.Tiles[x, y].unit.energy / -10f) + ",";
                }
                else
                {
                    temp += "0.0,";
                }
            }
        }

        //temp += lastMove + "\n";
        temp += "\n" + lastMove.ToNNString() + "\n";
        temp += (Moves.Count - 1) % 2 == 0 ? RedScore - BlueScore : BlueScore - RedScore;
        temp += "\n";

        return temp;
    }

    public double[] OutputCurrentBoardAsArray()
    {
        double[] output = new double[64];
        int count = 0;
        for(int y = Height - 1; y >= 0; y--)
        {
            for(int x = 0; x < Width; x++)
            {
                if(Tiles[x,y].unit.team == Teams.RED)
                {
                    output[count++] = Tiles[x, y].unit.energy / 10d;
                }
                else if(Tiles[x,y].unit.team == Teams.BLUE)
                {
                    output[count++] = Tiles[x, y].unit.energy / -10d;
                }
                else
                {
                    output[count++] = 0.0;
                }
            }
        }

        return output;
    }

    public Move GetMoveFromNN(double[] results)
    {
        Move move = new Move();
        double startX = results[0];
        double startY = results[8];
        double endX = results[16];
        double endY = results[24];

        for(int i = 0; i < 8; i++)
        {
            if (results[i] > startX)
            {
                startX = results[i];
                move.startX = i;
            }
        }
        for(int i = 8; i < 16; i++)
        {
            if (results[i] > startY)
            {
                startY = results[i];
                move.startY = i - 8;
            }
        }
        for(int i = 16; i < 24; i++)
        {
            if (results[i] > endX)
            {
                endX = results[i];
                move.endX = i - 16;
            }
        }
        for(int i = 24; i < 32; i++)
        {
            if (results[i] > endY)
            {
                endY = results[i];
                move.startX = i - 24;
            }
        }

        return move;
    }
}
