using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject instanceEndGame;
    
    public List<PlayerStatManager> loosers;
    public List<PlayerStatManager> winners;

    public bool endGame = false;
    public static string menuScreenBuildName = "Launcher"; //the menu screen's index in your Build Settings
    
    private void Awake()
    {
        SceneManager.activeSceneChanged += DestroyOnMenuScreen;
        if (this != null)
        {
            if (instanceEndGame == null)
            {
                instanceEndGame = gameObject;
                DontDestroyOnLoad(gameObject);
                return;
            }

            if (instanceEndGame == this) return;
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (this != null && endGame == false && PhotonNetwork.InRoom)
        {
            new WaitForEndOfFrame();
            if (loosers.Count + winners.Count >= PhotonNetwork.CurrentRoom.PlayerCount)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    print("End");
                    PhotonNetwork.LoadLevel("End");
                    endGame = true;
                }
            }
        }
        
        
    }

    public void AddLooser(PlayerStatManager looser)
    {
        loosers.Add(looser);
    }

    public void AddWinner(PlayerStatManager winner)
    {
        winners.Add(winner);
    }
    
    void DestroyOnMenuScreen(Scene oldScene, Scene newScene)
    {
        if (newScene.name == menuScreenBuildName) //could compare Scene.name instead
        {
            print("destroy");
            if (this != null)
            {
                Destroy(gameObject);
            }
        }
    }
}
