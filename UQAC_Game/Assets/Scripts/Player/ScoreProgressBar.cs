using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreProgressBar : MonoBehaviour
{
    public bool enable = true;
    public float score;
    public float scoreMax;

    public RectTransform globalScore;
    private float maxSize;

    // Start is called before the first frame update
    void Start()
    {
        if (enable)
        {
            maxSize = globalScore.rect.width;
            ModifyDisplay();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    public void IncreaseScore(int score)
    {
        IncreaseScore((float)score);
    }

    public void IncreaseScore(float score)
    {
        if (enable)
        {
            this.score += score;
            if (this.score >= scoreMax)
            {
                this.score = scoreMax;
            }
            ModifyDisplay();
        }
    }

    public void DecreaseScore(int score)
    {
        DecreaseScore((float)score);
    }

    public void DecreaseScore(float score)
    {
        if (enable)
        {
            this.score -= score;
            if (this.score < 0)
            {
                this.score = 0;
            }
            ModifyDisplay();
        }
    }

    private void ModifyDisplay()
    {
        if (enable)
        {
            //modifie l'avancement de la barre de vie
            globalScore.transform.position = globalScore.parent.position + new Vector3(this.score * maxSize / scoreMax - maxSize*1.5f, 0, 0);
        }
    }

    public void SetEnable(bool enable)
    {
        this.enable = enable;
    }
    public bool GetEnable()
    {
        return this.enable;
    }
    public float GetScore()
    {
        return this.score;
    }
}
