using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    [SerializeField] private GridInitializer gridInitializer;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private SpawnRulesSO rules;
    [SerializeField] private SpawnProbabilitySO rulesProbability;

    public void SpawnInitialObjects(int currentMove)
    {
        int blocksToSpawn = rulesProbability.GetBlocksToSpawn(currentMove);
        for (int i = 0; i < blocksToSpawn; i++)
        {
            int randomX = Random.Range(0, gridInitializer.GetWidth());
            if (gridInitializer.grid[randomX, 0].IsEmpty)
            {
                SpawnRandomObject(randomX, 0, currentMove);
            }
        }
    }
    private void SpawnRandomObject(int x, int y, int currentMove)
    {
        Block[] allowedBlocks = rules.GetAllowedNumbers(currentMove);
        Block chosenBlock = allowedBlocks[Random.Range(0, allowedBlocks.Length)];

        GameObject obj = Instantiate(blockPrefab, GetCellCenter(x, y), Quaternion.identity);
        obj.GetComponent<GridObject>().Init(chosenBlock.id, chosenBlock.color);

        gridInitializer.grid[x, y].Fill(obj);
    }
    private Vector3 GetCellCenter(int x, int y)
    {
        int invertedY = gridInitializer.GetHeight() - 1 - y;
        return new Vector3(
            transform.position.x + x * gridInitializer.GetCellSize() + gridInitializer.GetCellSize() / 2 + gridInitializer.GetOffset().x,
            transform.position.y + invertedY * gridInitializer.GetCellSize() + gridInitializer.GetCellSize() / 2 + gridInitializer.GetOffset().y,
            0
        );
    }
}