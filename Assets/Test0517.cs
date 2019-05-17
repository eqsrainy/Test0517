using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test0517 : MonoBehaviour
{
    public int row, col;
    public int startX, startY;
    public int targetX, targetY;

    private GameObject[] tilePrefabs;
    private GameObject[,] tiles;
    private GameObject motherNode;
    private GameObject startNode;
    private GameObject targetNode;
    private List<GameObject> listTile;
    private List<GameObject> openList;
    private List<GameObject> closeList;
    private List<GameObject> motherNodes;

    private Vector2[,] coordinate;
    
    void Start()
    {
        this.tilePrefabs = Resources.LoadAll<GameObject>("Tiles");
        this.coordinate = new Vector2[this.row, this.col];
        this.tiles = new GameObject[this.row, this.col];
        this.listTile = new List<GameObject>();

        this.SetCorrdinate();
        this.CreateTiles();
        

        this.SetMotherNode(this.tiles[startX, startY]);
        this.FindPath(this.tiles[2, 5]);

    }

    private void FindPath(GameObject target)
    {
        this.FindNodes();
    }

    private void FindNodes()
    {
        var mother = this.motherNode.GetComponent<TestTile>();
        var motherX = this.motherNode.GetComponent<TestTile>().x - 1;
        var motherY = this.motherNode.GetComponent<TestTile>().y - 1;
        var maxX = this.motherNode.GetComponent<TestTile>().x + 1;
        var maxY = this.motherNode.GetComponent<TestTile>().y + 1;
        if (motherX < 0)
        {
            motherX = 0;
        }
        if(motherY < 0)
        {
            motherY = 0;
        }

        for (int i = motherX; i <= maxX; i++)
        {
            for (int j = motherY; j <= maxY; j++)
            {
                if (this.tiles[i, j].GetComponent<TestTile>().tileType == 0)
                {
                    this.listTile.Add(this.tiles[i, j]);
                    this.SetG(this.tiles[i, j]);
                    this.SetH(this.tiles[i, j]);
                    this.SetF(this.tiles[i, j]);
                    this.TurnArrow(this.tiles[i,j]);
                    var sprite = this.tiles[i, j].GetComponentInChildren<SpriteRenderer>();
                    sprite.color = Color.cyan;
                }
            }
        }
    }

    private void TurnArrow(GameObject tile)
    {
        var script = tile.GetComponent<TestTile>();
        var xPos = this.startNode.transform.position.x - script.Arrow.transform.position.x;
        var yPos = this.startNode.transform.position.y - script.Arrow.transform.position.y;
        var rad = Mathf.Atan2(yPos, xPos) * Mathf.Rad2Deg;
        script.Arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rad+270));

        script.Arrow.SetActive(true);
    }

    private void SetMotherNode(GameObject mother)
    {
        this.motherNode = mother;
        this.motherNode.GetComponent<TestTile>().tileType = 2;
    }

    private void SetCorrdinate()
    {
        for (int i = 0; i < this.row; i++)
        {
            for (int j = 0; j < this.col; j++)
            {
                this.coordinate[i, j] = new Vector2(i, j);
            }
        }
    }

    private void SetF(GameObject tile)
    {
        var tileScript = tile.GetComponent<TestTile>();
        tileScript.f = tileScript.g + tileScript.h;
        tileScript.textF.text = tileScript.f.ToString();
    }
    private void SetG(GameObject tile)
    {
        var tileScript = tile.GetComponent<TestTile>();
        var targetScript = this.targetNode.GetComponent<TestTile>();
        var dis = (int)(Vector2.Distance(tile.transform.position, this.targetNode.transform.position)) * 10;
        Debug.Log(dis);
        tileScript.textH.text = dis.ToString();
        tileScript.h = dis;
    }
    private void SetH(GameObject tile)
    {
        var tileScript = tile.GetComponent<TestTile>();
        var targetScript = this.targetNode.GetComponent<TestTile>();
        var dis = (int)(Vector2.Distance(tile.transform.position, this.startNode.transform.position)) * 10;
        Debug.Log(dis);
        tileScript.textG.text = dis.ToString();
        tileScript.g = dis;
    }


    private void CreateTiles()
    {

        for (int i = 0; i < this.row; i++)
        {
            for (int j = 0; j < this.col; j++)
            {
                var screenPos = Camera.main.ScreenToWorldPoint(new Vector2((j * 100) + 400, (i * -100) + 550));
                screenPos.z = 0;
                if (i == this.startX && j == this.startY)
                {
                    var tile = GameObject.Instantiate(this.tilePrefabs[1]);
                    this.tiles[i, j] = tile;
                    this.startNode = tile;
                    var script = tile.GetComponent<TestTile>();
                    script.x = i;
                    script.y = j;
                    script.textCoord.text = script.x.ToString() + ", " + script.y.ToString();
                    tile.transform.position = screenPos;
                }
                else if (i == this.targetX && j == this.targetY)
                {
                    var tile = GameObject.Instantiate(this.tilePrefabs[2]);
                    this.tiles[i, j] = tile;
                    this.targetNode = tile;
                    var script = tile.GetComponent<TestTile>();
                    script.x = i;
                    script.y = j;
                    script.textCoord.text = script.x.ToString() + ", " + script.y.ToString();
                    tile.transform.position = screenPos;
                }
                else if ((i >= 1 && i < 4) && j == 3)
                {
                    var tile = GameObject.Instantiate(this.tilePrefabs[0]);
                    this.tiles[i, j] = tile;
                    var script = tile.GetComponent<TestTile>();
                    script.tileType = 1;
                    script.x = i;
                    script.y = j;
                    script.textCoord.text = script.x.ToString() + ", " + script.y.ToString();
                    tile.transform.position = screenPos;
                }
                else
                {
                    var tile = GameObject.Instantiate(this.tilePrefabs[3]);
                    this.tiles[i, j] = tile;
                    var script = tile.GetComponent<TestTile>();
                    script.x = i;
                    script.y = j;
                    script.textCoord.text = script.x.ToString() + ", " + script.y.ToString();
                    tile.transform.position = screenPos;
                }
            }
        }
    }
}
