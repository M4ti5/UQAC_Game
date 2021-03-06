using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

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
            // Check if the mouse was clicked over a UI element
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                //Add equipement behavior script
                if (transform.childCount > 0)
                {
                    this.transform.GetChild(0).GetComponent<Object>().Behaviour(); // utiliser l'objet
                }
            }
        }
        
        //Store equipement
        if (Input.mouseScrollDelta.y != 0 && PhotonNetwork.LocalPlayer == transform.parent.GetComponent<PhotonView>().Owner)
        {
            // Check if the mouse was clicked over a UI element
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                if (transform.parent.Find("Inventory").childCount > 0 ||
                    transform.parent.Find("Equipements").childCount > 0)
                {
                    OnStoreEquipement();
                }
            }
        }
    
    }

    public void OnStoreEquipement()
    {
        photonView.RPC(nameof(StoreEquipement), RpcTarget.AllBufferedViaServer);
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
        PlayerStatManager playerStatManager = gameObject.GetComponentInParent<PlayerStatManager>();

        //Management of handle object and inventory object
        if (storedObject == null && equipedObject != null) // Inventory object
        {
            equipedObject.transform.parent = Inventory;
            equipedObject.transform.localPosition = Vector3.zero;
            equipedObject.transform.localRotation = Quaternion.identity;
            EquipementDest.GetComponent<UseObject>().hasObject = false;
            equipedObject.GetComponent<Object>().isStored = true;
            playerStatManager.storedEquipement = equipedObject;

            if(transform.parent.GetComponent<PhotonView>().IsMine)// Cooldown display
                playerStatManager.UpdateCooldownDisplay(equipedObject.GetComponent<Object>().lastTimeUseObject, equipedObject.GetComponent<Object>().deltaTimeUseObject, equipedObject.name);

        }
        else if(storedObject != null && equipedObject == null)// handle object 
        {
            storedObject.transform.parent = storedObject.GetComponent<Object>().EquipmentDest;
            storedObject.transform.localPosition = Vector3.zero;
            storedObject.transform.localRotation = Quaternion.identity;
            storedObject.GetComponent<Object>().isStored = false;
            EquipementDest.GetComponent<UseObject>().hasObject = true;
            playerStatManager.storedEquipement = null;

            if(transform.parent.GetComponent<PhotonView>().IsMine)// Cooldown display
                playerStatManager.UpdateCooldownDisplay(storedObject.GetComponent<Object>().lastTimeUseObject, storedObject.GetComponent<Object>().deltaTimeUseObject, storedObject.name);

        }
        else if (storedObject != null && equipedObject != null)// Inventory & handle object
        {
            equipedObject.transform.parent = Inventory;
            equipedObject.transform.localPosition = Vector3.zero;
            equipedObject.transform.localRotation = Quaternion.identity;
            equipedObject.GetComponent<Object>().isStored = true;
            playerStatManager.storedEquipement = equipedObject;
            storedObject.transform.parent = storedObject.GetComponent<Object>().EquipmentDest;
            storedObject.transform.localPosition = Vector3.zero;
            storedObject.transform.localRotation = Quaternion.identity;
            storedObject.GetComponent<Object>().isStored = false;

            if(transform.parent.GetComponent<PhotonView>().IsMine)// Cooldown display
                playerStatManager.UpdateCooldownDisplay(storedObject.GetComponent<Object>().lastTimeUseObject, storedObject.GetComponent<Object>().deltaTimeUseObject, storedObject.name);

        }
    }
    
    
    
}
