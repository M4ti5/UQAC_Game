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
            if(swap.transform.tag == "Player" && swap.transform.GetComponent<PlayerStatManager>().isDead == false)// si le joueur n'est pas d�j� mort
            {
                if (player.GetComponent<PhotonView>().IsMine)
                {
                    Vector3 otherPlayerColor = swap.transform.GetComponent<PlayerStatManager>().playerColor;
                    //transform.parent.parent.GetComponent<PlayerStatManager>().setPlayerColor(otherPlayerColor.x, otherPlayerColor.y, otherPlayerColor.z, transform.parent.parent.GetComponent<PhotonView>().ViewID);
                    photonView.RPC(nameof(setPlayerColor), RpcTarget.AllBuffered, otherPlayerColor.x,
                        otherPlayerColor.y, otherPlayerColor.z,
                        transform.parent.parent.GetComponent<PhotonView>().ViewID);
                    //ObjectUsed();
                    StartCoroutine(WaitEndAnimation(transform.parent.parent, "inShoot"));
                }
            }
        }
    }

    [PunRPC]
    private void setPlayerColor(float _r, float _g, float _b, int idPlayer)
    {
        Transform player = FindPlayerByID(idPlayer);
        player.GetComponent<PlayerStatManager>().setPlayerColor(_r, _g, _b, idPlayer);
    }
}