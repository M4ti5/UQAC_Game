using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class Switcher : Object {

    public override void Behaviour () {

        if (player.GetComponent<PhotonView>().IsMine == true)
        {
            if (Time.time - lastTimeUseObject > deltaTimeUseObject)
            {
                transform.parent.parent.gameObject.GetComponent<Animations>().AttackAnim(this.tag);
                lastTimeUseObject = Time.time;

                Transform player = transform.parent.parent;
                List<Transform> listPlayers = new List<Transform>();// = player.parent.GetComponentsInChildren<Transform>().ToList();//get all players
                foreach (Transform pl in player.parent)//get all players
                {
                    listPlayers.Add(pl);
                }
                listPlayers = listPlayers.Where((p) => p.GetComponent<PlayerStatManager>().isDead == false).ToList();// without dead players

                //Player random
                int randomIndex = Random.Range(0, listPlayers.Count - 1);
                Transform randomPlayer = listPlayers[randomIndex] == GetComponent<PhotonView>().IsMine
                    ? listPlayers[randomIndex + 1 % listPlayers.Count]
                    : listPlayers[randomIndex];

                //Network task - get target player
                Photon.Realtime.Player networkBindRandomPlayer = PhotonNetwork.PlayerList.Where(player =>
                    player.ActorNumber == randomPlayer.gameObject.GetPhotonView().ControllerActorNr).First();
                photonView.RPC(nameof(Switch), networkBindRandomPlayer, player.position, player.rotation,
                    randomPlayer.gameObject.GetPhotonView().ViewID);
                
                // display cooldown
                PlayerStatManager playerStatManager = GetPlayerStatManager();
                playerStatManager.UpdateCooldownDisplay(lastTimeUseObject, deltaTimeUseObject, gameObject.name);


                //Local task
                Switch(randomPlayer.position, randomPlayer.rotation);
                print(randomPlayer.gameObject.GetPhotonView().ViewID);
                print(PhotonNetwork.PlayerList[0].ActorNumber);
            }
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
