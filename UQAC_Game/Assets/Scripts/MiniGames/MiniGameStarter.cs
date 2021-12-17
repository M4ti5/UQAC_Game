using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MiniGameStarter : MonoBehaviourPun
{
    public bool isOpen = false;
    public bool gameEnded = false;
    public float distanceToStart = 5 ;
    public GameObject allPlayers;

    private GameObject createdMiniGame;
    public GameObject miniGame;

    //Boolean that say if the player is a criminal or not
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
                //called when the player want to enter a miniGame.
                int allPlayersCount = allPlayers.transform.childCount;
                int grabberPlayerId = -1;
                float minDistance = float.PositiveInfinity;

                //We get the id of the closest miniGame and the distance between the player and this miniGame
                for (int i = 0; i < allPlayersCount; i++)
                {
                    //test only if the player is still alive
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

                //If the player is close enough, we create a new instance of the mini game and we display it
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
                    //the player can't move when a mini game is open
                    playerStatManager.canMove= false;
                    //We change the type of the player to be sure that the score will evolve in the good direction at the end of the minigame
                    criminal = playerStatManager.criminal;
                }
            }
            else if (miniGameActive == null && isOpen)
            {
                //called when the mini game click on the leaveMiniGame button (miniGameActive is destroyed in LeaveMiniGame.cs)
                isOpen = false;
                PlayerStatManager playerStatManager = GetComponent<PlayerStatManager>();
                playerStatManager = GetPlayerStatManager();
                playerStatManager.canMove = true;
            }
        }
        //When the player end the mini game, start a 2 min cooldown
        //At the end of the cooldown, the miniGameStarter is reactivated
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
                //Called when the player finish the miniGame
                PlayerStatManager playerStatManager = GetComponent<PlayerStatManager>();
                playerStatManager = GetPlayerStatManager();

                //The globalScore decrease if the player is a criminal
                //The globalScore and the personalScore increase if the player is an investigator
                if (criminal)
                {
                    playerStatManager.DecreaseGlobalScore();
                }
                else
                {
                    playerStatManager.IncreaseGlobalScore();
                    playerStatManager.IncreasePersonalScore();
                }
                //The player recover 15 HP
                playerStatManager.canMove = true;
                PhotonView playerPhotonView = playerStatManager.thisPlayer.GetComponent<PhotonView>();
                
                photonView.RPC(nameof(RecoverHPMiniGameStarter), RpcTarget.AllBuffered, 15, playerPhotonView.ViewID, photonView.ViewID);


                //int newId = PhotonNetwork.AllocateViewID(true);
                if (playerStatManager.objectPrefabListToInstantiate.Count != 0)
                {
                    int idToSpawn = Random.Range(0, playerStatManager.objectPrefabListToInstantiate.Count);
                    playerStatManager.spawnObject(Vector3.zero, Quaternion.identity, idToSpawn);
                }
                
                Destroy(miniGameActive);
                gameEnded = true;
                lastTimeUseMiniGame = Time.time;// reset cooldown
            }
        }
    }


    //Verify if the player is close enough of the miniGameStarter
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
    private void RecoverHPMiniGameStarter(int heal, int viewId, int viewIdMiniGame)
    {
        if (photonView.ViewID == viewIdMiniGame)
        {
            Transform player = FindPlayerByID(viewId);
            player.GetComponent<PlayerStatManager>().RecoverHP(heal, viewId);
        }
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
