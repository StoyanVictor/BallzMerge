using System;
using UnityEngine;
public class Test : MonoBehaviour
{
    public GameObject blockPrefab;
       public Vector2 originPosition = Vector2.zero;
       public float cellSizeY = 1f;
       public float cellSizeX = 1f;
       private GameObject objecta;
       public GameObject SpawnBlock(float x, float y)
       {
           Vector3 spawnPosition = new Vector3(originPosition.x + x * cellSizeX, originPosition.y + y * cellSizeY, 0);
            objecta = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
            return objecta;
       }
   
       // Для примера — создаем один блок в позиции (2, 3)
       private void Start()
       {
           MoveBlockInCell(SpawnBlock(1,1));
       }
       private void MoveBlockInCell(GameObject block)
       {
           block.transform.Translate(new Vector3(0,cellSizeY,0));
       }
       private void MoveBlockInCellBack(GameObject block)
       {
           block.transform.Translate(new Vector3(0,-cellSizeY,0));
       }
       private void MoveBlockInCellLeft(GameObject block)
       {
           block.transform.Translate(new Vector3(-cellSizeX,0,0));
       }
       private void MoveBlockInCellRight(GameObject block)
       {
           block.transform.Translate(new Vector3(cellSizeX,0,0));
       }
       private void Update()
       {
           if (Input.GetKeyDown(KeyCode.W))
           {
               MoveBlockInCell(objecta);
           }
           if (Input.GetKeyDown(KeyCode.S))
           {
               MoveBlockInCellBack(objecta);
           }
           if (Input.GetKeyDown(KeyCode.A))
           {
               MoveBlockInCellLeft(objecta);
           }
           if (Input.GetKeyDown(KeyCode.D))
           {
               MoveBlockInCellRight(objecta);
           }
       }
}
