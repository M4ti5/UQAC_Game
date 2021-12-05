using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EndGame : MonoBehaviourPun
{
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject instanceEndGame;

    public Transform allPlayers;
    
    public List<PlayerInfoEndGame> loosers;
    public List<PlayerInfoEndGame> winners;

    public bool endGame = false;
    public static string menuScreenBuildName = "Launcher"; //the menu screen's index in your Build Settings
    public static string endScreenBuildName = "End"; //the end screen's index in your Build Settings
    
    [System.Serializable]
    public class PlayerInfoEndGame
    {
        public PlayerInfoEndGame(int viewId, bool isMine, string name, bool isCriminal, bool isDead)
        {
            this.viewId = viewId;
            this.isMine = isMine;
            this.name = name;
            this.isCriminal = isCriminal;
            this.isDead = isDead;
        }

        public int viewId;
        public bool isMine;
        public string name;
        public bool isCriminal;
        public bool isDead;
    }

    private void Awake()
    {
        SceneManager.activeSceneChanged += DestroyOnMenuScreen;
        if (this != null)
        {
            if (instanceEndGame == null)
            {
                instanceEndGame = gameObject;
                DontDestroyOnLoad(gameObject);

                loosers = new List<PlayerInfoEndGame>();
                winners = new List<PlayerInfoEndGame>();
                return;
            }

            if (instanceEndGame == this) return;
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (this != null && endGame == false && PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount > 1) // s'il a plus qu'un joueur dans la room
                {
                    foreach (Transform player in allPlayers)
                    {
                        if (player.GetComponent<PlayerStatManager>().isDead)
                        {
                            if (loosers.Count((looser) => looser.viewId == player.GetComponent<PhotonView>().ViewID) ==
                                0)
                            {
                                AddLooser(
                                    player.GetComponent<PhotonView>().ViewID,
                                    player.GetComponent<PhotonView>().IsMine,
                                    player.GetComponent<PlayerStatManager>().playerName,
                                    player.GetComponent<PlayerStatManager>().criminal,
                                    player.GetComponent<PlayerStatManager>().isDead
                                );

                                // if looser is criminel tout les autres ont gagné
                                if (player.GetComponent<PlayerStatManager>().criminal == true)
                                {
                                    foreach (Transform playerBis in allPlayers)
                                    {
                                        if (playerBis.GetComponent<PlayerStatManager>().criminal == false)
                                        {
                                            if (winners.Count((winner) =>
                                                winner.viewId == playerBis.GetComponent<PhotonView>().ViewID) == 0)
                                            {
                                                AddWinner(
                                                    playerBis.GetComponent<PhotonView>().ViewID,
                                                    playerBis.GetComponent<PhotonView>().IsMine,
                                                    playerBis.GetComponent<PlayerStatManager>().playerName,
                                                    playerBis.GetComponent<PlayerStatManager>().criminal,
                                                    playerBis.GetComponent<PlayerStatManager>().isDead
                                                );
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // s'il ne reste plus qu'une personne alors, il a gagné
                    if (loosers.Count + winners.Count >= PhotonNetwork.CurrentRoom.PlayerCount - 1)
                    {
                        foreach (Transform player in allPlayers)
                        {
                            if (player.GetComponent<PlayerStatManager>().isDead == false)
                            {
                                if (winners.Count((winner) =>
                                        winner.viewId == player.GetComponent<PhotonView>().ViewID) ==
                                    0)
                                {
                                    AddWinner(
                                        player.GetComponent<PhotonView>().ViewID,
                                        player.GetComponent<PhotonView>().IsMine,
                                        player.GetComponent<PlayerStatManager>().playerName,
                                        player.GetComponent<PlayerStatManager>().criminal,
                                        player.GetComponent<PlayerStatManager>().isDead
                                    );
                                }
                            }
                        }
                    }

                    if (loosers.Count + winners.Count >= PhotonNetwork.CurrentRoom.PlayerCount)
                    {
                        endGame = true;
                        if (PhotonNetwork.IsMasterClient)
                        {
                            StartCoroutine(LoadEndScene());
                        }
                    }
                }
            }
        }
    }

    [PunRPC]
    public void AddLooser(int viewId, bool isMine, string name, bool isCriminal, bool isDead)
    {
        PlayerInfoEndGame temp = new PlayerInfoEndGame(viewId, isMine, name, isCriminal, isDead);
        if (loosers.Count((looser) => looser.viewId == temp.viewId) == 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                loosers.Add(temp);
                photonView.RPC(nameof(AddLooser), RpcTarget.AllBuffered, viewId, isMine, name, isCriminal, isDead);
            }
        }
    }

    [PunRPC]
    public void AddWinner(int viewId, bool isMine, string name, bool isCriminal, bool isDead)
    {
        PlayerInfoEndGame temp = new PlayerInfoEndGame(viewId, isMine, name, isCriminal, isDead);
        if (winners.Count((winner) => winner.viewId == temp.viewId) == 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                winners.Add(temp);
                photonView.RPC(nameof(AddWinner), RpcTarget.AllBuffered, viewId, isMine, name, isCriminal, isDead);
            }
        }
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
        /*if (newScene.name == endScreenBuildName) //could compare Scene.name instead
        {
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (PlayerInfoEndGame looser in loosers)
                {
                    photonView.RPC(nameof(AddLooser), RpcTarget.AllBuffered);
                }
            }
        }*/
    }

    IEnumerator LoadEndScene()
    {
        yield return new WaitForSeconds(5);// attente de la fin des animations
        print("End");
        PhotonNetwork.LoadLevel("End");
    }


    
}
