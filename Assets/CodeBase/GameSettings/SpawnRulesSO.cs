using UnityEngine;
[CreateAssetMenu(fileName = "SpawnRules", menuName = "Game/SpawnRules")]
public class SpawnRulesSO : ScriptableObject
{
    public MoveRangeRule[] rules;

    public Block[] GetAllowedNumbers(int currentMove)
    {
        foreach (var rule in rules)
        {
            if (currentMove >= rule.minMove && currentMove <= rule.maxMove)
            {
                return rule.availableBlocks;
            }
        }
        return new Block[]{};
    }
}
[System.Serializable]
public class MoveRangeRule
{
    public int minMove;
    public int maxMove;
    public Block[] availableBlocks;
}
[System.Serializable]
public class Block
{
    public Color color;
    public string id;
}
