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
    public GameObject tilePrefab;
    public GameObject lightBlockPrefab;
    public GameObject redUnit;
    public GameObject blueUnit;
    public int boardWidth;
    public int boardHeight;
    public ProcGenTypes procGen;
    // Start is called before the first frame update
    void Start()
    {
        gameState = new GameState(boardWidth, boardHeight, procGen, 6);
        Draw();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Draw()
    {
        GameObject[] redrawObjects = GameObject.FindGameObjectsWithTag("Drawable");
        foreach (GameObject go in redrawObjects) Destroy(go);

         for(int x = 0; x < gameState.Width; x++)
         {
            for(int z = 0; z < gameState.Height; z++)
            {
                //BASE TILES
                float y = 0f;
                GameObject go = Instantiate(tilePrefab, new Vector3(x, y, z), Quaternion.identity);
                go.name = "Tile" + x + z;

                //POSSILBE MOVES
                if (gameState.Tiles[x, z].possibleMove == Teams.RED)
                {
                    Renderer renderer = go.GetComponent<Renderer>();
                    renderer.material.SetColor("_Color", Color.red * 0.75f);
                }
                else if (gameState.Tiles[x, z].possibleMove == Teams.BLUE) 
                {
                    Renderer renderer = go.GetComponent<Renderer>();
                    renderer.material.SetColor("_Color", Color.blue * 0.75f);
                }
            
                //LIGHT BLOCKERS
                if(gameState.Tiles[x,z].lightValue)
                {
                   Instantiate(lightBlockPrefab, new Vector3(x, 9f , z), Quaternion.identity);

                }

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

    /*public void SetTileColor(int x, int z, Color color)
    {
        GameObject go = GameObject.Find("Tile" + x.ToString() + z.ToString());
        Renderer renderer = go.GetComponent<Renderer>();
        renderer.material.SetColor("_Color", color);
    }*/

    public GameState GetGameState()
    {
        return gameState;
    }
    public void SetGameState(GameState _gameState)
    {
        gameState = _gameState;
    }
}
