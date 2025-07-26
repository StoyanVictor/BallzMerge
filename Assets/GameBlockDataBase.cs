using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Block/CreateBlockDataBase")]
public class GameBlockDataBase : ScriptableObject
{
    public List<GameBlock> gameBlocks = new List<GameBlock>();
}