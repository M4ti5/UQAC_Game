using System.Collections;
using Photon.Pun;
using UnityEngine;

/**
 * Mother Class of an Equipment
 * Give the general behavior of an object
 */
public class Object : MonoBehaviourPun
{
    public float distanceToHeld;
    
    public GameObject allPlayers;
    public GameObject allObjects;
    
    public bool isHeld = false;
    public bool isStored = false;
    //position where the boxes or rays are cast in the customBahavior of object
    public Transform HitObj;
    //position of the Equipment (player right hand)
    public Transform EquipmentDest;
    //player that is currently the owner of the object
    public Transform player;
    
    //Timer to use the object
    public float lastTimeUseObject;
    public float deltaTimeUseObject = 10;
    

    // Start is called before the first frame update
    void Start()
    {
        allObjects = GameObject.Find("Objects");
        allPlayers = GameObject.Find("Players");
        name = tag;
    }

    //initialise object when instanciated dynamiccaly
    public void Init()
    {
        Start();
        transform.parent = allObjects.transform;
    }

    // Update is called once per frame
    void Update()
    {

        // E = take object
        //when E pressed, Equip object :
        if (Input.GetKeyUp(KeyCode.E) && isHeld == false)
        {
            int allPlayersCount = allPlayers.transform.childCount;
            int grabberPlayerId = -1;
            float minDistance = float.PositiveInfinity;
            
            //Check distance between object and all players
            for (int i = 0; i < allPlayersCount; i++)
            {
                //check if player is still alive
                if (allPlayers.transform.GetChild(i).GetComponent<PlayerStatManager>().isDead == false)
                {
                    (bool _isReachable, float _dist) = IsReachable(gameObject.transform,
                        allPlayers.transform.GetChild(i), distanceToHeld);
                    //if player can reach object and is the nearest, store ID
                    if (_isReachable && _dist < minDistance)
                    {
                        minDistance = _dist;
                        grabberPlayerId = i;
                        if (allPlayers.transform.GetChild(i).GetComponent<PhotonView>().IsMine)
                            break;
                    }
                }
            }
            //if we store a player ID Equip object in player's hand
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
        
        //if object hold and player press A, unEquip object
        if (Input.GetKeyUp(KeyCode.A) && isHeld == true && PhotonNetwork.LocalPlayer == player.GetComponent<PhotonView>().Owner && isStored == false)
        {
            OnDesequipmentTriggered();
        }

    }
    
    //Find a player with its Photon viewID
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

    //Find a player EquipementDest using its Photon ViewID
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

    // main function to synchronize EquipmentTriggered
    public void OnEquipmentTriggered(Transform _player)
    {
        //synchronization for all player : a player takes an object
        photonView.RPC(nameof(EquipmentTriggered), RpcTarget.AllBufferedViaServer, _player.GetComponent<PhotonView>().ViewID, PhotonNetwork.LocalPlayer);
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
        //synchronization for all player : a player throws an object to the ground
        photonView.RPC(nameof(DesequipmentTriggered), RpcTarget.AllBufferedViaServer);
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
    
    // main function to synchronize DesequipmentTriggered (using photon)
    public void OnDesequipmentTriggeredWhenPlayerLeaveGame()
    {
        
        //synchronization for all player : a player throws an object when leaving the game
        photonView.RPC(nameof(DesequipmentTriggeredWhenPlayerLeaveGame), RpcTarget.AllBufferedViaServer);
    }
    
    //Desequipe the object to the Equipment destination when player leaves game
    [PunRPC]
    protected void DesequipmentTriggeredWhenPlayerLeaveGame()
    {
        foreach (Collider coll in transform.GetComponentsInChildren<Collider>() )
        {
            coll.enabled = true;
        }
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
        transform.parent = allObjects.transform;
        isHeld = false;
        isStored = false;
        HitObj = allObjects.transform;
    }

    //Check if object is not too far from player and if it's in front of the player
    //Reachable is in front of the player from an angle of 30 degrees to a distance set before
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

    //main behavior function of an object (common to all objects)
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
                
                //Do the custom behavior for every player in the room
                photonView.RPC(nameof(CustomBehaviour), RpcTarget.AllBufferedViaServer);
                
            }
        }
    }

    //function to be overriden
    //Describe the specific behavior of an object
    [PunRPC]
    protected virtual void CustomBehaviour(){
        Debug.Log("do something");
        ObjectUsed();
    }

    //Destroy object
    public void ObjectUsed()
    {
        this.transform.parent.GetComponent<UseObject>().hasObject = false;
        this.DestroyObject(PhotonNetwork.LocalPlayer);
    }

    //Synchronize the destruction of an object
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
                
                //synchronization : Destroy an object (managed by the master client)
                photonView.RPC(nameof(DestroyObject), RpcTarget.MasterClient, null);
            }
        }
    }

    //Find the playerStatManager script of the player owner of the object
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

    //coroutine that starts Hit animation and then destroy object
    public IEnumerator WaitEndAnimation (Transform hitTransform, string var) 
    {
        transform.parent.parent.GetComponent<PlayerStatManager>().HideEquipedWeaponDisplay();
        yield return new WaitWhile(() => hitTransform.GetComponent<Animator>().GetBool(var) == true);
        transform.parent.parent.GetComponent<PlayerStatManager>().HideEquipedWeaponDisplay();
        ObjectUsed();
    }
}
