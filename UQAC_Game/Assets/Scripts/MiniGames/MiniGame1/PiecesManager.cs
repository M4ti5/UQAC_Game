using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manage creation of piece for a puzzle game
/// </summary>
public class PiecesManager : MonoBehaviour
{
    /// <summary>
    /// Image of the puzzle
    /// </summary>
    public Texture2D img;

    /// <summary>
    /// Number of horizontal divisions
    /// </summary>
    public int nbrLines;
    /// <summary>
    /// Number of vertical divisions
    /// </summary>
    public int nbrColumns;

    /// <summary>
    /// Prefab of one puzzle piece
    /// </summary>
    public GameObject piecePrefab;

    //public Sprite[] sprites;
    private List<Sprite> allSprites;
    private List<GameObject> allPieces;

    /// <summary>
    /// Zone to pop at start
    /// </summary>
    public GameObject tempZone;
    /// <summary>
    /// Global canvas of this mini game (used for all pieces)
    /// </summary>
    public Canvas canvas;
    /// <summary>
    /// Minimum distance to test if we put a piece at the right position (used for all pieces)
    /// </summary>
    public float precision = 20f;

    /// <summary>
    /// GameObject of image of the puzzle
    /// </summary>
    public GameObject bgImgObj;

    /// <summary>
    /// Bool to know if puzzle is completed
    /// </summary>
    public bool puzzleEnd;
    /// <summary>
    /// Object where is write a congratulation message 
    /// </summary>
    public GameObject successObj;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start PiecesManager script");
        // Set puzzle image
        //sprites = Resources.LoadAll<Sprite>("Materials/" + img.name);
        bgImgObj.GetComponent<RawImage>().texture = img;
        // Set ratio
        bgImgObj.GetComponent<AspectRatioFitter>().aspectRatio = (float)img.width / img.height;
        // Divise puzzle
        CreateSprites();
        // Create pieces
        InitAllPieces();
    }

    // Update is called once per frame
    void Update()
    {

        CheckEndOfGame();
        EndPuzzle();
    }

    /// <summary>
    /// Divise puzzle image with specified numbers of rows and columns
    /// </summary>
    void CreateSprites()
    {
        // inspiration: 
        // https://stackoverflow.com/questions/55738954/how-to-slice-sprite-by-script-not-use-editor
        // https://gamedev.stackexchange.com/questions/157310/how-to-access-sprite-sheet-via-row-and-column
        float xOffset = (float)img.width / nbrColumns;
        float yOffset = (float)img.height / nbrLines;

       // nbrLines = (int)(img.height / yOffset);
       // nbrColumns = (int)(img.width / xOffset);

        for (float y = 0; y <= img.height; y += yOffset)
        {
            for (float x = 0; x <= img.width; x += xOffset)
            {
                if (x + xOffset/2f <= img.width && y + yOffset/2f <= img.height) // Check if we are not out of image 
                {
                    // Create a sprite of a custom position and size
                    var rect = new Rect(x, y, xOffset, yOffset);
                    Sprite sprite = Sprite.Create(img, rect, new Vector2(0.5f, 0.5f));
                    // Add one piece image to the list
                    allSprites.Add(sprite);
                }
            }
        }
    }

    /// <summary>
    /// Create and add all pieces to the scene
    /// </summary>
    void InitAllPieces()
    {
        // Get background image sizes
        Vector2 bgImgSize = new Vector2(bgImgObj.GetComponent<RectTransform>().rect.width, bgImgObj.GetComponent<RectTransform>().rect.height);
        int i = 0; 
        for (float y = 0; y < nbrLines; y++)
        {
            for (float x = 0; x < nbrColumns; x ++)
            {
                // Duplicate prefab
                GameObject tmpObj = Instantiate(piecePrefab);
                // Attribute the good sprite
                tmpObj.GetComponent<Image>().sprite = allSprites[i];//sprites[i,2];//https://gamedev.stackexchange.com/questions/157310/how-to-access-sprite-sheet-via-row-and-column
                // Set parent
                tmpObj.transform.SetParent(transform, false);
                // tmpObj.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
                // Reset local scale
                tmpObj.transform.localScale = new Vector3(1, 1, 1);
                // Reset local rotation
                tmpObj.transform.localRotation = Quaternion.identity;
                // Set local position of the piece (to know it's right location)
                tmpObj.GetComponent<RectTransform>().localPosition = new Vector3(
                    (x * (float)bgImgSize.x / nbrColumns - bgImgSize.x / 2f + (float)bgImgSize.x / nbrColumns / 2f),// num col  * piece size - (piece size/2 [car c'est centré au centre de la piece])
                    (y * (float)bgImgSize.y / nbrLines - bgImgSize.y / 2f + (float)bgImgSize.y / nbrLines / 2f),// num line * piece size - (piece size/2 [car c'est centré au centre de la piece])
                    0);
                // Set size of the piece with the right ratio
                tmpObj.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    (float)bgImgSize.x / nbrColumns,
                    (float)bgImgSize.y / nbrLines
                    );
                // Set name
                tmpObj.transform.name = "Piece (" + i + ")";
                // Set public variables need for one piece
                tmpObj.GetComponent<Piece>().tempZone = tempZone;
                tmpObj.GetComponent<Piece>().canvas = canvas;
                tmpObj.GetComponent<Piece>().precision = precision;

                // Add piece to the list
                allPieces.Add(tmpObj);

                i++;
            }
        }
    }

    /// <summary>
    /// Check if all pieces are in the right place
    /// </summary>
    void CheckEndOfGame()
    {
        puzzleEnd = true;
        foreach (GameObject piece in allPieces)
        {
            if (piece.GetComponent<Piece>().inRightPosition == false) // if one is not, then false and stop to look at others
            {
                puzzleEnd = false;
                break;
            }
        }
    }

    /// <summary>
    /// Show or hide success message if all pieces are in the right place<br/>
    /// It could be modified to call end animation function 
    /// </summary>
    void EndPuzzle()
    {
        successObj.SetActive(puzzleEnd);
    }
}