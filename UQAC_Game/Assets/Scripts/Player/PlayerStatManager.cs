using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerStatManager : MonoBehaviourPun
{
    public int currentHP;
    public int stamina;
    
    public GameObject allObjects;
    public GameObject interractionDisplay;
    public TextMeshProUGUI interactionText;
    public GameObject inventoryDisplay;
    public TextMeshProUGUI inventoryText;

    public GameObject storedEquipement;
    

    
    public float distanceToHold = 5;
    // Start is called before the first frame update
    void Start()
    {
        currentHP = 100;
        allObjects = GameObject.Find("Objects");
        
        interractionDisplay = GameObject.Find("TakeObject");
        interactionText = interractionDisplay.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        inventoryDisplay = GameObject.Find("InventoryDisplay");
        inventoryText = inventoryDisplay.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        List<(GameObject, float)> _reachableObjects = reachableObjects();
        GameObject nearestObj = findNearestObj(_reachableObjects);
        if (GetComponent<PhotonView>().IsMine)
        {
            if (_reachableObjects.Count > 0)
            {
                interractionDisplay.SetActive(true);
                interactionText.text = "Take " + nearestObj.name + "\nPress E";
            }
            else
            {
                interractionDisplay.SetActive(false);
            }
        }

        if (storedEquipement != null)
        {   
            inventoryDisplay.SetActive(true);
            inventoryText.text = storedEquipement.name;
        }
        else
        {
            inventoryDisplay.SetActive(false);
            inventoryText.text = "";
        }
        
        
    }

    List<(GameObject, float)> reachableObjects()
    {
        List<(GameObject, float)> _reachableObjects = new List<(GameObject, float)>();
        int allObjectCount = allObjects.transform.childCount;
        for (int i = 0; i < allObjectCount; i++)
        {
            (bool _isReachable, float _dist) =
                IsReachable(allObjects.transform.GetChild(i), gameObject.transform, distanceToHold);
            if (_isReachable)
            {
                _reachableObjects.Add((allObjects.transform.GetChild(i).gameObject,_dist));
            }
        }

        return _reachableObjects;
    }
    
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

    GameObject findNearestObj(List<(GameObject, float)> _reachableObjects)
    {
        float nearestObjDist = -1;
        GameObject nearestObj = null;
        foreach ((GameObject o, float dist) in _reachableObjects)
        {
            //first obj
            if (nearestObjDist == -1 || dist < nearestObjDist)
            {
                nearestObj = o;
                nearestObjDist = dist;
            }
            
        }

        return nearestObj;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            Debug.Log("Game Over");
        }
    }

    
}
