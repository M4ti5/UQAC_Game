using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerStatManager : MonoBehaviourPun
{
    public GameObject thisPlayer;
    
    public int currentHP;
    public int stamina;
    public int hpMax;

    public GameObject canvas;
    public PersonalScore personalScore;
    public GlobalScore globalScore;
    
    public GameObject allObjects;
    public GameObject interractionDisplay;
    public TextMeshProUGUI interactionText;
    public GameObject inventoryDisplay;
    public TextMeshProUGUI inventoryText;

    public GameObject storedEquipement;
    public GameObject equipements;
    public GameObject inventory;

    public bool criminal = false;
    public int selectedFilter = 0;
    
    public float distanceToHold = 5;
    public List<GameObject> objectPrefabListToInstantiate;
    private bool findAllObjects = false;
    
    // Start is called before the first frame update
    void Start()
    {
        currentHP = 100;
        hpMax = 100;
        thisPlayer = this.gameObject;

        StartCoroutine(GetGameObjects());


        SetRandomRole();

    }

    IEnumerator GetGameObjects()
    {
        yield return new WaitUntil(() => GameObject.Find("Objects") != null);
        allObjects = GameObject.Find("Objects");

        storedEquipement = null;
        
        yield return new WaitUntil(() => GameObject.Find("TakeObject") != null);
        interractionDisplay = GameObject.Find("TakeObject");
        interactionText = interractionDisplay.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        
        yield return new WaitUntil(() => GameObject.Find("InventoryDisplay") != null);
        inventoryDisplay = GameObject.Find("InventoryDisplay");
        inventoryText = inventoryDisplay.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        
        yield return new WaitUntil(() => GameObject.Find("PlayerCanvas") != null);
        canvas = GameObject.Find("PlayerCanvas");
        int canvasCount = canvas.transform.childCount;
        for (int i = 0; i < canvasCount; i++)
        {
            if (canvas.transform.GetChild(i).tag == "Score")
            {
                personalScore = canvas.transform.GetChild(i).GetComponent<PersonalScore>();
                globalScore = canvas.transform.GetChild(i).GetComponent<GlobalScore>();
            }
        }
        
        findAllObjects = true;

    }

    // Update is called once per frame
    void Update()
    {
        if(findAllObjects == false)
            return;
        
        if (Input.GetKeyDown(KeyCode.Alpha9) && criminal == false)
        {
            transform.GetChild(0).GetChild(0).GetComponent<PostProcessManager>().allPostProcessVolumesEnabled[selectedFilter] ^= true;
        }

        if (GetComponent<PhotonView>().IsMine)
        {
            List<(GameObject, float)> _reachableObjects = reachableObjects();
            GameObject nearestObj = findNearestObj(_reachableObjects);
            
            if (_reachableObjects.Count > 0)
            {
                interractionDisplay.SetActive(true);
                interactionText.text = "Take " + nearestObj.name + "\nPress E";
            }
            else
            {
                interractionDisplay.SetActive(false);
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
    }

    #region object
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

    public void DesequipmentTriggeredWhenPlayerLeaveGame()
    {
        foreach (Transform obj in equipements.transform)
        {
            obj.GetComponent<Object>().OnDesequipmentTriggeredWhenPlayerLeaveGame();
        }

        foreach (Transform obj in inventory.transform)
        {
            obj.GetComponent<Object>().OnDesequipmentTriggeredWhenPlayerLeaveGame();
        }
    }

    public void UpdateCooldownDisplay(float currentCooldown, float cooldownMax, string objectName)
    {
        canvas = GameObject.Find("PlayerCanvas");
        int canvasCount = canvas.transform.childCount;
        for (int i = 0; i < canvasCount; i++)
        {
            if (canvas.transform.GetChild(i).name == "Weapon")
            {
                WeaponPanel wp = canvas.transform.GetChild(i).GetChild(0).GetComponent<WeaponPanel>();
                wp.cooldownByWeapon[objectName] = currentCooldown;
                wp.cooldownMax = cooldownMax;
                wp.currentCooldown = currentCooldown;
            }
        }
        StartCoroutine(EquipedWeaponDisplay());
    }

    public void UpdateEquipedWeaponDisplay()
    {
        StartCoroutine(EquipedWeaponDisplay());
    }

    IEnumerator EquipedWeaponDisplay()
    {
        yield return new WaitForSeconds(0.02f);
        canvas = GameObject.Find("PlayerCanvas");
        int canvasCount = canvas.transform.childCount;
        for (int i = 0; i < canvasCount; i++)
        {
            if (canvas.transform.GetChild(i).name == "Weapon")
            {
                WeaponPanel wp = canvas.transform.GetChild(i).GetChild(0).GetComponent<WeaponPanel>();
                wp.UpdateWeaponDisplay(equipements);
            }
        }
    }
    #endregion

    #region hp
    //G�re la modification des pv du joueur
    //Pris en compte dans le fichier HealthBar
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            Debug.Log("Game Over");
        }
    }

    public void RecoverHP(int heal)
    {
        currentHP += heal;
        if (currentHP >= hpMax)
        {
            currentHP = hpMax;
            //Debug.Log("Full Life");
        }
    }
    #endregion

    #region score
    //Appelle les fonctions contenues dans GlobalScore et PersonalScore afin de g�rer la modification du score
    public void IncreasePersonalScore()
    {
        personalScore.IncreaseScore();
    }

    public void DecreasePersonalScore()
    {
        personalScore.DecreaseScore();
    }

    public void IncreaseGlobalScore()
    {
        globalScore.IncreaseScore();
    }

    public void DecreaseGlobalScore()
    {
        globalScore.DecreaseScore();
    }
    #endregion



    #region move
    public void canMove(bool move)
    {
        gameObject.GetComponent<Movement>().canMove = move;
    }
    #endregion


    #region roleAndFilter
    public void SetRandomRole()
    {
        if (photonView.IsMine)
        {

            float random = UnityEngine.Random.Range(0.000f, 1.000f);

            bool alreadyACriminal = false;
            foreach (Transform player in transform.parent)
            {
                if (player.GetComponent<PlayerStatManager>().criminal)
                {
                    alreadyACriminal = true;
                }
            }

            if (!alreadyACriminal && (transform.parent.childCount == 4 || random > 0.75f))
            {
                photonView.RPC(nameof(RandomRole), RpcTarget.AllBuffered, true);
            }
            else
            {
                photonView.RPC(nameof(RandomRole), RpcTarget.AllBuffered, false);
                StartCoroutine(AddFilter());
            }
        }
        
    }

    [PunRPC]
    public void RandomRole(bool role)
    {
        criminal = role;
        selectedFilter = -1;
    }

    IEnumerator AddFilter()
    {
        yield return new WaitForSeconds(0.1f);
        if (photonView.IsMine)
        {
            List<int> filtersAvailable = new List<int>();
            for (int i = 0; i < transform.GetChild(0).GetChild(0).GetComponent<PostProcessManager>().allPostProcessVolumes.Count; i++)
            {
                filtersAvailable.Add(i);
            }
            foreach (Transform player in transform.parent)
            {
                if (player.GetComponent<PlayerStatManager>().selectedFilter != -1)
                {
                    filtersAvailable.Remove(player.GetComponent<PlayerStatManager>().selectedFilter);
                }
            }
            int randomRange = UnityEngine.Random.Range(0, filtersAvailable.Count);
            int randomFilter = filtersAvailable[randomRange];
            photonView.RPC(nameof(Filter), RpcTarget.AllBuffered, randomFilter, filtersAvailable.Count);
        }
    }

    [PunRPC]
    public void Filter(int randomFilter, int f)
    {
        selectedFilter = randomFilter;
        PostProcessManager ppm = transform.GetChild(0).GetChild(0).GetComponent<PostProcessManager>();
        ppm.allPostProcessVolumesEnabled[randomFilter] = true;
    }
    #endregion

    //[PunRPC]
    public void spawnObject(Vector3 pos, Quaternion rot, int idToSpawn)//, Transform parent, PlayerStatManager playerStatManager
    {
        GameObject newObject = PhotonNetwork.Instantiate("Prefabs/Objects/"+objectPrefabListToInstantiate[idToSpawn].name,Vector3.zero, Quaternion.identity, 0);
        newObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
        newObject.GetComponent<Object>().Init();

        if (!(thisPlayer.transform.Find("Equipements").childCount > 0 &&
              thisPlayer.transform.Find("Inventory").childCount > 0))
        {
            
            if (thisPlayer.transform.Find("Equipements").childCount > 0)
            {
                thisPlayer.transform.Find("Equipements").GetComponent<UseObject>().OnStoreEquipement();
            }
            newObject.GetComponent<Object>().EquipmentDest = thisPlayer.transform.Find("Equipements");
            newObject.GetComponent<Object>().OnEquipmentTriggered(thisPlayer.transform);
            
        }
        

    }
    
    public IEnumerator OnDestroy()
    {
        DesequipmentTriggeredWhenPlayerLeaveGame();
        yield return new WaitForSeconds(1);
    }
}
