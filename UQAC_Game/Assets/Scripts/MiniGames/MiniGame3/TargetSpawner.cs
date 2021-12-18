using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Game Manager of the target minigame
/// Spawn a number of target in a canvas, when every target is clicked the player wins
/// </summary>

public class TargetSpawner : MonoBehaviour
{
    public int nbOfTarget;

    public GameObject targetPrefab;
    public GameObject victoryText;
    private float maxWidth;
    private float maxHeight;

    //store size of the sprite to offset the display so that it looks better
    private float sizeOfTargetSprite = 110;
    
    public List<GameObject> allTargets;

    public bool miniGameEnded = false;
    

    // Start is called before the first frame update
    void Start()
    {
        victoryText.SetActive(false);
        miniGameEnded = false;
        // Get size of the canvas
        maxWidth = gameObject.GetComponent<RectTransform>().rect.width;
        maxHeight = gameObject.GetComponent<RectTransform>().rect.height;
        initAllTargets();
    }

    // Update is called once per frame
    void Update()
    {
        CheckEndOfGame();
        if (miniGameEnded)
        {
            StartCoroutine(EndMiniGame());
        }
    }

    //Init the game : Instantiate target on the canvas to the max number (nbOfTarget)
    //Add every target in a List (AllTargets)
    void initAllTargets()
    {
        for (int i = 0; i < nbOfTarget; i++)
        {
            //Instantiate prefab
            GameObject tmpObj = Instantiate(targetPrefab);
            tmpObj.transform.SetParent(transform, false);
            //Set Position of the target randomly
            // (x y) = (0 0) is at the center of the canvas so the position goes from
            // (-(maxWith+SizeOfSprite)/2 -(maxHeight+SizeOfSprite)/2) to  (maxWith+SizeOfSprite/2   maxHeight+SizeOfSprite/2)
            float x = Random.Range(-maxWidth/2+sizeOfTargetSprite/2, maxWidth/2-sizeOfTargetSprite/2);
            float y = Random.Range(-maxHeight/2+sizeOfTargetSprite/2, maxHeight/2-sizeOfTargetSprite/2);
            tmpObj.transform.localPosition = new Vector3(x, y, 1);
            tmpObj.transform.localScale = new Vector3(1, 1, 1);
            tmpObj.transform.localRotation = Quaternion.identity;
            allTargets.Add(tmpObj);
        }
        
    }
    //End game when all targets got clicked, if one is not clicked, then continues the game
    void CheckEndOfGame()
    {
        miniGameEnded = true;
        foreach (GameObject target in allTargets)
        {
            if (target.GetComponent<Target>().isClicked == false) // if one is not, then false and stop to look at others
            {
                miniGameEnded = false;
                break;
            }
        }
    }
    
    //Coroutine for the end of game
    IEnumerator EndMiniGame()
    {
        //Display the victory text
        victoryText.SetActive(true);
        //Wait for 1 second
        yield return new WaitForSeconds(1);
        //Close minigame
        gameObject.transform.parent.parent.gameObject.SetActive(false);
    }  
    
    
}
