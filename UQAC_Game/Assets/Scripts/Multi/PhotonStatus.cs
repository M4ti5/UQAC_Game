using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
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

    private string NA = "N/A";

    // Start is called before the first frame update
    void Start()
    {
        if (activated)
        {
            playerName.text = NA;
            roomName.text = NA;
            nbrPlayers.text = NA;
            ping.text = NA + " ms";
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
            if (PhotonNetwork.IsConnected)
            {
                playerName.text = PhotonNetwork.NickName.ToString();
                roomName.text = (PhotonNetwork.InRoom ? 
                    PhotonNetwork.CurrentRoom.Name.ToString() + " [" + (PhotonNetwork.CurrentRoom.IsVisible ? "visible" : "hidden") + "]" : 
                    NA);
                nbrPlayers.text =
                    (PhotonNetwork.Server == ServerConnection.MasterServer ?
                        PhotonNetwork.CountOfPlayers.ToString() + " using app\n" :
                        "")  +
                    (PhotonNetwork.Server == ServerConnection.MasterServer ?
                        PhotonNetwork.CountOfPlayersInRooms.ToString() + " in all rooms\n" :
                        "")  +
                    (PhotonNetwork.Server == ServerConnection.MasterServer ? 
                        PhotonNetwork.CountOfPlayersOnMaster.ToString() + " on master\n" :
                        "")  +
                    (PhotonNetwork.InRoom ? 
                        PhotonNetwork.CurrentRoom.PlayerCount.ToString() + " in this room" :
                        "");
                ping.text = PhotonNetwork.GetPing().ToString() + " ms";
            }
        }
    }
}
