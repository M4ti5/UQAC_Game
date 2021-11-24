using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Object : MonoBehaviourPun
{
    public float distanceToHeld;
    public GameObject allPlayers;
    public GameObject allObjects;

    protected bool isHeld = false;
    public bool isStored = false;
    protected Transform HitObj;
    public Transform EquipmentDest;
    public Transform player;
    
    private float lastTimeUseObject;
    private float deltaTimeUseObject = 10;
    

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start Object script");
    }

    // Update is called once per frame
    void Update()
    {
        //Get distance between player and object (works for only one player)
        //float dist = Vector3.Distance(gameObject.transform.position, player.transform.position);


        //bool reachable = isReachable(gameObject.transform, player.transform, distanceToHeld);

        //EquipmentDest = player.transform.Find("Equipements");

        if (Input.GetKeyUp(KeyCode.E) && isHeld == false)
        {
            int allPlayersCount = allPlayers.transform.childCount;
            int grabberPlayerId = -1;
            float minDistance = float.PositiveInfinity;

            for (int i = 0; i < allPlayersCount; i++)
            {
                (bool _isReachable, float _dist) = IsReachable(gameObject.transform, allPlayers.transform.GetChild(i), distanceToHeld);
                if (_isReachable && _dist < minDistance)
                {
                    minDistance = _dist;
                    grabberPlayerId = i;
                    if (allPlayers.transform.GetChild(i).GetComponent<PhotonView>().IsMine)
                        break;
                }
            }
            if (grabberPlayerId >= 0)
            {
                player = allPlayers.transform.GetChild(grabberPlayerId);
                EquipmentDest = player.transform.Find("Equipements");
                if (EquipmentDest.GetComponent<UseObject>().hasObject == false && player.GetComponent<PhotonView>().IsMine)
                {
                    OnEquipmentTriggered(player);
                }
            }

        }

        if (Input.GetKeyUp(KeyCode.A) && isHeld == true && PhotonNetwork.LocalPlayer == player.GetComponent<PhotonView>().Owner && isStored == false)
        {
            OnDesequipmentTriggered();
        }

    }

    Transform FindEquipmentsPlayerByID(int id)
    {
        foreach (Transform child in allPlayers.transform)
        {
            if (child.GetComponent<PhotonView>().ViewID == id)
            {
                player = child;
                return child.Find("Equipements");
            }
        }
        return null;
    }
    
    Transform FindInventoryPlayerByID(int id)
    {
        foreach (Transform child in allPlayers.transform)
        {
            if (child.GetComponent<PhotonView>().ViewID == id)
            {
                player = child;
                return child.Find("Inventory");
            }
        }
        return null;
    }

    // main function to synchronize EquipmentTriggered
    public void OnEquipmentTriggered(Transform _player)
    {
        photonView.RPC(nameof(EquipmentTriggered), RpcTarget.AllBuffered, _player.GetComponent<PhotonView>().ViewID, PhotonNetwork.LocalPlayer);
        PlayerStatManager playerStatManager = GetPlayerStatManager();
        playerStatManager.UpdateEquipedWeaponDisplay();
    }

    //Equipe the object to the Equipment destination
    [PunRPC]
    protected void EquipmentTriggered(int id, Photon.Realtime.Player _player)
    {
        EquipmentDest = FindEquipmentsPlayerByID(id);
        if (EquipmentDest != null)
        {
            photonView.TransferOwnership(_player);
            transform.parent = EquipmentDest;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
            EquipmentDest.GetComponent<UseObject>().hasObject = true;
            HitObj = player.transform.Find("HitPos");
            Debug.Log(HitObj);
            isHeld = true;
        }
    }

    // main function to synchronize DesequipmentTriggered
    public void OnDesequipmentTriggered()
    {
        photonView.RPC(nameof(DesequipmentTriggered), RpcTarget.AllBuffered);
        PlayerStatManager playerStatManager = GetPlayerStatManager();
        playerStatManager.UpdateEquipedWeaponDisplay();
    }
    //Desequipe the object to the Equipment destination
    [PunRPC]
    protected void DesequipmentTriggered()
    {
        transform.parent = allObjects.transform;
        transform.position = EquipmentDest.parent.Find("Inventory").position;
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
        EquipmentDest.GetComponent<UseObject>().hasObject = false;
        isHeld = false;
        HitObj = allObjects.transform;
    }
    
    // main function to synchronize DesequipmentTriggered
    public void OnDesequipmentTriggeredWhenPlayerLeaveGame()
    {
        photonView.RPC(nameof(DesequipmentTriggeredWhenPlayerLeaveGame), RpcTarget.AllBuffered);
    }
    
    //Desequipe the object to the Equipment destination
    [PunRPC]
    protected void DesequipmentTriggeredWhenPlayerLeaveGame()
    {
        //transform.position = EquipmentDest.parent.Find("Inventory").position;
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
        transform.parent = allObjects.transform;
        //EquipmentDest.GetComponent<UseObject>().hasObject = false;
        isHeld = false;
        isStored = false;
        HitObj = allObjects.transform;
    }

    //Check if object is not too far from player and if it's in front of the player
    (bool, float) IsReachable(Transform objectA, Transform playerA, float range)
    {
        float dist = Vector3.Distance(objectA.position, playerA.position);
        float angle = Vector3.Angle(playerA.forward, objectA.position - playerA.position);

        if (dist < range && angle <= Mathf.Abs(30))
        {
            return (true, dist);
        }
        else
        {
            return (false, dist);
        }

    }

    public void Behaviour()
    {
        // wait cooldown
        if (Time.time - lastTimeUseObject > deltaTimeUseObject)
        {
            lastTimeUseObject = Time.time;
            photonView.RPC(nameof(CustomBehaviour), RpcTarget.AllBuffered); // faire l'action pour tous les clients
            PlayerStatManager playerStatManager = GetPlayerStatManager();
            playerStatManager.UpdateCooldownDisplay(deltaTimeUseObject);
        }
    }

    [PunRPC]
    protected virtual void CustomBehaviour(){
        Debug.Log("do something");
        ObjectUsed();
    }

    protected void ObjectUsed()
    {
        this.transform.parent.GetComponent<UseObject>().hasObject = false;
        this.DestroyObject(PhotonNetwork.LocalPlayer); // détruire l'objet
    }

    [PunRPC]
    public void DestroyObject(Photon.Realtime.Player localPlayer)
    {
        if (localPlayer == photonView.Owner || localPlayer == null)
        {
            if (photonView.Owner == PhotonNetwork.LocalPlayer)
            {
                PhotonNetwork.Destroy(photonView);
            }
            else
            {
                photonView.RPC(nameof(DestroyObject), RpcTarget.MasterClient, null);
            }
        }
    }

    /*
    public void OnStoreEquipement(Transform _player)
    {
        //Debug.Log("oui");
        photonView.RPC(nameof(StoreEquipement), PhotonNetwork.LocalPlayer, _player.GetComponent<PhotonView>().ViewID, PhotonNetwork.LocalPlayer);
        //StoreEquipement();
        //Debug.Log("ouiii");
    }
    
    [PunRPC]
    protected void StoreEquipement(int id, Photon.Realtime.Player _player)
    {
        photonView.TransferOwnership(_player);
        EquipmentDest = FindEquipmentsPlayerByID(id);
        Transform Inventory = FindInventoryPlayerByID(id);
        
        GameObject storedObject = null;
        if (Inventory.childCount > 0)
        {
            storedObject = Inventory.GetChild(0).gameObject;
        }
        
        GameObject equipedObject = null;
        if (EquipmentDest.childCount > 0)
        {
            equipedObject = EquipmentDest.GetChild(0).gameObject;
        }

        if (storedObject == null && equipedObject != null)
        {
            equipedObject.transform.parent = Inventory;
            transform.parent.GetComponent<UseObject>().hasObject = false;
            isStored = true;
        }
        else if(storedObject != null && equipedObject == null)
        {
            storedObject.transform.parent = EquipmentDest;
            isStored = false;
            transform.parent.GetComponent<UseObject>().hasObject = true;
        }
        else if (storedObject != null && equipedObject != null)
        {
            equipedObject.transform.parent = Inventory;
            equipedObject.GetComponent<Object>().isStored = true;
            storedObject.transform.parent = EquipmentDest;
            storedObject.GetComponent<Object>().isStored = false;
        }

    }*/

    private PlayerStatManager GetPlayerStatManager()
    {
        PlayerStatManager playerStatManager = GetComponent<PlayerStatManager>();
        int playerCount = allPlayers.transform.childCount;
        for (int i = 0; i < playerCount; i++)
        {
            if (allPlayers.transform.GetChild(i).GetComponent<PhotonView>().IsMine)
            {
                playerStatManager = allPlayers.transform.GetChild(i).transform.GetComponent<PlayerStatManager>();
            }
        }
        return playerStatManager;
    }
}
