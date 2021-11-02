using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlobalScore : MonoBehaviour
{
    public Image globalScore;
    public GameObject mask;
    private int globalScoreMax;
    private int currentGlobalScore;


    // Start is called before the first frame update
    void Start()
    {
        globalScoreMax =  500;
        currentGlobalScore = 0;
        ModifyDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            IncreaseGlobalScore(30);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            DecreaseGlobalScore(20);
        }
    }

    private void IncreaseGlobalScore(int score)
    {
        currentGlobalScore += score;
        if (currentGlobalScore >= globalScoreMax)
        {
            currentGlobalScore = globalScoreMax;
        }
        ModifyDisplay();
    }

    private void DecreaseGlobalScore(int score)
    {
        currentGlobalScore -= score;
        if (currentGlobalScore < 0)
        {
            currentGlobalScore = 0;
        }
        ModifyDisplay();
    }

    private void ModifyDisplay()
    {
        //modifie l'avancement de la barre de vie ainsi que le texte correspondant
        globalScore.transform.position = mask.transform.position + new Vector3(currentGlobalScore * 300 / globalScoreMax - 300, 0, 0);

    }
    public void SetGlobalScore(int score)
    {
        currentGlobalScore = score;
        if (currentGlobalScore >= globalScoreMax)
        {
            currentGlobalScore = globalScoreMax;
        }
        ModifyDisplay();
    }

    public int GetGlobalScore()
    {
        return currentGlobalScore;
    }
}
