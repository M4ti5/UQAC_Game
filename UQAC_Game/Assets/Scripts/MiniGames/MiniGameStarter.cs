using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class MiniGameStarter : MonoBehaviour
{
    //private string nameMiniGame;

    protected bool isOpen = false;
    protected bool gameEnded = false;
    public float distanceToStart;
    public GameObject allPlayers;

    private GameObject createdMiniGame;
    public GameObject miniGame;

    //Booléen indiquant si le joueur qui ouvre le mini-jeu est un criminel ou un enquêteur
    public bool criminal = false;

    public GameObject panelScore;
    private PersonalScore personalScore;
    private GlobalScore globalScore;

    public GameObject panelHP;
    private HealthBar healthBar;

    private GameObject miniGameActive;


    // Start is called before the first frame update
    void Start()
    {
        distanceToStart = 3;
        globalScore = panelScore.GetComponentInChildren<GlobalScore>();
        personalScore = panelScore.GetComponentInChildren<PersonalScore>();
        healthBar = panelHP.GetComponentInChildren<HealthBar>();

        
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameEnded)
        {
            if (Input.GetKeyDown(KeyCode.N) && !isOpen)
            {
                //appelé si le joueur débute veut ouvrir un mini jeu
                int allPlayersCount = allPlayers.transform.childCount;
                int grabberPlayerId = -1;
                float minDistance = float.PositiveInfinity;

                //On récupère l'ID du joueur et on regarde si il est assez proche d'un mini-jeu
                for (int i = 0; i < allPlayersCount; i++)
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

                //Si le joueur est assez proche, on crée une instance de mini jeu que le joueur devra résoudre
                if (grabberPlayerId >= 0)
                {
                    isOpen = true;
                    miniGameActive = new GameObject();
                    miniGameActive.transform.SetParent(gameObject.transform.parent);

                    createdMiniGame = Instantiate(miniGame, new Vector3(0, 0, 0), miniGame.transform.rotation);
                    createdMiniGame.SetActive(true);
                    createdMiniGame.transform.SetParent(miniGameActive.transform, false);

                    PlayerStatManager playerStatManager = GetComponent<PlayerStatManager>();
                    playerStatManager = GetPlayerStatManager();
                    playerStatManager.canMove(false);
                }
            }
            else if (miniGameActive == null && isOpen)
            {
                //appelé si le joueur appui sur LeaveMiniGame
                isOpen = false;
                PlayerStatManager playerStatManager = GetComponent<PlayerStatManager>();
                playerStatManager = GetPlayerStatManager();
                playerStatManager.canMove(true);
            }
        }
        if (miniGameActive != null)
        {
            if (!miniGameActive.transform.GetChild(0).gameObject.activeSelf)
            {
                //Appelé si le joueur a terminé un mini jeu

                //Récupération du script contenant les stats du joueur
                PlayerStatManager playerStatManager = GetComponent<PlayerStatManager>();
                playerStatManager = GetPlayerStatManager();

                //Diminution du score global si le joueur est un criminel
                //Augmentation du score personnel et global si le joueur est un enquêteur
                if (criminal)
                {
                    playerStatManager.DecreaseGlobalScore();
                }
                else
                {
                    playerStatManager.IncreaseGlobalScore();
                    playerStatManager.IncreasePersonalScore();
                }
                //Le joueur récupère des PV
                playerStatManager.canMove(true);
                playerStatManager.RecoverHP(15);
                Destroy(miniGameActive);
                gameEnded = true;
            }
        }
    }

    //On regarde si le joueur est assez proche du miniGameStarter
    (bool, float) IsReachable(Transform objectA, Transform playerA, float range)
    {
        float dist = Vector3.Distance(objectA.position, playerA.position);
        float angle = Vector3.Angle(playerA.forward, objectA.position - playerA.position);

        if (dist < range && angle <= Mathf.Abs(30))
        {
            return (true, dist);
        }
        else
        {
            return (false, dist);
        }

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
}
