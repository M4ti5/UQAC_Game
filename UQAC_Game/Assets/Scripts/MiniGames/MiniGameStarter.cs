using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class MiniGameStarter : MonoBehaviour
{
    public MiniGamesManager miniGameManager;
    private string nameMiniGame;

    protected bool isOpen = false;
    protected bool gameActive = false;
    public float distanceToStart;
    public GameObject allPlayers;
    public Button exitButton;
    private GameObject createdMiniGame;

    // Start is called before the first frame update
    void Start()
    {
        nameMiniGame = gameObject.name;
        nameMiniGame = nameMiniGame.Split('_')[0];

        distanceToStart = 3;
    }

    // Update is called once per frame
    void Update()
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
                int j = 0;
                foreach (GameObject miniGame in miniGameManager.allMiniGames)
                {
                    if (miniGame.name == nameMiniGame)
                    {
                        Vector3 position = new Vector3(miniGameManager.allMiniGames[j].transform.position.x, miniGameManager.allMiniGames[j].transform.position.y, 0);
                        createdMiniGame = Instantiate(miniGameManager.allMiniGames[j], position, miniGameManager.allMiniGames[j].transform.rotation);
                        createdMiniGame.SetActive(true);
                        var test2 = createdMiniGame.GetComponent<PiecesManager>();
                        createdMiniGame.transform.SetParent(gameObject.transform, false);
                        miniGameManager.allMiniGamesEnabled[j] = true;
                        isOpen = true;
                        gameActive = true;
                        exitButton.gameObject.SetActive(true);
                        break;
                    }
                    j += 1;
                }
            }
        }
        else if (gameActive)
        {
            bool ended = false;
            int j = 0;
            foreach (GameObject miniGame in miniGameManager.allMiniGames)
            {
                if (miniGameManager.allMiniGamesEnabled[j] == true)
                {
                    ended = true;
                    break;
                } 
                j += 1;
            }
            if (ended == false)
            {
                WinMiniGame();
            }
        }
        //else if (isOpen && !gameActive)
        //{
        //    int j = 0;
        //    foreach (GameObject miniGame in miniGameManager.allMiniGames)
        //    {
        //        if (miniGame.name == nameMiniGame)
        //        {
        //            Destroy(createdMiniGame);
        //            miniGameManager.allMiniGamesEnabled[j] = false;
        //            isOpen = false;
        //            break;
        //        }
        //        j += 1;
        //    }
        //}
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

    public void LeaveMiniGame()
    {
        gameActive = false;
        exitButton.gameObject.SetActive(false);
        Destroy(createdMiniGame);
        int j = 0;
        foreach (GameObject miniGame in miniGameManager.allMiniGames)
        {
            miniGameManager.allMiniGamesEnabled[j] = false;
            j += 1;
        }
        isOpen = false;
    }

    public void WinMiniGame()
    {
        exitButton.gameObject.SetActive(false);
        Destroy(createdMiniGame);
        Debug.Log("Victory kdjfhsdfhjskdghjkqsghd");
    }
}
