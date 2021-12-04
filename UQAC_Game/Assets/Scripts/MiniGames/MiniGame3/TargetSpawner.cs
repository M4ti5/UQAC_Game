using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public int nbOfTarget = 1;

    public List<GameObject> allTargets;
    public GameObject targetPrefab;

    private bool miniGameEnded = false;
    public GameObject victoryText;
    
    private List<Vector3> allPositions;

    private List<bool> stateOfPositions;
    // Start is called before the first frame update
    void Start()
    {
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
            tmpObj.transform.position = Vector3.zero;
            tmpObj.transform.SetParent(transform, false);
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
