using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTile : MonoBehaviour
{
    public TextMesh textCoord;
    public TextMesh textF;
    public TextMesh textG;
    public TextMesh textH;
    public GameObject Arrow;
    public int x, y;
    public int tileType;
    public Vector2 vec2;
    public float g, h, f;

    public TestTile()
    {
        this.vec2 = new Vector2(this.x, this.y);
    }
}
