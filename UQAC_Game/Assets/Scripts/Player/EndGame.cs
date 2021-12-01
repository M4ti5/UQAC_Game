using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject instanceEndGame;

    public Transform allPlayers;
    public bool firstInitAllEnqueteurs = true;
    public bool isFindingAllEnqueteurs = false;
    public List<PlayerStatManager> allEnqueteurs;
    
    public List<PlayerStatManager> loosers;
    public List<PlayerStatManager> winners;

    public bool endGame = false;
    public static string menuScreenBuildName = "Launcher"; //the menu screen's index in your Build Settings
    
    private void Awake()
    {
        firstInitAllEnqueteurs = true;
        isFindingAllEnqueteurs = false;
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
            if (allPlayers != null)
            {
                if (firstInitAllEnqueteurs == true && isFindingAllEnqueteurs == false)
                {
                    isFindingAllEnqueteurs = true;
                    StartCoroutine(FindAllEnqueteurs());
                }

                if (firstInitAllEnqueteurs == false)
                {
                    if (PhotonNetwork.CurrentRoom.PlayerCount > 1) // eviter de terminer directement la partie d'entrainement
                    {
                        foreach (Transform player in allPlayers)
                        {
                            PlayerStatManager playerStatManager = player.GetComponent<PlayerStatManager>();
                            // add looser players if there are dead and no already in list
                            if (playerStatManager.isDead &&
                                loosers.Find((looser) => looser == playerStatManager) == null)
                            {
                                AddLooser(playerStatManager);
                                if (playerStatManager.criminal ==
                                    true) // si le criminel est mort alors tous les autres ont gagné
                                {
                                    foreach (Transform winner in allPlayers)
                                    {
                                        PlayerStatManager winnerPlayerStatManager =
                                            winner.GetComponent<PlayerStatManager>();
                                        if (winnerPlayerStatManager.isDead == false &&
                                            winners.Find((winner) => winner == winnerPlayerStatManager) == null)
                                        {
                                            AddWinner(winnerPlayerStatManager);
                                        }
                                    }

                                    break;
                                }
                            }
                        }


                        // si on a autant de loosers que d'enqueteurs alors le criminel a gagné
                        if (loosers.Count == allEnqueteurs.Count)
                        {
                            foreach (PlayerStatManager playerStatManager in allPlayers
                                .GetComponentsInChildren<PlayerStatManager>().Where((player) => player.criminal == true)
                                .ToList())
                            {
                                if (winners.Find((winner) => winner == playerStatManager) == null)
                                    AddWinner(playerStatManager);
                            }
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

    IEnumerator FindAllEnqueteurs()
    {
        yield return new WaitForSeconds(5);
        // find all enqueteurs
        foreach (Transform player in allPlayers)
        {
            if (player.GetComponent<PlayerStatManager>().criminal == false)
            {
                allEnqueteurs.Add(player.GetComponent<PlayerStatManager>());
            }
        }
        firstInitAllEnqueteurs = false;
        isFindingAllEnqueteurs = false;
    }

    IEnumerator LoadEndScene()
    {
        yield return new WaitForSeconds(5);// attente de la fin des animations
        print("End");
        PhotonNetwork.LoadLevel("End");
    }
    
}
