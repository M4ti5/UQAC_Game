using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class Switcher : Object {

    public override void Behaviour () {
        if (Time.time - lastTimeUseObject > deltaTimeUseObject) {
            lastTimeUseObject = Time.time;

            Transform player = transform.parent.parent;
            Transform listPlayers = player.parent;

            //Player random
            int randomIndex = Random.Range(0 , listPlayers.childCount-1);
            Transform randomPlayer = listPlayers.GetChild(randomIndex) == GetComponent<PhotonView>().IsMine ? listPlayers.GetChild(randomIndex+1 % listPlayers.childCount) : listPlayers.GetChild(randomIndex);

            //Local task
            Switch(randomPlayer);
            print(randomPlayer.gameObject.GetPhotonView().ViewID);
            print(PhotonNetwork.PlayerList[0].ActorNumber);

            Photon.Realtime.Player networkBindRandomPlayer = PhotonNetwork.PlayerList.Where(player => player.ActorNumber == randomPlayer.gameObject.GetPhotonView().ControllerActorNr).First();

            //Network task
            photonView.RPC(nameof(Switch) , networkBindRandomPlayer , player , randomPlayer.gameObject.GetPhotonView().ViewID ); ;
           
        }
    }

    Transform FindPlayerByID (int id) {
        foreach (Transform child in allPlayers.transform) {
            if (child.GetComponent<PhotonView>().ViewID == id) {
                return child;
            }
        }
        return null;
    }

    [PunRPC]
    private void Switch (Transform other) {
        transform.parent.parent.position = other.position;
        transform.parent.parent.rotation = other.rotation;
    }

    [PunRPC]
    private void Switch (Transform other , int otherId) {
        Transform temp = FindPlayerByID(otherId);
        temp.position = other.position;
        temp.rotation = other.rotation;
    }
}
