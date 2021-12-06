using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Gun : Object
{
    private RaycastHit hit;
    public int maxDistance = 20;

    public int damage;

    [PunRPC]
    protected override void CustomBehaviour(){
     
        if(Physics.Raycast(HitObj.position, HitObj.forward, out hit, maxDistance)){
    
            if(hit.transform.tag == "Player" && hit.transform.GetComponent<PlayerStatManager>().isDead == false)// si le joueur n'est pas déjà mort
            {
                if (player.GetComponent<PhotonView>().IsMine)
                {
                    //hit.transform.GetComponent<PlayerStatManager>().TakeDamage(damage, hit.transform.GetComponent<PhotonView>().ViewID);
                    photonView.RPC(nameof(TakeDamage), RpcTarget.AllBuffered, damage,
                        hit.transform.GetComponent<PhotonView>().ViewID);
                    //ObjectUsed();
                    StartCoroutine(WaitEndAnimation(hit.transform, "inShoot"));
                }
            }
        }
        
    }

    [PunRPC]
    private void TakeDamage(int damage, int viewId)
    {
        Transform player = FindPlayerByID(viewId);
        player.GetComponent<PlayerStatManager>().TakeDamage(damage, viewId);
    }
}
