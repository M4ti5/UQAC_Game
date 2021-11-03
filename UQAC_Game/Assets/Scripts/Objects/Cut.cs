using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Cut : Object
{
    public float distanceToCut = 3;


    [PunRPC]
    protected override void CustomBehaviour(){
        Vector3 cutColliderParam = new Vector3(0.5f,1,distanceToCut/2); //Half size of Collider checker box
        Collider[] HitColliders = Physics.OverlapBox(HitObj.position + new Vector3(0,0,distanceToCut/2), cutColliderParam); //offset the center of the collider checker box by half the distance
        Debug.DrawRay(HitObj.position+new Vector3(1,0.5f,0), HitObj.forward * distanceToCut, Color.yellow);
        Debug.DrawRay(HitObj.position+new Vector3(-1,0.5f,0), HitObj.forward * distanceToCut, Color.yellow);
        Debug.DrawRay(HitObj.position+new Vector3(1,-0.5f,0), HitObj.forward * distanceToCut, Color.yellow);
        Debug.DrawRay(HitObj.position+new Vector3(-1,-0.5f,0), HitObj.forward * distanceToCut, Color.yellow);
        Debug.Log(HitColliders.Length);
        if(HitColliders.Length != 0){
            Debug.Log("Did cut");
            
            Debug.Log(cutColliderParam);
            
        }
        else{
            Debug.Log("Did not cut");
        }
        
    }

    

}
