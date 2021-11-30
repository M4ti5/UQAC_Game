using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(PhotonView))]
public class GlobalScore : MonoBehaviourPun
{
    public ScoreProgressBar scoreProgressBar;
    public float stepIncrease = 30;
    public float stepDecrease = 20;

    public bool isLeaked = false;


    [Tooltip("Score ReadOnly")]
    public float score;

    public CriminalLeak criminalLeak;


    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            score = GetScore();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            scoreProgressBar.IncreaseScore(stepIncrease);
            score = GetScore();
            // synchronise
            OnChangeScore(score);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            scoreProgressBar.DecreaseScore(stepDecrease);
            score = GetScore();
            // synchronise
            OnChangeScore(score);
        }

    }

    public void IncreaseScore()
    {
        
        scoreProgressBar.IncreaseScore(stepIncrease);
        score = GetScore();
        // synchronise
        OnChangeScore(score);
    }

    public void DecreaseScore()
    {
        scoreProgressBar.DecreaseScore(stepDecrease);
        score = GetScore();
        // synchronise
        OnChangeScore(score);
    }

    public void OnChangeScore(float score)
    {
        photonView.RPC(nameof(SetScore), RpcTarget.AllBufferedViaServer, score, PhotonNetwork.LocalPlayer);

    }

    public float GetScore()
    {
        return scoreProgressBar.GetScore();
    }

    [PunRPC]
    public void SetScore(float score, Photon.Realtime.Player _player = null)
    {
        SetScore((int)score, _player);
    }

    [PunRPC]
    public void SetScore(int score, Photon.Realtime.Player _player = null)
    {
        if (_player != null)
        {
            photonView.TransferOwnership(_player);
        }
        this.scoreProgressBar.SetScore(score);
        this.score = GetScore();


        //Criminal Leak
        if (score == scoreProgressBar.scoreMax) {
            if (!isLeaked) {
                criminalLeak.ShowCriminalLeak();
                isLeaked = true;
            }
        }
    }
}
