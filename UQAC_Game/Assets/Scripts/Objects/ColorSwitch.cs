using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ColorSwitch : Object
{
    private RaycastHit swap;
    public int maxDistance = 20;

    [PunRPC]
    protected override void CustomBehaviour()
    {
        if (Physics.Raycast(HitObj.position,HitObj.forward, out swap, maxDistance))
        {
            if(swap.transform.tag == "Player" && swap.transform.GetComponent<PlayerStatManager>().isDead == false)// si le joueur n'est pas déjà mort
            {
                Vector3 otherPlayerColor = swap.transform.GetComponent<PlayerStatManager>().playerColor;
                //transform.parent.parent.GetComponent<PlayerStatManager>().setPlayerColor(otherPlayerColor.x, otherPlayerColor.y, otherPlayerColor.z, transform.parent.parent.GetComponent<PhotonView>().ViewID);
                photonView.RPC(nameof(PlayerStatManager.setPlayerColor), RpcTarget.AllBuffered, otherPlayerColor.x, otherPlayerColor.y, otherPlayerColor.z, transform.parent.parent.GetComponent<PhotonView>().ViewID);
                //ObjectUsed();
                StartCoroutine(WaitEndAnimation( transform.parent.parent, "inShoot"));
            }
        }
    }
}