using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    public float distanceToHeld;
    public GameObject allPlayers;
    public GameObject allObjects;

    protected bool isHeld = false;
    protected Transform HitObj;
    protected Transform EquipmentDest;
    protected Transform player;

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
        
            for(int i=0; i<allPlayersCount; i++){
                if(isReachable(gameObject.transform, allPlayers.transform.GetChild(i), distanceToHeld)){
                    grabberPlayerId = i;
                }
            }
            if(grabberPlayerId >= 0){
                player = allPlayers.transform.GetChild(grabberPlayerId);
                EquipmentDest = player.transform.Find("Equipements");
                if(EquipmentDest.GetComponent<UseObject>().hasObject == false){
                    OnEquipmentTriggered();
                }
            }

        }

        if (Input.GetKeyUp(KeyCode.A) && isHeld == true)
        {
            OnDesequipmentTriggered();
        }

    }

    //Equipe the object to the Equipment destination
    void OnEquipmentTriggered()
    { 
        this.transform.parent = EquipmentDest;
        this.transform.position = EquipmentDest.position;
        this.transform.rotation = EquipmentDest.rotation;
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;
        EquipmentDest.GetComponent<UseObject>().hasObject = true;
        HitObj = player.transform.Find("HitPos");
        Debug.Log(HitObj);
        isHeld = true;

    }

    //Desequipe the object to the Equipment destination
    void OnDesequipmentTriggered()
    {
        this.transform.parent = allObjects.transform;
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
        EquipmentDest.GetComponent<UseObject>().hasObject = false;
        isHeld = false;
        HitObj = allObjects.transform;
    }

    //Check if object is not too far from player and if it's in front of the player
    bool isReachable(Transform objectA, Transform playerA, float range)
    {
        float dist = Vector3.Distance(objectA.position, playerA.position);
        float angle = Vector3.Angle(playerA.forward, objectA.position - playerA.position);

        if (dist < range && angle <= Mathf.Abs(30))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public virtual void behaviour(){
        Debug.Log("do something");
    }

    
}
