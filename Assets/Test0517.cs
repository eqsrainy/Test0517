using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Test0517 : MonoBehaviour
{
    public int row, col;
    public int startX, startY;
    public int targetX, targetY;
    public Button btn;
    public Button btn2;
    public GameObject character;

    private GameObject[] tilePrefabs;
    private GameObject[,] tiles;
    private GameObject motherNode;
    private GameObject startNode;
    private GameObject targetNode;
    private List<GameObject> listTile;
    private List<GameObject> openList;
    private List<GameObject> closeList;
    private List<GameObject> motherNodes;
    private List<GameObject> minList;
    private float moveCost;

    private Vector2[,] coordinate;

    void Start()
    {
        this.minList = new List<GameObject>();
        this.motherNodes = new List<GameObject>();
        this.openList = new List<GameObject>();
        this.closeList = new List<GameObject>();
        this.tilePrefabs = Resources.LoadAll<GameObject>("Tiles");
        this.coordinate = new Vector2[this.row, this.col];
        this.tiles = new GameObject[this.row, this.col];
        this.listTile = new List<GameObject>();

        this.SetCorrdinate();
        this.CreateTiles();
        this.SetMotherNode(this.tiles[startX, startY]);
        this.SetOpenList();
        this.OrderByList();
        this.character.transform.position = this.motherNode.transform.position;


        this.btn.onClick.AddListener(() =>
        {
            if (closeList.LastOrDefault() == this.targetNode)
            {
                Debug.Log("목표 위치에 도달했습니다.");

                foreach (var a in this.motherNodes)
                {
                    Debug.LogFormat("<color=red>{0}</color>", a.GetComponent<TestTile>().vec2);
                }
            }
            else
            {
                this.SetOpenList();
                this.OrderByList();
                this.closeList.Add(this.openList.FirstOrDefault());
                this.ResetOpenList();
                if (this.CheckWall(this.openList.FirstOrDefault()))
                {
                    this.SetMotherNode(openList.FirstOrDefault());
                }
                else
                {
                    this.SetMotherNode(openList[1]);
                }
                this.openList.Clear();
                this.MoveCharacter();
            }
        });

        this.btn2.onClick.AddListener(() =>
        {
            if (closeList.LastOrDefault() == this.targetNode)
            {
                Debug.Log("목표 위치에 도달했습니다.");

                foreach (var a in this.motherNodes)
                {
                    Debug.LogFormat("<color=red>{0}</color>", a.GetComponent<TestTile>().vec2);
                }
            }
            else
            {
                bool tf = false;
                foreach (var nodes in this.minList)
                {
                    var list = this.minList.OrderBy(x => x.GetComponent<TestTile>().f);
                    if (nodes.GetComponent<TestTile>().vec2 == this.targetNode.GetComponent<TestTile>().vec2 && nodes.GetComponent<TestTile>().f == list.FirstOrDefault().GetComponent<TestTile>().f)
                    {
                        Debug.Log("if문에 들어왓음");
                        this.SetMotherNode(this.targetNode);
                        this.motherNode = this.targetNode;
                        this.motherNodes.Add(this.targetNode);
                        tf = true;
                        break;
                    }
                    tf = false;
                }
                if(tf == false)
                {
                    var orderList = this.minList.OrderBy(x => x.GetComponent<TestTile>().f).ToList();
                    this.closeList.Add(orderList.FirstOrDefault());
                    this.ResetOpenList();
                    this.SetMotherNode(orderList.FirstOrDefault());
                    this.openList.Clear();
                    this.minList.Clear();
                    this.MoveCharacter();

                    this.SetOpenList();
                    this.OrderByList();
                    foreach (var item in this.minList)
                    {
                        Debug.LogFormat("<color=blue>minList item : {0} </color>", item.GetComponent<TestTile>().vec2);
                    }
                }

                //this.openList = new List<GameObject>();
                //this.minList = new List<GameObject>();


                
            }
        });
    }

    private bool CheckWall(GameObject node)
    {
        var checkNode = node;
        var x = checkNode.GetComponent<TestTile>().x;
        var y = checkNode.GetComponent<TestTile>().y;

        Debug.LogFormat("checkNode : {0}.{1}", x, y);

        if(this.motherNode.GetComponent<TestTile>().x < x)
        {
            //타겟의 x-1 / y-1 검사
            var targetNode1 = this.tiles[x - 1, y];
            var targetNode2 = this.tiles[x, y - 1];
            if(targetNode1.GetComponent<TestTile>().tileType == 1 || targetNode2.GetComponent<TestTile>().tileType == 1)
            {
                return false;
            }
        }
        else if(this.motherNode.GetComponent<TestTile>().x > x)
        { //4,3 -> 3,4  3,3 4,4
            //타겟의 x+1 / y+1 검사
            var targetNode1 = this.tiles[x + 1, y];
            var targetNode2 = this.tiles[x, y + 1];
            var targetNode3 = this.tiles[x, y - 1];
            if(targetNode1.GetComponent<TestTile>().tileType == 1 || targetNode2.GetComponent<TestTile>().tileType == 1 || targetNode3.GetComponent<TestTile>().tileType == 1)
            {
                return false;
            }
        }
        return true;
    }

    private void MoveCharacter()
    {
        var anim = this.character.GetComponentInChildren<Animation>();
        anim.Play("run@loop");
        StartCoroutine(this.MoveCharacterImpl());
    }

    private IEnumerator MoveCharacterImpl()
    {
        var anim = this.character.GetComponentInChildren<Animation>();
        var dir = (this.motherNode.transform.position - this.character.transform.position).normalized;
        
        while(true)
        {
            var dis = Vector3.Distance(this.character.transform.position, this.motherNode.transform.position);
            this.character.transform.position += (dir * 0.1f);
            yield return null;
            if(dis <= 0.1f)
            {
                this.character.transform.position = this.motherNode.transform.position;
                anim.Play("idle@loop");
                break;
            }
        }
    }

    private bool CheckValueToOpenList()
    {
        var returnNode = this.motherNodes[this.motherNodes.Count - 2];
        var nextNode = this.openList.FirstOrDefault();

        var returnMotherDistance = Vector3.Distance(returnNode.transform.position, nextNode.transform.position);
        var returnTargetDistance = Vector3.Distance(returnNode.transform.position, this.targetNode.transform.position);
        var returnDistance = returnMotherDistance + returnTargetDistance;

        var resultCost = Vector3.Distance(returnNode.transform.position, this.motherNode.transform.position) + this.moveCost;

        if (returnMotherDistance < resultCost)
        {
            var sprite = this.motherNode.GetComponentInChildren<SpriteRenderer>();
            sprite.color = Color.white;
            this.SetMotherNode(returnNode);
            return true;
        }
        return false;
    }

    #region OrderBy

    private void SaveMoveCost()
    {
        this.moveCost = this.openList.FirstOrDefault().GetComponent<TestTile>().f;
    }

    private void OrderByList()
    {
        var list = this.openList.OrderBy(x => x.GetComponent<TestTile>().f).ToList();
        this.openList = list;
        this.OrderByMin();
    }

    private void OrderByMin()
    {
        var list = this.openList.OrderBy(x => x.GetComponent<TestTile>().g).ToList();
        int index = 0;
        foreach (var tile in list)
        {
            Debug.LogFormat("{0} vs {1}", tile.GetComponent<TestTile>().g, list.FirstOrDefault().GetComponent<TestTile>().g);
            if (tile.GetComponent<TestTile>().g == list.FirstOrDefault().GetComponent<TestTile>().g && tile != this.motherNode)
            {
                this.minList.Add(list[index]);
            }
            index++;
        }
    }

    #endregion


    private void ResetOpenList()
    {
        foreach (var tile in this.openList)
        {
            var sprite = tile.GetComponentInChildren<SpriteRenderer>();
            sprite.color = Color.white;
        }
        this.SetFalseArrow();
        this.SetFalseValue();
    }


    private void SetOpenList()
    {
        var motherX = this.motherNode.GetComponent<TestTile>().x - 1;
        var motherY = this.motherNode.GetComponent<TestTile>().y - 1;
        var maxX = this.motherNode.GetComponent<TestTile>().x + 1;
        var maxY = this.motherNode.GetComponent<TestTile>().y + 1;
        if (motherX < 0)
        {
            motherX = 0;
        }
        if (motherY < 0)
        {
            motherY = 0;
        }

        for (int i = motherX; i <= maxX; i++)
        {
            for (int j = motherY; j <= maxY; j++)
            {
                if (this.tiles[i, j].GetComponent<TestTile>().tileType == 0)
                {
                    this.openList.Add(this.tiles[i, j]);
                    this.SetG(this.tiles[i, j]);
                    this.SetH(this.tiles[i, j]);
                    this.SetF(this.tiles[i, j]);
                    var sprite = this.tiles[i, j].GetComponentInChildren<SpriteRenderer>();
                    sprite.color = Color.cyan;
                }
            }
        }
        this.TurnArrow();
    }

    private void SetMotherNode(GameObject mother)
    {
        this.motherNode = mother;
        this.motherNodes.Add(this.motherNode);
        this.motherNode.GetComponent<TestTile>().tileType = 2;
        Debug.LogFormat("Mother node : {0}", this.motherNode.GetComponent<TestTile>().vec2);
        var sprite = this.motherNode.GetComponentInChildren<SpriteRenderer>();
        sprite.color = Color.gray;
    }


    #region Arrow, FGH Value, Coordinate
    private void TurnArrow()
    {
        foreach (var tile in this.openList)
        {
            var script = tile.GetComponent<TestTile>();
            var xPos = this.motherNode.transform.position.x - script.Arrow.transform.position.x;
            var yPos = this.motherNode.transform.position.y - script.Arrow.transform.position.y;
            var rad = Mathf.Atan2(yPos, xPos) * Mathf.Rad2Deg;
            script.Arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rad + 270));

            script.Arrow.SetActive(true);
        }
    }

    private void SetFalseArrow()
    {
        foreach (var tile in this.openList)
        {
            var script = tile.GetComponent<TestTile>();
            script.Arrow.SetActive(false);
        }
    }

    private void SetFalseValue()
    {
        foreach (var tile in this.openList)
        {
            var script = tile.GetComponent<TestTile>();
            script.textF.text = "F";
            script.textG.text = "G";
            script.textH.text = "H";
        }
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
        tileScript.textF.text = tileScript.f.ToString("f0");
    }

    private void SetG(GameObject tile)
    {
        var tileScript = tile.GetComponent<TestTile>();
        var targetScript = this.targetNode.GetComponent<TestTile>();
        var dis = (Vector2.Distance(tile.transform.position, this.motherNode.transform.position)) * 10;
        tileScript.textG.text = dis.ToString("f0");
        tileScript.g = dis;
    }

    private void SetH(GameObject tile)
    {
        var tileScript = tile.GetComponent<TestTile>();
        var targetScript = this.targetNode.GetComponent<TestTile>();
        var dis = (Vector2.Distance(tile.transform.position, this.targetNode.transform.position)) * 10;
        tileScript.textH.text = dis.ToString("f0");
        tileScript.h = dis;
    }
    #endregion

    #region CreateTiles
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
                    script.vec2 = this.coordinate[i, j];
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
                    script.vec2 = this.coordinate[i, j];
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
                    script.vec2 = this.coordinate[i, j];
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
                    script.vec2 = this.coordinate[i, j];
                    script.x = i;
                    script.y = j;
                    script.textCoord.text = script.x.ToString() + ", " + script.y.ToString();
                    tile.transform.position = screenPos;
                }
            }
        }
    }
    #endregion
}
