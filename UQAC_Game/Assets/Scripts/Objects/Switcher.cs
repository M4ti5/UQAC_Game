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
            Switch(randomPlayer.position , randomPlayer.rotation);
            print(randomPlayer.gameObject.GetPhotonView().ViewID);
            print(PhotonNetwork.PlayerList[0].ActorNumber);

            //Network task
            Photon.Realtime.Player networkBindRandomPlayer = PhotonNetwork.PlayerList.Where(player => player.ActorNumber == randomPlayer.gameObject.GetPhotonView().ControllerActorNr).First();
            photonView.RPC(nameof(Switch) , networkBindRandomPlayer , player.position , player.rotation, randomPlayer.gameObject.GetPhotonView().ViewID ); ;
           
        }
    }



    [PunRPC]
    private void Switch (Vector3 otherPos , Quaternion otherRot) {
        transform.parent.parent.position = otherPos;
        transform.parent.parent.rotation = otherRot;
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
    private void Switch (Vector3 otherPos , Quaternion otherRot , int otherId) {
        Transform temp = FindPlayerByID(otherId);
        temp.position = otherPos;
        temp.rotation = otherRot;
    } 
}
