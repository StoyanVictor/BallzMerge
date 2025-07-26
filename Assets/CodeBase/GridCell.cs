using UnityEngine;

public class GridCell
{
    public int x;
    public int y;
    public GameObject content;
    public bool IsEmpty => content == null;

    public GridCell(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public void Fill(GameObject obj)
    {
        content = obj;
    }
    public void Clear()
    {
        content = null;
    }
}