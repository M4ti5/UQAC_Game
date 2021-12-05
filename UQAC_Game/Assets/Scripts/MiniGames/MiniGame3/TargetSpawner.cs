using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TargetSpawner : MonoBehaviour
{
    public int nbOfTarget = 1;

    public GameObject targetPrefab;
    public GameObject victoryText;
    private float maxWidth;
    private float maxHeight;

    private float sizeOfTargetSprite = 110;
    
    public List<GameObject> allTargets;

    private bool miniGameEnded = false;
    

    // Start is called before the first frame update
    void Start()
    {
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

    void initAllTargets()
    {
        for (int i = 0; i < nbOfTarget; i++)
        {
            GameObject tmpObj = Instantiate(targetPrefab);
            tmpObj.transform.SetParent(transform, false);
            float x = Random.Range(-maxWidth/2+sizeOfTargetSprite/2, maxWidth/2-sizeOfTargetSprite/2);
            float y = Random.Range(-maxHeight/2+sizeOfTargetSprite/2, maxHeight/2-sizeOfTargetSprite/2);
            tmpObj.transform.localPosition = new Vector3(x, y, 1);
            tmpObj.transform.localScale = new Vector3(1, 1, 1);
            tmpObj.transform.localRotation = Quaternion.identity;
            //tmpObj.SetActive(false);
            allTargets.Add(tmpObj);
        }
        
    }
    
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
    
    IEnumerator EndMiniGame()
    {
        victoryText.SetActive(true);
        yield return new WaitForSeconds(1);
        gameObject.transform.parent.parent.gameObject.SetActive(false);
    }  
    
    
}
