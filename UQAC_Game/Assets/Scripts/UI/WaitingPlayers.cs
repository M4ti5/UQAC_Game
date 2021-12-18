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

        // get max number of waiting player and room name
        if (PhotonNetwork.InRoom)
        {
            nbrParticipant = PhotonNetwork.CurrentRoom.MaxPlayers;
            roomName.text = PhotonNetwork.CurrentRoom.Name;
        }

        // check if we start entertainment room, in this case, we have 1 player so it isn't necessary to use columns
        if (nbrParticipant == 1)
        {
            Instantiate(prefabPlayerCard, firstColumn.parent.transform).name = "Player_0";
            Destroy(firstColumn.gameObject);
            Destroy(secondColumn.gameObject);
        }
        else
        {
            // init card by row and column
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
        // wait card instantiate
        yield return new WaitUntil(() => GameObject.Find("Player_0").GetComponent<WaitingPlayerCard>().IsPlayerNameNull() == false);
        
        SetNameOfAllConnectedPlayers();
    }

    private void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            // enable start buttom if we are master 
            if (PhotonNetwork.IsMasterClient /*&& PhotonNetwork.CurrentRoom.PlayerCount == nbrParticipant*/) // if room is full
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
        // if we are the last one, close room
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        // leave room to return to the lobby
        PhotonNetwork.LeaveRoom();
    }
    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Launcher");
    }
    
    // start game
    void LoadGame()
    {
        // just master can load new scene for everyone
        if ( ! PhotonNetwork.IsMasterClient )
        {
            Debug.LogWarning( "PhotonNetwork : Trying to Load a level but we are not the master Client" );
            return;
        }
        
        // load game scene for all players
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
            // set name in card for all connected players in the room
            if (i < PhotonNetwork.PlayerList.Length)
            {
                // add info if we are the master
                if (PhotonNetwork.PlayerList[i].IsMasterClient)
                    waitingPlayerCard.SetCardPlayerName(PhotonNetwork.PlayerList[i].NickName + "\n" + "<i>(Master)</i>");
                else
                    waitingPlayerCard.SetCardPlayerName(PhotonNetwork.PlayerList[i].NickName);
            }
            else // if number of players is less than room capacity, set other card to default value
            {
                waitingPlayerCard.SetDefaultCardPlayerName();
            }

            i++;
        }
        
    }
}
