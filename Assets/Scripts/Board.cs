using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int boardWidth;
    public int boardHeight;
    public ProcGenTypes procGen;
    // Start is called before the first frame update
    void Start()
    {
        gameState = new GameState(boardWidth, boardHeight, procGen);
         for(int x = 0; x < gameState.Width; x++)
         {
            for(int z = 0; z < gameState.Height; z++)
            {
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

                if(gameState.Tiles[x,z].lightValue)
                   Instantiate(lightBlockPrefab, new Vector3(x, 9f , z), Quaternion.identity);
                 /*    Renderer renderer = go.GetComponent<Renderer> ();
                    Material mat = renderer.material;
                Color baseColor = mat.color;
                baseColor.a = 1f - gameState.Tiles[x, z].lightValue;
                mat.SetColor("_Color", baseColor);*/
            }
         }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
   }
