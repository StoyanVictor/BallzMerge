using UnityEngine;

public class GridCell
{
    public int x, y;
    public GameObject content;

    public GridCell(int x, int y)
    {
        this.x = x;
        this.y = y;
        content = null;
    }

    public bool IsEmpty => content == null;

    public void Fill(GameObject obj) => content = obj;
    public void Clear() => content = null;
}