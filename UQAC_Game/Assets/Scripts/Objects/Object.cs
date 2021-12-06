using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using UnityEngine;

public class Object : MonoBehaviourPun
{
    public float distanceToHeld;
    public GameObject allPlayers;
    public GameObject allObjects;

    public bool isHeld = false;
    public bool isStored = false;
    public Transform HitObj;
    public Transform EquipmentDest;
    public Transform player;
    
    public float lastTimeUseObject;
    public float deltaTimeUseObject = 10;
    

    // Start is called before the first frame update
    void Start()
    {
        allObjects = GameObject.Find("Objects");
        allPlayers = GameObject.Find("Players");
        name = tag;
    }

    public void Init()
    {
        Start();
        transform.parent = allObjects.transform;
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
                if (allPlayers.transform.GetChild(i).GetComponent<PlayerStatManager>().isDead == false)
                {
                    (bool _isReachable, float _dist) = IsReachable(gameObject.transform,
                        allPlayers.transform.GetChild(i), distanceToHeld);
                    if (_isReachable && _dist < minDistance)
                    {
                        minDistance = _dist;
                        grabberPlayerId = i;
                        if (allPlayers.transform.GetChild(i).GetComponent<PhotonView>().IsMine)
                            break;
                    }
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
        if(allPlayers == null)
            Start();
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
        playerStatManager.UpdateCooldownDisplay(lastTimeUseObject, deltaTimeUseObject, gameObject.name);
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
            //GetComponent<BoxCollider>().enabled = false;
            foreach (Collider coll in transform.GetComponentsInChildren<Collider>())
            {
                coll.enabled = false;
            }
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
            EquipmentDest.GetComponent<UseObject>().hasObject = true;
            HitObj = player.transform.Find("HitPos");
            isHeld = true;
        }
    }

    // main function to synchronize DesequipmentTriggered
    public void OnDesequipmentTriggered()
    {
        photonView.RPC(nameof(DesequipmentTriggered), RpcTarget.AllBuffered);
        PlayerStatManager playerStatManager = GetPlayerStatManager();
        playerStatManager.UpdateCooldownDisplay(lastTimeUseObject, deltaTimeUseObject, "");
    }
    //Desequipe the object to the Equipment destination
    [PunRPC]
    protected void DesequipmentTriggered()
    {
        transform.parent = allObjects.transform;
        if(EquipmentDest != null)
            transform.position = EquipmentDest.parent.Find("Inventory").position;
        //GetComponent<BoxCollider>().enabled = true;
        foreach (Collider coll in transform.GetComponentsInChildren<Collider>() )
        {
            coll.enabled = true;
        }
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
        //GetComponent<BoxCollider>().enabled = true;
        foreach (Collider coll in transform.GetComponentsInChildren<Collider>() )
        {
            coll.enabled = true;
        }
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
        float dist = Vector3.Distance(objectA.position - new Vector3(0 , objectA.position.y , 0) , (playerA.position - new Vector3(0, playerA.position.y, 0)));
        if (dist < range)
        {
            float angle = Vector3.Angle(playerA.forward,
                (objectA.position - new Vector3(0, objectA.position.y, 0)) - (playerA.position - new Vector3(0, playerA.position.y, 0)));

            if (angle <= Mathf.Abs(30))
            {
                return (true, dist);
            }
        }
        
        return (false, dist);

    }

    public virtual void Behaviour()
    {
        if (player.GetComponent<PhotonView>().IsMine == true)
        {
            // wait cooldown
            if (Time.time - lastTimeUseObject > deltaTimeUseObject)
            {
                lastTimeUseObject = Time.time;
                
                
                //Animation Object
                transform.parent.parent.gameObject.GetComponent<Animations>().AttackAnim(this.tag);
                
                // display cooldown
                PlayerStatManager playerStatManager = GetPlayerStatManager();
                playerStatManager.UpdateCooldownDisplay(lastTimeUseObject, deltaTimeUseObject, gameObject.name);
                
                photonView.RPC(nameof(CustomBehaviour), RpcTarget.AllBuffered); // faire l'action pour tous les clients
                
            }
        }
    }

    [PunRPC]
    protected virtual void CustomBehaviour(){
        Debug.Log("do something");
        ObjectUsed();
    }

    public void ObjectUsed()
    {
        this.transform.parent.GetComponent<UseObject>().hasObject = false;
        this.DestroyObject(PhotonNetwork.LocalPlayer); // d�truire l'objet
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

    protected PlayerStatManager GetPlayerStatManager()
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
    
    
    public Transform FindPlayerByID(int id)
    {
        foreach (Transform child in allPlayers.transform)
        {
            if (child.GetComponent<PhotonView>().ViewID == id)
            {
                return child;
            }
        }
        return null;
    }
    
    public IEnumerator WaitEndAnimation (Transform hitTransform, string var) 
    {
        transform.parent.parent.GetComponent<PlayerStatManager>().HideEquipedWeaponDisplay();
        yield return new WaitWhile(() => hitTransform.GetComponent<Animator>().GetBool(var) == true);
        transform.parent.parent.GetComponent<PlayerStatManager>().HideEquipedWeaponDisplay();
        ObjectUsed();
    }
}
