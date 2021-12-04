using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitingPlayers : MonoBehaviourPunCallbacks
{
    public int nbrParticipant;
    public GameObject prefabPlayerCard;
    public Transform firstColumn;
    public Transform secondColumn;
    public TextMeshProUGUI roomName;
    public Button startButton;

    // Start is called before the first frame update
    void Start()
    {
        // in case we started with the wrong scene being active, simply load the menu scene
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Launcher");

            return;
        }

        // get max number of waiting player
        if (PhotonNetwork.InRoom)
        {
            nbrParticipant = PhotonNetwork.CurrentRoom.MaxPlayers;
            roomName.text = PhotonNetwork.CurrentRoom.Name;
        }

        if (nbrParticipant == 1)
        {
            Instantiate(prefabPlayerCard, firstColumn.parent.transform).name = "Player_0";
            Destroy(firstColumn.gameObject);
            Destroy(secondColumn.gameObject);
        }
        else
        {
            for (int i = 0; i < nbrParticipant; i += 2)
            {
                Instantiate(prefabPlayerCard, firstColumn.transform).name = "Player_" + i;

            }

            for (int i = 1; i < nbrParticipant; i += 2)
            {
                Instantiate(prefabPlayerCard, secondColumn.transform).name = "Player_" + i;
            }
        }

        StartCoroutine(WaitStartAllCards());
    }

    IEnumerator WaitStartAllCards()
    {
        yield return new WaitUntil(() => GameObject.Find("Player_0").GetComponent<WaitingPlayerCard>().IsPlayerNameNull() == false);
        SetNameOfAllConnectedPlayers();
    }

    private void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient /*&& PhotonNetwork.CurrentRoom.PlayerCount == nbrParticipant*/) // si on a atteint le nbr max de players
            {
                startButton.interactable = true;
            }
            else
            {
                startButton.interactable = false;
            }
        }
    }

    public void StartGame()
    {
        LoadGame();
    }

    public void RetourHome()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        PhotonNetwork.LeaveRoom();
    }
    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Launcher");
    }
    
    // reload game
    void LoadGame()
    {
        if ( ! PhotonNetwork.IsMasterClient )
        {
            Debug.LogWarning( "PhotonNetwork : Trying to Load a level but we are not the master Client" );
            return;
        }

        Debug.LogFormat("PhotonNetwork : Loading Game - PlayerCount: {0}", PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("Game");
        
        // close room for other players
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }
    
    
    /// <summary>
    /// Called when a Photon Player got connected
    /// </summary>
    /// <param name="other">Other.</param>
    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.Log( "OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting

        if ( other.IsMasterClient )
        {
            Debug.LogFormat( "OnPlayerEnteredRoom IsMasterClient {0}", other.IsMasterClient ); // called before OnPlayerLeftRoom

        }

        StartCoroutine(WaitStartAllCards());
    }

    /// <summary>
    /// Called when a Photon Player got disconnected
    /// </summary>
    /// <param name="other">Other.</param>
    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.Log( "OnPlayerLeftRoom() " + other.NickName ); // seen when other disconnects

        if ( other.IsMasterClient )
        {
            Debug.LogFormat( "OnPlayerEnteredRoom IsMasterClient {0}", other.IsMasterClient ); // called before OnPlayerLeftRoom

        }

        StartCoroutine(WaitStartAllCards());
    }
    
    private void SetNameOfAllConnectedPlayers()
    {
        WaitingPlayerCard[] listOfCard = FindObjectsOfType<WaitingPlayerCard>();
        listOfCard = listOfCard.OrderBy(card=>card.gameObject.name).ToArray();// sort by gameobject name
        int i = 0;
        foreach (WaitingPlayerCard waitingPlayerCard in listOfCard)
        {
            if (i < PhotonNetwork.PlayerList.Length)
            {
                if (PhotonNetwork.PlayerList[i].IsMasterClient)
                    waitingPlayerCard.SetCardPlayerName(PhotonNetwork.PlayerList[i].NickName + "\n" + "<i>(Master)</i>");
                else
                    waitingPlayerCard.SetCardPlayerName(PhotonNetwork.PlayerList[i].NickName);
            }
            else
            {
                waitingPlayerCard.SetDefaultCardPlayerName();
            }

            i++;
        }
        
    }
}
