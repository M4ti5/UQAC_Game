using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PersonalScore : MonoBehaviour
{
    public Image personalScore;
    public GameObject mask;
    public TextMeshProUGUI personalScoreText;
    private int personalScoreMax;
    private int currentPersonalScore;


    // Start is called before the first frame update
    void Start()
    {
        personalScoreMax = 100;
        currentPersonalScore = 0;
        ModifyDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        //test des fonctions
        if (Input.GetKeyDown(KeyCode.L))
        {
            TaskAccomplished(10);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            TaskSabotaged(10);
        }
    }

    private void TaskAccomplished(int points)
    {
        currentPersonalScore += points;
        if (currentPersonalScore >= personalScoreMax)
        {
            currentPersonalScore = personalScoreMax;
            Debug.Log("All Task Accomplished");
        }
        ModifyDisplay();
    }


    private void TaskSabotaged(int points)
    {
        currentPersonalScore -= points;
        if (currentPersonalScore <= 0)
        {
            currentPersonalScore = 0;
            Debug.Log("Any Task Done");
        }
        ModifyDisplay();
    }

    public int GetPersonalScore()
    {
        return currentPersonalScore;
    }

    private void ModifyDisplay()
    {
        //modifie l'avancement de la barre de vie ainsi que le texte correspondant
        personalScore.transform.position = mask.transform.position + new Vector3(currentPersonalScore * 300 / personalScoreMax - 300, 0, 0);
        
    }
}
