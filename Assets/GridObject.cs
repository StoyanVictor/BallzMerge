using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TextMeshPro numberText;
    [SerializeField] private GameBlockDataBase blockDataBase;
    public string Id;
    
    private void Start()
    {
        Init();
    }
    private void Init()
    {
        var randomIndex = Random.Range(0, blockDataBase.gameBlocks.Count);
        Id = blockDataBase.gameBlocks[randomIndex].Id;
        spriteRenderer.color = blockDataBase.gameBlocks[randomIndex].Color;
        numberText.text = Id;
    }
}