using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlobalScore : MonoBehaviour
{
    public Image[] personalScore;
    public Image globalScore;
    public GameObject mask;
    public TextMeshProUGUI globalScoreText;
    private int globalScoreMax;
    private int currentGlobalScore;
    private int investigatorNumber;


    // Start is called before the first frame update
    void Start()
    {
        investigatorNumber = personalScore.Length;
        foreach (Image i in personalScore)
        {
            currentGlobalScore += i.GetComponent<PersonalScore>().GetPersonalScore();
        }
        globalScoreMax = investigatorNumber * 100;

        StartCoroutine(GlobalScoreUpdate(0.5f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    IEnumerator GlobalScoreUpdate(float timeUpdate)
    {
        yield return new WaitForSeconds(timeUpdate);
        //currentGlobalScore = 0;
        Debug.Log("test");
        //foreach (Image i in personalScore)
        //{
        //    currentGlobalScore += i.GetComponent<PersonalScore>().GetPersonalScore();
        //    //ModifyDisplay();
        //}
    }

    private void ModifyDisplay()
    {
        //modifie l'avancement de la barre de vie ainsi que le texte correspondant
        globalScore.transform.position = mask.transform.position + new Vector3(currentGlobalScore * 300 / globalScoreMax - 300, 0, 0);

    }
}
