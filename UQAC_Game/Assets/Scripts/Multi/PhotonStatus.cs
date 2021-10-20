using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

/// <summary>
/// Display Photon Status for the room
/// </summary>
public class PhotonStatus : MonoBehaviourPun
{
    public bool activated = true;
    public GameObject photonStatus;
    public TextMeshProUGUI playerName, roomName, nbrPlayers, ping;

    // Start is called before the first frame update
    void Start()
    {
        if (activated)
        {
            playerName.text = "N/A";
            roomName.text = "N/A";
            nbrPlayers.text = "N/A";
            ping.text = "N/A ms";
        }
        else
        {
            if (photonStatus)
            {
                photonStatus.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                playerName.text = PhotonNetwork.NickName.ToString();
                roomName.text = PhotonNetwork.CurrentRoom.Name.ToString() + " [" + (PhotonNetwork.CurrentRoom.IsVisible ? "visible" : "hidden") + "]";
                nbrPlayers.text = PhotonNetwork.CountOfPlayers + " players\n" +
                    PhotonNetwork.CountOfPlayersInRooms + " players in rooms\n" +
                    PhotonNetwork.CountOfPlayersOnMaster + " players on master\n" +
                    PhotonNetwork.CurrentRoom.PlayerCount + " players in current room";
                ping.text = PhotonNetwork.GetPing() + " ms";
            }
        }
    }
}
