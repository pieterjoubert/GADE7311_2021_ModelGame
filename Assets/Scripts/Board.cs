using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ProcGenTypes
{
    DEFAULT,
    RANDOM,
    PERLIN
}
public class Board : MonoBehaviour
{
    private GameState gameState;
    public GameObject lightTilePrefab;
    public GameObject darkTilePrefab;
    public GameObject lightBlockPrefab;
    public GameObject redUnit;
    public GameObject blueUnit;
    public int boardWidth;
    public int boardHeight;
    public ProcGenTypes procGen;
    // Start is called before the first frame update
    void Start()
    {
        gameState = new GameState(boardWidth, boardHeight, procGen, 4);
        Draw();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Draw()
    {
         for(int x = 0; x < gameState.Width; x++)
         {
            for(int z = 0; z < gameState.Height; z++)
            {
                //ALTERNATIVE DARK AND LIGHT TILES
                float y = 0f;
                if(x % 2 == 0)
                {
                    if (z % 2 == 0)
                    {
                        Instantiate(lightTilePrefab, new Vector3(x, y, z), Quaternion.identity);
                    } 
                    else
                    {
                        Instantiate(darkTilePrefab, new Vector3(x, y, z), Quaternion.identity);
                    }
                }
                else
                {
                    if (z % 2 != 0)
                    {
                        Instantiate(lightTilePrefab, new Vector3(x, y, z), Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(darkTilePrefab, new Vector3(x, y, z), Quaternion.identity);
                    }
                }
                
                //LIGHT BLOCKERS
                if(gameState.Tiles[x,z].lightValue)
                {
                   Instantiate(lightBlockPrefab, new Vector3(x, 9f , z), Quaternion.identity);
                }


                GameObject go = new GameObject();
                //Units
                if (gameState.Tiles[x, z].unit.team == Teams.RED)
                {
                    go = Instantiate(redUnit, new Vector3(x, y + 0.5f, z), Quaternion.identity);
                    go.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = gameState.Tiles[x, z].unit.energy.ToString();
                }
                else if (gameState.Tiles[x, z].unit.team == Teams.BLUE)
                { 
                    go = Instantiate(blueUnit, new Vector3(x, y + 0.5f, z), Quaternion.identity);
                    go.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = gameState.Tiles[x, z].unit.energy.ToString();
                }
                

            }
         }

    }
}
