using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlobalScore : MonoBehaviour
{
    public ScoreProgressBar scoreProgressBar;
    public float stepIncrease = 30;
    public float stepDecrease = 20;

    [Tooltip("Score ReadOnly")]
    public float score;

    // Start is called before the first frame update
    void Start()
    {
        score = GetScore();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            scoreProgressBar.IncreaseScore(stepIncrease);
            score = GetScore();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            scoreProgressBar.DecreaseScore(stepDecrease);
            score = GetScore();
        }
    }

    public float GetScore()
    {
        return scoreProgressBar.GetScore();
    }
}
