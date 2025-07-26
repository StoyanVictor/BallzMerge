using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TextMeshPro numberText;
    
    public string Id;
    
    public void Init(string id,Color color)
    {
        Id = id;
        spriteRenderer.color = color;
        numberText.text = Id;
    }
}