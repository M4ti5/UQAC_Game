using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MiniGameStarter : MonoBehaviour
{
    public MiniGamesManager miniGameManager;
    private string nameMiniGame;

    protected bool isOpen = false;
    public float distanceToStart;
    public GameObject allPlayers;
    public GameObject allObjects;

    protected Transform player;
    protected Transform miniGameActive;

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
        if (Input.GetKeyDown(KeyCode.N) && !isOpen){
            

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
                        Debug.Log("break ? ");
                    break;
                }
                Debug.Log("Reachable ? " + _isReachable);
            }
            Debug.Log("Grabber ID ? " + grabberPlayerId);
            if (grabberPlayerId >= 0)
            {
                player = allPlayers.transform.GetChild(grabberPlayerId);
                miniGameActive = player.transform.Find("MiniGameActive");
                //if (EquipmentDest.GetComponent<UseObject>().hasObject == false && player.GetComponent<PhotonView>().IsMine)
                //{
                //    OnEquipmentTriggered(player);
                //}
                int j = 0;
                foreach (GameObject miniGame in miniGameManager.allMiniGames)
                {
                    if (miniGame.name == nameMiniGame)
                    {
                        miniGameManager.allMiniGamesEnabled[j] = true;
                        isOpen = true;
                        break;
                    }
                    j += 1;
                }
            }
        }

        (bool, float) IsReachable(Transform objectA, Transform playerA, float range)
        {
            float dist = Vector3.Distance(objectA.position, playerA.position);
            float angle = Vector3.Angle(playerA.forward, objectA.position - playerA.position);

            Debug.Log("Dist ID ? " + dist);
            Debug.Log("Angle ? " + angle);
            if (dist < range && angle <= Mathf.Abs(30))
            {
                Debug.Log("true ? ");
                return (true, dist);
            }
            else
            {
                Debug.Log("false ? ");
                return (false, dist);
            }

        }
    }
}
