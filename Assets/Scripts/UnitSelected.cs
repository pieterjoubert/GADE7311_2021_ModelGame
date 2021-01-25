using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using System; 

public class UnitSelected : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {

        Board board = GameObject.Find("Board").GetComponent<Board>();
        int x = Convert.ToInt32(gameObject.name.Substring(4,1));
        int z = Convert.ToInt32(gameObject.name.Substring(5,1));
        Tile tile = board.GetGameState().Tiles[x, z];
        board.GetGameState().SetPossibleMoves(x, z, tile.unit.team, tile.unit.energy);
        board.Draw();
    }  
}
