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
    
            if(hit.transform.tag == "Player"){
                hit.transform.GetComponent<PlayerStatManager>().TakeDamage(damage);
               
            }
        }
        
    }
}
