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
     /*   Vector3 cutColliderParam = new Vector3(0.1f,0.1f,maxDistance/2); //Half size of Collider checker box
        Collider[] HitColliders = Physics.OverlapBox(HitObj.position + new Vector3(0,0,maxDistance/2), cutColliderParam); //offset the center of the collider checker box by half the distance
        Debug.DrawRay(HitObj.position+new Vector3(0.1f,0.1f,0), HitObj.forward * maxDistance, Color.yellow);
        //Debug.DrawRay(HitObj.position+new Vector3(-0.1f,0.1f,0), HitObj.forward * maxDistance, Color.yellow);
        //Debug.DrawRay(HitObj.position+new Vector3(0.1f,-0.1f,0), HitObj.forward * maxDistance, Color.yellow);
        //Debug.DrawRay(HitObj.position+new Vector3(-0.1f,-0.1f,0), HitObj.forward * maxDistance, Color.yellow);
        //Debug.Log(HitColliders.Length);
        if(HitColliders.Length != 0){
            Debug.Log("Did hit");
            Debug.Log(HitColliders.Length);
            */
            /*foreach (Collider c in HitColliders)
            {
                Debug.Log(c.tag);
                if (c.tag == "Player")
                {
                    c.GetComponent<PlayerStatManager>().currentHP -= damage;
                    //Debug.Log(c.GetComponent<PhotonView>().IsMine);
                    new WaitForSeconds(1);
                    Debug.Log("hit player");
                }
            }*/
            
           /* 
        }
        else{
            Debug.Log("Did not hit");
        }*/
        if(Physics.Raycast(HitObj.position, HitObj.forward, out hit, maxDistance)){
            //Debug.DrawRay(HitObj.position, HitObj.forward * hit.distance, Color.yellow);
    
            if(hit.transform.tag == "Player"){
                hit.transform.GetComponent<PlayerStatManager>().TakeDamage(damage);
                //take damage
            }
        }
        /*else
        {
            Debug.DrawRay(HitObj.position, HitObj.forward * 1000, Color.red);
            Debug.Log("Did not Hit");
        }*/
    }
}
