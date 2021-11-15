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

    // Start is called before the first frame update
    void Start()
    {
        //nameMiniGame = gameObject.name;
        //nameMiniGame = nameMiniGame.Split('_')[0];

        distanceToStart = 3;
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
                    createdMiniGame = Instantiate(miniGame, new Vector3(0, 0, 0), miniGame.transform.rotation);
                    createdMiniGame.SetActive(true);
                    createdMiniGame.transform.SetParent(gameObject.transform, false);
                }
            }
            else if (gameObject.transform.childCount == 0 && isOpen)
            {
                isOpen = false;
            }
        }
        if (gameObject.transform.childCount != 0)
        {
            if (!gameObject.transform.GetChild(0).gameObject.activeSelf)
            {
                Destroy(gameObject.transform.GetChild(0).gameObject);
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
