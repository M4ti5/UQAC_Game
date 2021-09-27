using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{   
    public bool isHeld = false;
    public Transform EquipmentDest;
    public Transform player;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        //Get distance between player and object (works for only one player)
        float dist = Vector3.Distance(gameObject.transform.position, player.position);
        
        bool reachable = isReachable(gameObject.transform,player,2);

        if(Input.GetKeyUp(KeyCode.A) && isHeld == false && reachable){
            OnEquipmentTriggered();
            isHeld = true;
           
        }
        else if(Input.GetKeyUp(KeyCode.A) && isHeld == true){
            OnDesequipmentTriggered();
            isHeld = false;
            
        }
        
    }

    //Equipe the object to the Equipment destination
    void OnEquipmentTriggered(){
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        this.transform.position = EquipmentDest.position;
        this.transform.parent = GameObject.Find("Equipment").transform;
    }

    //Desequipe the object to the Equipment destination
    void OnDesequipmentTriggered(){
        this.transform.parent = null;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<BoxCollider>().enabled = true;
    }

    //Check if object is not too far from player and if it's in front of the player
    bool isReachable(Transform objectA, Transform playerA, float range){
        float dist = Vector3.Distance(objectA.position, playerA.position);
        float angle = Vector3.Angle(playerA.position, objectA.position);

        if(dist < range && angle < Mathf.Abs(45)){
            return true;
        }
        else{
            return false;
        }

    }
}
