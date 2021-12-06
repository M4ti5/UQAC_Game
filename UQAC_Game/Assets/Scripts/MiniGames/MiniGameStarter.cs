using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MiniGameStarter : MonoBehaviour
{
    //private string nameMiniGame;

    public bool isOpen = false;
    public bool gameEnded = false;
    public float distanceToStart = 5 ;
    public GameObject allPlayers;

    private GameObject createdMiniGame;
    public GameObject miniGame;

    //Bool�en indiquant si le joueur qui ouvre le mini-jeu est un criminel ou un enqu�teur
    public bool criminal = false;

    public GameObject panelScore;
    private PersonalScore personalScore;
    private GlobalScore globalScore;

    public GameObject panelHP;
    private HealthBar healthBar;

    private GameObject miniGameActive;

    protected float lastTimeUseMiniGame;
    public float deltaTimeUseMiniGame = 120;// 2 min


    // Start is called before the first frame update
    void Start()
    {
        globalScore = panelScore.GetComponentInChildren<GlobalScore>();
        personalScore = panelScore.GetComponentInChildren<PersonalScore>();
        healthBar = panelHP.GetComponentInChildren<HealthBar>();
        isOpen = false;
        gameEnded = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameEnded)
        {
            if (Input.GetKeyDown(KeyCode.E) && !isOpen)
            {
                //appel� si le joueur d�bute veut ouvrir un mini jeu
                int allPlayersCount = allPlayers.transform.childCount;
                int grabberPlayerId = -1;
                float minDistance = float.PositiveInfinity;

                //On r�cup�re l'ID du joueur et on regarde si il est assez proche d'un mini-jeu
                for (int i = 0; i < allPlayersCount; i++)
                {
                    if (allPlayers.transform.GetChild(i).GetComponent<PlayerStatManager>().isDead == false)
                    {
                        (bool _isReachable, float _dist) = IsReachable(gameObject.transform, allPlayers.transform.GetChild(i), distanceToStart);
                        if (_isReachable && _dist < minDistance)
                        {
                            minDistance = _dist;
                            grabberPlayerId = i;
                            if (allPlayers.transform.GetChild(i).GetComponent<PhotonView>().IsMine)
                                break;
                        }
                    }
                }

                //Si le joueur est assez proche, on cr�e une instance de mini jeu que le joueur devra r�soudre
                if (grabberPlayerId >= 0 && allPlayers.transform.GetChild(grabberPlayerId).GetComponent<PhotonView>().IsMine)
                {
                    isOpen = true;
                    miniGameActive = new GameObject();
                    miniGameActive.transform.SetParent(gameObject.transform.parent);

                    createdMiniGame = Instantiate(miniGame, new Vector3(0, 0, 0), miniGame.transform.rotation);
                    createdMiniGame.SetActive(true);
                    createdMiniGame.transform.SetParent(miniGameActive.transform, false);

                    PlayerStatManager playerStatManager = GetComponent<PlayerStatManager>();
                    playerStatManager = GetPlayerStatManager();
                    playerStatManager.canMove= false;
                    criminal = playerStatManager.criminal;
                }
            }
            else if (miniGameActive == null && isOpen)
            {
                //appel� si le joueur appui sur LeaveMiniGame
                isOpen = false;
                PlayerStatManager playerStatManager = GetComponent<PlayerStatManager>();
                playerStatManager = GetPlayerStatManager();
                playerStatManager.canMove = true;
            }
        }
        // start cooldown de 2 min avant de pouvoir s'en reservir
        else
        {
            // wait cooldown
            if (Time.time - lastTimeUseMiniGame > deltaTimeUseMiniGame)
            {
                lastTimeUseMiniGame = Time.time;
                gameEnded = false;
            }
        }
        if (miniGameActive != null)
        {
            if (!miniGameActive.transform.GetChild(0).gameObject.activeSelf)
            {
                //Appel� si le joueur a termin� un mini jeu
                //R�cup�ration du script contenant les stats du joueur
                PlayerStatManager playerStatManager = GetComponent<PlayerStatManager>();
                playerStatManager = GetPlayerStatManager();

                //Diminution du score global si le joueur est un criminel
                //Augmentation du score personnel et global si le joueur est un enqu�teur
                if (criminal)
                {
                    playerStatManager.DecreaseGlobalScore();
                }
                else
                {
                    playerStatManager.IncreaseGlobalScore();
                    playerStatManager.IncreasePersonalScore();
                }
                //Le joueur r�cup�re des PV
                playerStatManager.canMove = true;
                PhotonView playerPhotonView = playerStatManager.thisPlayer.GetComponent<PhotonView>();
                //playerStatManager.RecoverHP(15, playerStatManager.ViewID);
                if (playerPhotonView.IsMine)
                {
                    print("playerPhotonView.IsMine " + playerPhotonView.ViewID);
                    playerPhotonView.RPC(nameof(PlayerStatManager.RecoverHPStatic), RpcTarget.AllBuffered, 15, playerPhotonView.ViewID);
                }
                print("update " + playerPhotonView.ViewID);

                //int newId = PhotonNetwork.AllocateViewID(true);
                //TODO add if list is not empty
                int idToSpawn = Random.Range(0,playerStatManager.objectPrefabListToInstantiate.Count);
                
                //photonView.RPC(nameof(PlayerStatManager.spawnObject), RpcTarget.AllBuffered,Vector3.zero, Quaternion.identity, newId, idToSpawn);
                playerStatManager.spawnObject(Vector3.zero, Quaternion.identity, idToSpawn);
                Destroy(miniGameActive);
                gameEnded = true;
                lastTimeUseMiniGame = Time.time;// reset cooldown
            }
        }
    }


    //On regarde si le joueur est assez proche du miniGameStarter
    (bool, float) IsReachable(Transform objectA, Transform playerA, float range)
    {
        
        float dist = Vector3.Distance(objectA.position - new Vector3(0 , objectA.position.y , 0) , (playerA.position - new Vector3(0, playerA.position.y, 0)));
        if (dist < range)
        {
            float angle = Vector3.Angle(playerA.forward,
                (objectA.position - new Vector3(0, objectA.position.y, 0)) - (playerA.position - new Vector3(0, playerA.position.y, 0)));

            if (angle <= Mathf.Abs(30))
            {
                return (true, dist);
            }
        }
        
        return (false, dist);

    }
    

    private PlayerStatManager GetPlayerStatManager()
    {
        PlayerStatManager playerStatManager = GetComponent<PlayerStatManager>();
        int playerCount = allPlayers.transform.childCount;
        for (int i = 0; i < playerCount; i++)
        {
            if (allPlayers.transform.GetChild(i).GetComponent<PhotonView>().IsMine)
            {
                playerStatManager = allPlayers.transform.GetChild(i).transform.GetComponent<PlayerStatManager>();
            }
        }
        return playerStatManager;
    }

    [PunRPC]
    private void RecoverHPMiniGameStarter(int heal, int viewId)
    {
        print("RecoverHPMiniGameStarter " + viewId);
        Transform player = FindPlayerByID(viewId);
        player.GetComponent<PlayerStatManager>().RecoverHP(heal, viewId);
    }
    
    private Transform FindPlayerByID(int id)
    {
        foreach (Transform child in allPlayers.transform)
        {
            if (child.GetComponent<PhotonView>().ViewID == id)
            {
                return child;
            }
        }
        return null;
    }
}
