using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Object
{
    private RaycastHit hit;
    public int maxDistance = 20;
    public override void behaviour(){
        Vector3 cutColliderParam = new Vector3(0.1f,0.1f,maxDistance/2); //Half size of Collider checker box
        Collider[] HitColliders = Physics.OverlapBox(HitObj.position + new Vector3(0,0,maxDistance/2), cutColliderParam); //offset the center of the collider checker box by half the distance
        Debug.DrawRay(HitObj.position+new Vector3(0.1f,0.1f,0), HitObj.forward * maxDistance, Color.yellow);
        Debug.DrawRay(HitObj.position+new Vector3(-0.1f,0.1f,0), HitObj.forward * maxDistance, Color.yellow);
        Debug.DrawRay(HitObj.position+new Vector3(0.1f,-0.1f,0), HitObj.forward * maxDistance, Color.yellow);
        Debug.DrawRay(HitObj.position+new Vector3(-0.1f,-0.1f,0), HitObj.forward * maxDistance, Color.yellow);
        Debug.Log(HitColliders.Length);
        if(HitColliders.Length != 0){
            Debug.Log("Did hit");
            
        }
        else{
            Debug.Log("Did not cut");
        }
        //if(Physics.Raycast(gunPos, gunDir, out hit, maxDistance)){
        //    Debug.DrawRay(gunPos, gunDir * hit.distance, Color.yellow);
        //    Debug.Log("Did Hit");
        //    if(hit.transform.tag == "Player"){
        //        Debug.Log("hit player");
                //take damage
         //   }
        //}
        //else
        //{
        //    Debug.DrawRay(gunPos, gunDir * 1000, Color.red);
        //    Debug.Log("Did not Hit");
        //}
    }
}
