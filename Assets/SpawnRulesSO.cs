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
        // Если не нашли правило - возвращаем дефолт
        return new Block[]{};
    }
}
[System.Serializable]
public class MoveRangeRule
{
    public int minMove;
    public int maxMove;
    public Block[] availableBlocks; // Например: [1, 2, 3]
}
[System.Serializable]
public class Block
{
    public Color color;
    public string id;
}
