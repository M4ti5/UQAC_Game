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
            if(swap.transform.tag == "Player")
            {
                Vector3 otherPlayerColor = swap.transform.GetComponent<PlayerStatManager>().playerColor;
                transform.parent.parent.GetComponent<PlayerStatManager>().setPlayerColor(otherPlayerColor.x, otherPlayerColor.y, otherPlayerColor.z, transform.parent.parent.GetComponent<PhotonView>().ViewID);
            }
        }
    }
}