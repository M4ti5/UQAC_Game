using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Map : MonoBehaviour
{
    private Box[,] grid;
    public Image horizontalWall;
    public Image verticalWall;
    public Image player;
    public Image arrive;
    public Image entrance;

    public Image horizontalExternalWall;
    public Image verticalExternalWall;

    public GameObject panel;

    private int row;
    private int column;
    private float widthMap;
    private float heightMap;
    private float widthWall;
    private float heightWall;
    private float marginWidthMap;
    private float marginHeightMap;

    private float hW;

    // Start is called before the first frame update
    void Start()
    {
        row = 10;
        column = 15;
        RectTransform rt = GetComponent<RectTransform>();
        //taille de la marge
        marginWidthMap = widthMap * 10 / 100;
        marginHeightMap = heightMap * 10 / 100;

        heightMap = rt.rect.height;
        widthMap = rt.rect.width;

        //longueur d'un mur horizontal
        widthWall = (widthMap - 2 * marginWidthMap) / column;
        //longueur d'un mur vertical
        heightWall = (heightMap - 2 * marginHeightMap) / row;
        //épaisseur d'un mur
        hW = heightMap / 100;

        
        player.gameObject.SetActive(true);
        entrance.gameObject.SetActive(true);
        arrive.gameObject.SetActive(true);
        
        //Dimensionnement de la taille des murs, du joueur et de l'arrivée en fonction de la taille de l'écran de jeu
        horizontalWall.rectTransform.sizeDelta = new Vector2(hW, widthWall+hW);
        verticalWall.rectTransform.sizeDelta = new Vector2(heightWall+hW, hW);

        horizontalExternalWall.rectTransform.sizeDelta = new Vector2(hW, widthWall * column + hW);
        verticalExternalWall.rectTransform.sizeDelta = new Vector2(heightWall * (row - 1) + hW, hW);

        //Le joueur représenté par un cube vert
        player.rectTransform.sizeDelta = new Vector2(Mathf.Min(widthWall, heightWall) * 0.4f, Mathf.Min(widthWall, heightWall) * 0.4f);

        //Une fois que le joueur atteint ce collider, il a fini le mini-jeu
        arrive.rectTransform.sizeDelta = new Vector2(hW, heightWall);

        //Empêche le joueur de sortir par l'entrée
        entrance.rectTransform.sizeDelta = new Vector2(hW, widthWall + hW);

        //Redimensionnement des colliders en fonction de la taille des objets
        BoxCollider[] collider = horizontalWall.gameObject.GetComponents<BoxCollider>();
        collider[0].size = new Vector2(hW, widthWall + hW);
        collider = verticalWall.gameObject.GetComponents<BoxCollider>();
        collider[0].size = new Vector2(heightWall + hW, hW);
        collider = horizontalExternalWall.gameObject.GetComponents<BoxCollider>();
        collider[0].size = new Vector2(hW, widthWall * column + hW);
        collider = verticalExternalWall.gameObject.GetComponents<BoxCollider>();
        collider[0].size = new Vector2(heightWall * (row - 1) + hW, hW);
        collider = player.gameObject.GetComponents<BoxCollider>();
        collider[0].size = new Vector2(Mathf.Min(widthWall, heightWall) * 0.4f, Mathf.Min(widthWall, heightWall) * 0.4f);

        BoxCollider arriveCollider = arrive.gameObject.GetComponent<BoxCollider>();
        arriveCollider.size = new Vector2(hW, heightWall);


        CreateMapGrid();
        DisplayMap();
        
    }

    //Création de la map du labyrinthe
    void CreateMapGrid()
    {
        Debug.Log("createMapGrid");
        grid = new Box[column, row];
        int n = 1;
        //création de la grille de cases du labyrinthe (tous les murs sont actifs)
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

        //On enlève les murs un par un jusqu'à ce qu'il y ait un chemin possible entre la case de départ et celle d'arrivée
        int numberMin = 0;
        int numberMax = 0;
        var actions = new List<string>() { "left", "right", "up", "down" };
        while (grid[0, 0].number != grid[column - 1, row - 1].number)
        {
            int x = Random.Range(0, column);
            int y = Random.Range(0, row);
            if (TestNumberOfWalls(grid[x,y])<=1)
            {
                continue;
            }
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

        //On parcours toutes les cases de la grille et on fait en sorte qu'il n'y en ait aucune d'isolée
        //Toutes les cases sont ainsi atteignables par le joueur
        numberMax = grid[column - 1, row - 1].number;
        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                if (grid[x,y].number != numberMax)
                {
                    if (grid[x, y].left == true && grid[x-1, y].number==numberMax)
                    {
                        numberMin = grid[x, y].number;
                        grid[x, y].left = false;
                        grid[x - 1, y].right = false;
                    }
                    else if (grid[x, y].right == true && grid[x+1, y].number == numberMax)
                    {
                        numberMin = grid[x, y].number;
                        grid[x, y].right = false;
                        grid[x + 1, y].left = false;
                    }
                    else if (grid[x, y].up == true && grid[x, y-1].number == numberMax)
                    {
                        numberMin = grid[x, y].number;
                        grid[x, y].up = false;
                        grid[x, y - 1].down = false;
                    }
                    else if (grid[x, y].down == true && grid[x, y+1].number == numberMax)
                    {
                        numberMin = grid[x, y].number;
                        grid[x, y].down = false;
                        grid[x, y + 1].up = false;
                    }

                    if (numberMin != 0)
                    {
                        foreach (var g in grid)
                        {
                            if (g.number == numberMin)
                            {
                                g.number = numberMax;
                            }
                        }
                    }
                }
            }
        }
    }

    //On vérifie le nombre de murs actifs de la case
    private int TestNumberOfWalls(Box box)
    {
        Debug.Log("TestNumberOfWalls");
        int numberOfWalls = 0;
        if (box.left == true) numberOfWalls++;
        if (box.right == true) numberOfWalls++;
        if (box.up == true) numberOfWalls++;
        if (box.down == true) numberOfWalls++;
        return numberOfWalls;
    }


    //Affichage de la carte
    void DisplayMap()
    {
        //On commence par affiché les murs extérieurs, l'arrivée, l'entrée (invisible) et le joueur
        var createdExternalWall1 = Instantiate(horizontalExternalWall, new Vector3(widthWall * 0.5f * column + +marginWidthMap, hW / 2 - marginHeightMap, 0), Quaternion.Euler(new Vector3(0, 0, 90)));
        createdExternalWall1.transform.SetParent(panel.transform, false);
        var createdExternalWall2 = Instantiate(horizontalExternalWall, new Vector3(widthWall * 0.5f * column + +marginWidthMap, hW / 2 - marginHeightMap - row * heightWall, 0), Quaternion.Euler(new Vector3(0, 0, 90)));
        createdExternalWall2.transform.SetParent(panel.transform, false);

        var createdExternalWall3 = Instantiate(verticalExternalWall, new Vector3(marginWidthMap, hW / 2 - row * 0.5f * heightWall - 0.5f * heightWall - marginHeightMap, 0), Quaternion.Euler(new Vector3(0, 0, 90)));
        createdExternalWall3.transform.SetParent(panel.transform, false);
        var createdExternalWall4 = Instantiate(verticalExternalWall, new Vector3(marginWidthMap + column * widthWall, hW / 2 - row * 0.5f * heightWall + 0.5f * heightWall - marginHeightMap, 0), Quaternion.Euler(new Vector3(0, 0, 90)));
        createdExternalWall4.transform.SetParent(panel.transform, false);

        arrive.transform.Translate(new Vector3(widthWall * column + marginWidthMap, -heightWall * (row - 0.5f), 0));
        entrance.transform.Translate(new Vector3(marginWidthMap, hW / 2 - 0.5f * heightWall - marginHeightMap, 0));
        player.transform.Translate(new Vector3(marginWidthMap + widthWall * 0.5f, hW / 2 - 0.5f * heightWall - marginHeightMap, 0));


        //Affichage des murs intérieurs
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
}
