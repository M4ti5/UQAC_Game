using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class UseObject : MonoBehaviourPun
{

    public bool hasObject = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start UseObject script");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && hasObject)
        {
            //add equipement behavior script
            this.transform.GetChild(0).GetComponent<Object>().Behaviour();// utiliser l'objet
            //this.transform.GetChild(0).GetComponent<Object>().DestroyObject(PhotonNetwork.LocalPlayer);// détruire l'objet
            //hasObject = false;
        }
        
        //Store equipement
        if (Input.mouseScrollDelta.y != 0 && PhotonNetwork.LocalPlayer == transform.parent.GetComponent<PhotonView>().Owner)
        {
            Debug.Log("mouse wheel");
            //Get gameobjetcs
            GameObject storedObject = null;
            if (transform.parent.Find("Inventory").childCount > 0)
            {
                storedObject = transform.parent.Find("Inventory").GetChild(0).gameObject;
            }
            GameObject equipedObject = null;
            if (transform.childCount > 0)
            {
                equipedObject = transform.GetChild(0).gameObject;
            }

            
            if (storedObject == null && equipedObject != null)
            {
                equipedObject.transform.parent = transform.parent.Find("Inventory");
                hasObject = false;
                equipedObject.GetComponent<Object>().isStored = true;
            }
            else if(storedObject != null && equipedObject == null)
            {
                storedObject.transform.parent = storedObject.GetComponent<Object>().EquipmentDest;
                storedObject.GetComponent<Object>().isStored = false;
                hasObject = true;
            }
            else if (storedObject != null && equipedObject != null)
            {
                equipedObject.transform.parent = transform.parent.Find("Inventory");
                equipedObject.GetComponent<Object>().isStored = true;
                storedObject.transform.parent = storedObject.GetComponent<Object>().EquipmentDest;
                storedObject.GetComponent<Object>().isStored = false;
            }
            
        }
    
    }
    
    
    
    
}
