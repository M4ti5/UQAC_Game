using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using UnityEngine;

public class UseObject : MonoBehaviourPun
{

    public bool hasObject = false;
    public bool canUseObject = true;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start UseObject script");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt) == false && Input.GetMouseButton(0) && hasObject && canUseObject)
        {
            //add equipement behavior script
            if (transform.childCount > 0)
            {
                this.transform.GetChild(0).GetComponent<Object>().Behaviour(); // utiliser l'objet
                this.transform.GetChild(0).GetComponent<Object>().DestroyObject(PhotonNetwork.LocalPlayer); // détruire l'objet
                hasObject = false;
            }
        }
        
        //Store equipement
        if (Input.mouseScrollDelta.y != 0 && PhotonNetwork.LocalPlayer == transform.parent.GetComponent<PhotonView>().Owner)
        {
            //Debug.Log("mouse wheel");
            //Get gameobjetcs
            OnStoreEquipement();
            //this.transform.GetChild(0).GetComponent<Object>().OnStoreEquipement(this.transform.GetChild(0).GetComponent<Object>().player);

        }
    
    }

    public void OnStoreEquipement()
    {
        photonView.RPC(nameof(StoreEquipement), RpcTarget.AllBuffered);
    }
    
    [PunRPC]
    protected void StoreEquipement()
    {
        Transform Inventory = transform.parent.Find("Inventory");
        Transform EquipementDest = transform.parent.Find("Equipements");
        GameObject storedObject = null;
        if (Inventory.childCount > 0)
        {
            storedObject = Inventory.GetChild(0).gameObject;
        }
        
        GameObject equipedObject = null;
        if (EquipementDest.childCount > 0)
        {
            equipedObject = EquipementDest.GetChild(0).gameObject;
        }

        if (storedObject == null && equipedObject != null)
        {
            equipedObject.transform.parent = Inventory;
            EquipementDest.GetComponent<UseObject>().hasObject = false;
            equipedObject.GetComponent<Object>().isStored = true;
            transform.parent.GetComponent<PlayerStatManager>().storedEquipement = equipedObject;
        }
        else if(storedObject != null && equipedObject == null)
        {
            storedObject.transform.parent = storedObject.GetComponent<Object>().EquipmentDest;
            storedObject.GetComponent<Object>().isStored = false;
            EquipementDest.GetComponent<UseObject>().hasObject = true;
            transform.parent.GetComponent<PlayerStatManager>().storedEquipement = null;
        }
        else if (storedObject != null && equipedObject != null)
        {
            equipedObject.transform.parent = Inventory;
            equipedObject.GetComponent<Object>().isStored = true;
            transform.parent.GetComponent<PlayerStatManager>().storedEquipement = equipedObject;
            storedObject.transform.parent = storedObject.GetComponent<Object>().EquipmentDest;
            storedObject.GetComponent<Object>().isStored = false;
        }

    }
    
    
    
    
}
