using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    private Box[,] grid;
    public Image horizontalWall;
    public Image verticalWall;
    public Image player;

    public Image horizontalExternalWall;
    public Image verticalExternalWall;
    private Image createdPlayer;

    public GameObject panel;

    private int row;
    private int column;
    private float widthMap;
    private float heightMap;
    private float widthWall;
    private float heightWall;
    private float marginWidthMap;
    private float marginHeightMap;
    private float startX;
    private float startY;

    private float hW;

    // Start is called before the first frame update
    void Start()
    {
        row = 10;
        column = 15;
        RectTransform rt = GetComponent<RectTransform>();
        widthMap = rt.rect.width;
        heightMap = rt.rect.height;
        marginWidthMap = widthMap * 10 / 100;
        marginHeightMap = heightMap * 10 / 100;
        hW = heightMap / 100;
        widthWall = (widthMap - 2*marginWidthMap) / column;
        heightWall = (heightMap - 2*marginHeightMap) / row;
        

        startX = rt.rect.xMin + marginWidthMap;
        startY = rt.rect.yMin;

        Debug.Log(widthMap + "    " + marginWidthMap + "     " + widthWall);
        
        
        horizontalWall.rectTransform.sizeDelta = new Vector2(hW, widthWall+hW);
        verticalWall.rectTransform.sizeDelta = new Vector2(heightWall+hW, hW);

        horizontalExternalWall.rectTransform.sizeDelta = new Vector2(hW, widthWall * column + hW);
        verticalExternalWall.rectTransform.sizeDelta = new Vector2(heightWall * (row - 1) + hW, hW);

        player.rectTransform.sizeDelta = new Vector2(Mathf.Min(widthWall, heightWall) * 0.4f, Mathf.Min(widthWall, heightWall) * 0.4f);

        //BoxCollider[] collider = horizontalWall.gameObject.GetComponents<BoxCollider>();
        //collider[0].size = new Vector2(hW, widthWall + hW);
        //collider = verticalWall.gameObject.GetComponents<BoxCollider2D>();
        //collider[0].size = new Vector2(heightWall + hW, hW);
        //var collider = horizontalExternalWall.gameObject.GetComponents<BoxCollider>();
        //collider[0].size = new Vector2(hW, widthWall * column + hW);
        //collider = verticalExternalWall.gameObject.GetComponents<BoxCollider2D>();
        //collider[0].size = new Vector2(heightWall * (row - 1) + hW, hW);
        BoxCollider[] collider = player.gameObject.GetComponents<BoxCollider>();
        collider[0].size = new Vector2(Mathf.Min(widthWall, heightWall) * 0.4f, Mathf.Min(widthWall, heightWall) * 0.4f);

        CreateMapGrid();
        DisplayMap();
        
    }

    void CreateMapGrid()
    {
        grid = new Box[column, row];
        int n = 1;
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                grid[i, j] = gameObject.AddComponent<Box>();
                if (j != 0)  grid[i, j].up = true;
                if (j != row-1)  grid[i, j].down = true;
                if (i != 0) grid[i, j].left = true;
                if (i != column - 1) grid[i, j].right = true;
                grid[i, j].number = n;
                n++;
                
            }
        }


        int numberMin = 0;
        int numberMax = 0;
        var actions = new List<string>() { "left", "right", "up", "down" };
        while (grid[0, 0].number != grid[column - 1, row - 1].number)
        {
            int x = Random.Range(0, column);
            int y = Random.Range(0, row);
            string a = actions[Random.Range(0, 4)];

            if (a == "left" && grid[x, y].left == true)
            {
                numberMax = Mathf.Max(grid[x, y].number, grid[x-1, y].number);
                numberMin = Mathf.Min(grid[x, y].number, grid[x - 1, y].number);
                grid[x, y].left = false;
                grid[x - 1, y].right = false;
            }
            else if (a == "right" && grid[x, y].right == true)
            {
                numberMax = Mathf.Max(grid[x, y].number, grid[x + 1, y].number);
                numberMin = Mathf.Min(grid[x, y].number, grid[x + 1, y].number);
                grid[x, y].right = false;
                grid[x + 1, y].left = false;
            }
            else if (a == "up" && grid[x, y].up == true)
            {
                numberMax = Mathf.Max(grid[x, y].number, grid[x, y - 1].number);
                numberMin = Mathf.Min(grid[x, y].number, grid[x, y - 1].number);
                grid[x, y].up = false;
                grid[x, y - 1].down = false;
            }
            else if (a == "down" && grid[x, y].down == true)
            {
                numberMax = Mathf.Max(grid[x, y].number, grid[x, y + 1].number);
                numberMin = Mathf.Min(grid[x, y].number, grid[x, y + 1].number);
                grid[x, y].down = false;
                grid[x, y + 1].up= false;
            }
            if (numberMin != 0)
            {
                foreach ( var g in grid)
                {
                    if (g.number == numberMin)
                    {
                        g.number = numberMax;
                    }
                }

            }
        }
    }

    void DisplayMap()
    {
        var createdExternalWall = Instantiate(horizontalExternalWall, new Vector3(widthWall * 0.5f * column + +marginWidthMap, hW / 2 - marginHeightMap, 0), horizontalExternalWall.transform.rotation);
        createdExternalWall.transform.SetParent(panel.transform, false);
        createdExternalWall = Instantiate(horizontalExternalWall, new Vector3(widthWall * 0.5f * column + +marginWidthMap, hW / 2 - marginHeightMap - row * heightWall, 0), horizontalExternalWall.transform.rotation);
        createdExternalWall.transform.SetParent(panel.transform, false);

        createdExternalWall = Instantiate(verticalExternalWall, new Vector3(marginWidthMap, hW / 2 - row * 0.5f * heightWall - 0.5f * heightWall - marginHeightMap, 0), verticalExternalWall.transform.rotation);
        createdExternalWall.transform.SetParent(panel.transform, false);
        createdExternalWall = Instantiate(verticalExternalWall, new Vector3(marginWidthMap + column * widthWall, hW / 2 - row * 0.5f * heightWall + 0.5f * heightWall - marginHeightMap, 0), verticalExternalWall.transform.rotation);
        createdExternalWall.transform.SetParent(panel.transform, false);


        createdPlayer = Instantiate(player, new Vector3(marginWidthMap + widthWall *0.5f, hW / 2 - 0.5f * heightWall - marginHeightMap, 0), player.transform.rotation);
        createdPlayer.transform.SetParent(panel.transform, false);

        


        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                if (grid[i, j].down == true)
                {
                    var createdWall = Instantiate(horizontalWall, new Vector3(widthWall * (0.5f + i) + marginWidthMap, hW / 2 - (j + 1) * heightWall - marginHeightMap, 0), horizontalWall.transform.rotation);
                    createdWall.transform.SetParent(panel.transform, false);
                }
                if (grid[i, j].right == true)
                {
                    var createdWall = Instantiate(verticalWall, new Vector3(widthWall * (1 + i) + marginWidthMap, hW / 2 - (j + 0.5f) * heightWall - marginHeightMap, 0), verticalWall.transform.rotation);
                    createdWall.transform.SetParent(panel.transform, false);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (createdPlayer.IsDestroyed())
        {
            createdPlayer = Instantiate(player, new Vector3(marginWidthMap + widthWall * 0.5f, hW / 2 - 0.5f * heightWall - marginHeightMap, 0), player.transform.rotation);
            createdPlayer.transform.SetParent(panel.transform, false);
        }
    }
}
