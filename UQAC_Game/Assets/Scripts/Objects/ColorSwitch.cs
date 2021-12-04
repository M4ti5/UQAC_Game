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
            if(HitObj.transform.tag == "Player")
            {
                transform.parent.parent.GetComponent<PlayerStatManager>().playerColor = HitObj.transform.GetComponent<PlayerStatManager>().playerColor;
            }
        }
    }
}