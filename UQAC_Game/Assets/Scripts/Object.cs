using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{   
    private bool isHeld = false;
    public Transform EquipmentDest;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyUp(KeyCode.A) && isHeld == false){
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
}
