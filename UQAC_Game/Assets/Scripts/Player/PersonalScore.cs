using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PersonalScore : MonoBehaviour
{
    public ScoreProgressBar scoreProgressBar;
    public float stepIncrease = 20;
    public float stepDecrease = 10;
    public bool criminel = false;

    [Tooltip("Score ReadOnly")]
    public float score;

    // Start is called before the first frame update
    void Start()
    {
        scoreProgressBar.SetEnable(!criminel);
        score = GetScore();
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR // allow cheat code just when we start game with unity
        // DEBUG MODE -- Progress bar
        if (!criminel)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                scoreProgressBar.IncreaseScore(stepIncrease);
                score = GetScore();
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                scoreProgressBar.DecreaseScore(stepDecrease);
                score = GetScore();
            }
        }
#endif
    }

    public float GetScore()
    {
        return scoreProgressBar.GetScore();
    }

    public void IncreaseScore()
    {
        scoreProgressBar.IncreaseScore(stepIncrease);
        score = GetScore();
    }

    public void DecreaseScore()
    {
        scoreProgressBar.DecreaseScore(stepDecrease);
        score = GetScore();
    }
}
