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
                int allPlayersCount = allPlayers.transform.childCount;
                int grabberPlayerId = -1;
                float minDistance = float.PositiveInfinity;

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

                if (grabberPlayerId >= 0)
                {
                    isOpen = true;
                    miniGameActive = new GameObject();
                    miniGameActive.transform.SetParent(gameObject.transform.parent);

                    createdMiniGame = Instantiate(miniGame, new Vector3(0, 0, 0), miniGame.transform.rotation);
                    createdMiniGame.SetActive(true);
                    createdMiniGame.transform.SetParent(miniGameActive.transform, false);
                }
            }
            else if (miniGameActive == null && isOpen)
            {
                isOpen = false;
            }
        }
        if (miniGameActive != null)
        {
            if (!miniGameActive.transform.GetChild(0).gameObject.activeSelf)
            {
                if (criminal)
                {
                    globalScore.DecreaseScore();
                }
                else
                {
                    globalScore.IncreaseScore();
                    personalScore.IncreaseScore();
                }
                healthBar.RecoverHP(15);
                Destroy(miniGameActive);
                gameEnded = true;
            }
        }
    }

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
}
