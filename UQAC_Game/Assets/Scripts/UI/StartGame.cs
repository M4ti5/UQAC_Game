using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// active first panel at the beginning of the game (during 5s)
public class StartGame : MonoBehaviour
{
    public float waitingTime = 5;
    public Transform allPlayers;

    // Start is called before the first frame update
    void Start()
    {
        if (allPlayers == null)
            allPlayers = GameObject.Find("Players").transform;
        
        // active first panel
        transform.GetChild(0).gameObject.SetActive(true);
        // start coroutine that will disable this panel
        StartCoroutine(DesactiveImageStartGame());
    }

    IEnumerator DesactiveImageStartGame()
    {
        // disable ability to move
        yield return toggleMoveMyPlayer(false);
        
        // wait 5s and then disable panel
        yield return new WaitForSeconds(waitingTime);
        transform.GetChild(0).gameObject.SetActive(false);
        
        // re enable ability to move
        yield return toggleMoveMyPlayer(true);
    }

    // wait player instantiate and set ability to move (disable when waiting)
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
