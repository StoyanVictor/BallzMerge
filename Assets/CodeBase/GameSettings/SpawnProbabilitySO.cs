using UnityEngine;

[CreateAssetMenu(fileName = "SpawnProbabilities", menuName = "Game/SpawnProbabilities")]
public class SpawnProbabilitySO : ScriptableObject
{
   

    public MoveRangeProbability[] moveRanges;

    public int GetBlocksToSpawn(int currentMove)
    {
        foreach (var range in moveRanges)
        {
            if (currentMove >= range.minMove && currentMove <= range.maxMove)
            {
                float total = 0;
                foreach (var p in range.probabilities) total += p.chance;

                float roll = Random.Range(0f, total);
                float cumulative = 0;

                foreach (var p in range.probabilities)
                {
                    cumulative += p.chance;
                    if (roll <= cumulative)
                        return p.blockCount;
                }
            }
        }
        return 1;
    }
} 

[System.Serializable]
public class MoveRangeProbability
{
    public int minMove;
    public int maxMove;
    public Probability[] probabilities;
}

[System.Serializable]
public class Probability
{
    public int blockCount;
    public float chance;
}