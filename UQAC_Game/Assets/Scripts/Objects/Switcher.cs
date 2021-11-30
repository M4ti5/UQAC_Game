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
            int randomIndex = Random.Range(0 , listPlayers.childCount);
            Transform randomPlayer = listPlayers.GetChild(randomIndex) == GetComponent<PhotonView>().IsMine ? listPlayers.GetChild(randomIndex+1 % listPlayers.childCount) : listPlayers.GetChild(randomIndex);

            //Local task
            Switch(randomPlayer);

            Photon.Realtime.Player networkBindRandomPlayer = PhotonNetwork.PlayerList.Where(player => player.UserId == randomPlayer.gameObject.GetPhotonView().ViewID.ToString()).First();

            //Network task
            photonView.RPC(nameof(Switch) , networkBindRandomPlayer , player); ;
           
        }
    }

    [PunRPC]
    private void Switch (Transform other) {
        transform.parent.parent.position = other.position;
        transform.parent.parent.rotation = other.rotation;
    }
}
