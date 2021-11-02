using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PersonalScore : MonoBehaviour
{
    public Image personalScore;
    public GameObject mask;
    public TextMeshProUGUI personalText;
    private int personalScoreMax;
    private int currentPersonalScore;
    public bool criminel;


    // Start is called before the first frame update
    void Start()
    {
        personalScoreMax = 100;
        currentPersonalScore = 0;
        if (criminel)
        {
            personalScore.gameObject.SetActive(false);
            personalText.gameObject.SetActive(false);
            mask.gameObject.SetActive(false);
        }
        else
        {
            personalScore.gameObject.SetActive(true);
            personalText.gameObject.SetActive(true);
            mask.gameObject.SetActive(true);
            ModifyDisplay();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!criminel)
        {
            personalScore.gameObject.SetActive(true);
            personalText.gameObject.SetActive(true);
            mask.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.J))
            {
                IncreaseGlobalScore(20);
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                DecreaseGlobalScore(10);
            }
        }
        else
        {
            personalScore.gameObject.SetActive(false);
            personalText.gameObject.SetActive(false);
            mask.gameObject.SetActive(false);
        }
    }

    private void IncreaseGlobalScore(int score)
    {
        currentPersonalScore += score;
        if (currentPersonalScore >= personalScoreMax)
        {
            currentPersonalScore = personalScoreMax;
        }
        ModifyDisplay();
    }

    private void DecreaseGlobalScore(int score)
    {
        currentPersonalScore -= score;
        if (currentPersonalScore < 0)
        {
            currentPersonalScore = 0;
        }
        ModifyDisplay();
    }

    private void ModifyDisplay()
    {
        //modifie l'avancement de la barre de vie ainsi que le texte correspondant
        personalScore.transform.position = mask.transform.position + new Vector3(currentPersonalScore * 300 / personalScoreMax - 300, 0, 0);

    }
}
