using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public float waitingTime = 5;
    public Transform allPlayers;

    // Start is called before the first frame update
    void Start()
    {
        if (allPlayers == null)
            allPlayers = GameObject.Find("Players").transform;
        transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(DesactiveImageStartGame());
    }

    IEnumerator DesactiveImageStartGame()
    {
        yield return toggleMoveMyPlayer(false);
        
        yield return new WaitForSeconds(waitingTime);
        transform.GetChild(0).gameObject.SetActive(false);
        
        yield return toggleMoveMyPlayer(true);
    }

    IEnumerator toggleMoveMyPlayer(bool canMove)
    {
        yield return new WaitWhile(() => allPlayers.childCount == 0);
        foreach (Transform player in allPlayers)
        {
            if (player.GetComponent<PhotonView>().IsMine)
                player.GetComponent<PlayerStatManager>().canMove = canMove;
        }
    }
}
