using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIPlayable 
{
    GameState Move(GameState gameState, Teams team);
}
