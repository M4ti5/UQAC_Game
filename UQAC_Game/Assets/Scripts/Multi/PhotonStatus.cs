using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

/// <summary>
/// Display Photon Status for the room
/// </summary>
public class PhotonStatus : MonoBehaviourPun
{
    public bool activated = false;
    public GameObject photonStatus;
    public TextMeshProUGUI playerName, roomName, nbrPlayers, ping, roomsList;

    private string NA = "N/A";

    public List<RoomInfo> roomsInfos;

    // Start is called before the first frame update
    void Start()
    {
        // check if we enable status (changed with options menu)
        if (activated)
        {
            CantGetSatus();
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
            // if we are connected to internet and photon cloud servers
            if (PhotonNetwork.IsConnected)
            {
                DisplayStatus();
            }
            else
            {
                CantGetSatus();
            }
        }
    }

    // display non define if error
    private void CantGetSatus()
    {
        playerName.text = NA;
        roomName.text = NA;
        nbrPlayers.text = NA;
        ping.text = NA + " ms\n(region: " + NA + ")";
    }

    // display allow possible information depending on if we are in lobby (home scene) or in a room (because photon not allow all every where)
    private void DisplayStatus()
    {
        playerName.text = PhotonNetwork.NickName.ToString();
        roomName.text = (PhotonNetwork.InRoom ? 
            PhotonNetwork.CurrentRoom.Name.ToString() + " [" + (PhotonNetwork.CurrentRoom.IsOpen ? "open" : "close") + "]" : 
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
                PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + " in this room" :
                "");
        ping.text = PhotonNetwork.GetPing().ToString() + " ms\n(region: " + PhotonNetwork.CloudRegion + ")";

        DisplayRoomsList();
    }

    // display room list if we are in lobby or call display players in room
    private void DisplayRoomsList()
    {
        // if we are in lobby / home scene
        if (PhotonNetwork.Server == ServerConnection.MasterServer)
        {
            if (FindObjectOfType<Launcher>())
            {
                roomsInfos = FindObjectOfType<Launcher>().GetRoomList();
                // reset display list of all rooms
                roomsList.text = "Created Rooms: ";
                foreach (RoomInfo roomInfo in roomsInfos)
                {
                    // txt
                    roomsList.text += "\n" + 
                                      (roomInfo.IsOpen ? "open" : "close") + " - " +
                                      (roomInfo.IsVisible ? "visible" : "hidden") + " - " + 
                                      roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers +  " - " + 
                                      roomInfo.Name;

                }
            }
            else
            {
                roomsList.text = NA;
            }
        }
        else // if we are not in lobby => we are in a room so display players
        {
            DisplayPlayersList();
        }
    }
    
    // display list of players in our room
    private void DisplayPlayersList()
    {
        if (PhotonNetwork.InRoom)
        {
            // reset list of all rooms
            roomsList.text = "Players: ";
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                // txt
                roomsList.text += "\n" +
                                  player.NickName + (player.IsMasterClient ? " (Master)" : "");

            }
        }
        else // if we are not in room and not on lobby
        {
            roomsList.text = "";
        }
    }
}
