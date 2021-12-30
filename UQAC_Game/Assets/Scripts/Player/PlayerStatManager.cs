using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerStatManager : MonoBehaviourPun {
    public GameObject thisPlayer;

    public int currentHP;
    public int stamina;
    public int hpMax;
    public Vector3 playerColor ;

    public bool isDead = false;
    public bool canMove = true;

    public GameObject canvas;
    public PersonalScore personalScore;
    public GlobalScore globalScore;

    public GameObject infoRole;
    
    public GameObject allObjects;
    public List<GameObject> allMiniGames;

    public GameObject interractionDisplay;
    public TextMeshProUGUI interactionText;
    public GameObject inventoryDisplay;
    public TextMeshProUGUI inventoryText;

    public GameObject storedEquipement;
    public GameObject equipements;
    public GameObject inventory;

    public bool criminal = false;
    public int selectedFilter = 0;

    public List<GameObject> objectPrefabListToInstantiate;
    public bool findAllObjects = false;

    public bool isMinePlayer;
    public string playerName;

    public GameObject repairPositionForMiniMap;
    
    // Start is called before the first frame update
    void Start () {
        currentHP = 100;
        hpMax = 100;
        thisPlayer = this.gameObject;

        isMinePlayer = photonView.IsMine;
        playerName = photonView.Controller.NickName;

        
        //Enable a point in map to show the player's position
        repairPositionForMiniMap.SetActive(isMinePlayer);
        
        StartCoroutine(GetGameObjects());
    }

    IEnumerator GetGameObjects () {
        //get Objects & Mini-Games
        yield return new WaitUntil(() => GameObject.Find("Objects") != null);
        allObjects = GameObject.Find("Objects");
        allMiniGames = GameObject.FindGameObjectsWithTag("MiniGame").ToList();

        storedEquipement = null; // Set an empty inventory

        //Set Canvas informations
        yield return new WaitUntil(() => GameObject.Find("PlayerCanvas") != null);
        canvas = GameObject.Find("PlayerCanvas");
        int canvasCount = canvas.transform.childCount;
        for (int i = 0 ; i < canvasCount ; i++) {
            //Set Canvas score global + perso
            if (canvas.transform.GetChild(i).tag == "Score") {
                personalScore = canvas.transform.GetChild(i).GetComponent<PersonalScore>();
                globalScore = canvas.transform.GetChild(i).GetComponent<GlobalScore>();
            }
            //Set Canvas info role
            else if (canvas.transform.GetChild(i).name == "InfoRole")
            {
                infoRole = canvas.transform.GetChild(i).gameObject;
            }
            //Set Canvas interaction
            else if (canvas.transform.GetChild(i).name == "TakeObject")
            {
                interractionDisplay = canvas.transform.GetChild(i).gameObject;
                interactionText = interractionDisplay.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            }
        }

        // Find Disable object
        yield return new WaitUntil(() => (inventoryDisplay = canvas.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "InventoryDisplay").gameObject) != null);
        inventoryText = inventoryDisplay.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        
        //Only the master session set Roles and Color
        if (PhotonNetwork.IsMasterClient && isMinePlayer)
        {
            StartCoroutine(SetRandomRole());
            StartCoroutine(AddFilter());
        }



        findAllObjects = true;

    }

    // Update is called once per frame
    void Update () {
        if (findAllObjects == false)
            return;
        
#if UNITY_EDITOR // allow cheat code just when we start game with unity
        // DEBUG MODE -- Remove filter
        if (Input.GetKeyDown(KeyCode.Alpha9) && criminal == false) {
            transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<PostProcessManager>().allPostProcessVolumesEnabled[selectedFilter] ^= true;
        }
#endif
        
        //Update interaction canvas
        if (GetComponent<PhotonView>().IsMine && !isDead) {
            List<(GameObject, float)> _reachableObjects = reachableObjects();
            GameObject nearestObj = findNearestObj(_reachableObjects);

            if (_reachableObjects.Count > 0) {
                interractionDisplay.SetActive(true);
                if (nearestObj.tag == "MiniGame") {
                    interactionText.text = "Start Minigame" + "\nPress E";
                } else {
                    interactionText.text = "Take " + nearestObj.name + "\nPress E";
                }
            } else {
                interractionDisplay.SetActive(false);
            }

            if (storedEquipement != null) {
                inventoryDisplay.SetActive(true);
                inventoryText.text = storedEquipement.name;
            } else {
                inventoryDisplay.SetActive(false);
                inventoryText.text = "";
            }
        }

        //Live or Dead
        if (currentHP <= 0) {
            isDead = true;
            canMove = false;
        }

        gameObject.GetComponent<Animations>().DeathAnim();

    }

    #region object
    // Functions to manage objects

    List<(GameObject, float)> reachableObjects () {
        List<(GameObject, float)> _reachableObjects = new List<(GameObject, float)>();
        int allObjectCount = allObjects.transform.childCount;
        for (int i = 0 ; i < allObjectCount ; i++) {
            (bool _isReachable, float _dist) =
                IsReachable(allObjects.transform.GetChild(i) , gameObject.transform , allObjects.transform.GetChild(i).GetComponent<Object>().distanceToHeld);
            if (_isReachable) {
                _reachableObjects.Add((allObjects.transform.GetChild(i).gameObject, _dist));
            }
        }
        int allMiniGameCount = allMiniGames.Count;
        for (int i = 0 ; i < allMiniGameCount ; i++) {
            if (allMiniGames[i].GetComponent<MiniGameStarter>().gameEnded == false && allMiniGames[i].GetComponent<MiniGameStarter>().isOpen == false)// si le jeu n'est pas déjà terminé et qu'il n'est pas ouvert
            {
                (bool _isReachable, float _dist) =
                    IsReachable(allMiniGames[i].transform, gameObject.transform,
                        allMiniGames[i].GetComponent<MiniGameStarter>().distanceToStart);
                if (_isReachable)
                {
                    _reachableObjects.Add((allMiniGames[i].gameObject, _dist));
                }
            }
        }

        return _reachableObjects;
    }

    (bool, float) IsReachable (Transform objectA , Transform playerA , float range)
    {
        float dist = Vector3.Distance(objectA.position - new Vector3(0 , objectA.position.y , 0) , (playerA.position - new Vector3(0, playerA.position.y, 0)));
        if (dist < range) // in distance range
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

    GameObject findNearestObj (List<(GameObject, float)> _reachableObjects) {
        float nearestObjDist = -1;
        GameObject nearestObj = null;
        foreach ((GameObject o, float dist) in _reachableObjects) {
            if (nearestObjDist == -1 || dist < nearestObjDist) {
                nearestObj = o;
                nearestObjDist = dist;
            }

        }

        return nearestObj;
    }

    public void DesequipmentTriggeredWhenPlayerLeaveGame () {
        foreach (Transform obj in equipements.transform) {
            obj.GetComponent<Object>().OnDesequipmentTriggeredWhenPlayerLeaveGame();
        }

        foreach (Transform obj in inventory.transform) {
            obj.GetComponent<Object>().OnDesequipmentTriggeredWhenPlayerLeaveGame();
        }
    }

    public void UpdateCooldownDisplay (float currentCooldown , float cooldownMax , string objectName) {
        canvas = GameObject.Find("PlayerCanvas");
        WeaponPanel wp = canvas.GetComponentInChildren<WeaponPanel>();
        wp.cooldownByWeapon[objectName] = currentCooldown;
        wp.cooldownMax = cooldownMax;
        wp.currentCooldown = currentCooldown;
        
        StartCoroutine(EquipedWeaponDisplay());
    }

    public void UpdateEquipedWeaponDisplay () {
        StartCoroutine(EquipedWeaponDisplay());
    }

    IEnumerator EquipedWeaponDisplay () {
        //yield return new WaitForSeconds(0.02f);
        canvas = GameObject.Find("PlayerCanvas");
        WeaponPanel wp = canvas.GetComponentInChildren<WeaponPanel>();
        wp.UpdateWeaponDisplay(equipements);
        yield return null;
    }

    public void HideEquipedWeaponDisplay()
    {
        print("HideEquipedWeaponDisplay");
        canvas = GameObject.Find("PlayerCanvas");
        WeaponPanel wp = canvas.GetComponentInChildren<WeaponPanel>();
        wp.HideDisplay();
    }
    #endregion

    #region hp
    //Management of player's health

    public void TakeDamage (int damage, int viewId) {
        
        Transform player = FindPlayerByID(viewId);
        if (player != null)
        {
            PlayerStatManager playerStatManager = player.GetComponent<PlayerStatManager>();
            
            playerStatManager.currentHP -= damage;
            playerStatManager.gameObject.GetComponent<Animations>().HitAnim();
            if (playerStatManager.currentHP <= 0) {
                playerStatManager.currentHP = 0;
                Debug.Log("Game Over");
            }
        }
    }

    public void RecoverHP (int heal, int viewId)
    {
        Transform player = FindPlayerByID(viewId);
        if (player != null)
        {
            PlayerStatManager playerStatManager = player.GetComponent<PlayerStatManager>();
            
            playerStatManager.currentHP += heal;
            if (playerStatManager.currentHP >= playerStatManager.hpMax) {
                playerStatManager.currentHP = playerStatManager.hpMax;
            }
        }
    }
    #endregion

    #region score
    //Functions to modify Scorces
    public void IncreasePersonalScore () {
        personalScore.IncreaseScore();
    }

    public void DecreasePersonalScore () {
        personalScore.DecreaseScore();
    }

    public void IncreaseGlobalScore () {
        globalScore.IncreaseScore();
    }

    public void DecreaseGlobalScore () {
        globalScore.DecreaseScore();
    }
    #endregion


    #region roleAndFilter
    // Functions to manage roles & filters 

    IEnumerator SetRandomRole()
    {
        yield return new WaitForSeconds(3f);
        int nbrMaxCriminels = 1;
        // we set a criminal if we don't have
        if (transform.parent.GetComponentsInChildren<PlayerStatManager>().Where((player) => player.criminal == true)
            .Count() < nbrMaxCriminels)
        {
            int random = UnityEngine.Random.Range(0, transform.parent.childCount);
            photonView.RPC(nameof(RandomRole), RpcTarget.AllBufferedViaServer, true,
                transform.parent.GetChild(random).GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    public void RandomRole(bool role, int idPlayer)
    {
        Transform player = FindPlayerByID(idPlayer);
        player.GetComponent<PlayerStatManager>().criminal = role;
        player.GetComponent<PlayerStatManager>().selectedFilter = -1;

        if (player.GetComponent<PhotonView>().IsMine && player.GetComponent<PlayerStatManager>().criminal) // set only one criminal
        {
            infoRole.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Criminel";
        }
    }

    IEnumerator AddFilter()
    {
        yield return new WaitForSeconds(4f);
        List<int> filtersAvailable = new List<int>();
        for (int i = 0; i < transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<PostProcessManager>().allPostProcessVolumes.Count; i++)
        {
            filtersAvailable.Add(i);
        }
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if (transform.parent.GetChild(i).GetComponent<PlayerStatManager>().criminal == false)
            {
                int randomRange = UnityEngine.Random.Range(0, filtersAvailable.Count);
                int randomFilter = filtersAvailable[randomRange];
                photonView.RPC(nameof(Filter), RpcTarget.AllBufferedViaServer, randomFilter, transform.parent.GetChild(i).GetComponent<PhotonView>().ViewID);

                filtersAvailable.Remove(randomFilter);
            }
        } 
    }

    [PunRPC]
    public void Filter(int randomFilter, int idPlayer)
    {
        Debug.Log(randomFilter + " " + idPlayer + " Filter");
        Transform player = FindPlayerByID(idPlayer);
        player.GetComponent<PlayerStatManager>().selectedFilter = randomFilter;
        player.GetChild(0).GetChild(0).GetChild(0).GetComponent<PostProcessManager>().allPostProcessVolumesEnabled[randomFilter] = true;
    }
    #endregion

    public void spawnObject (Vector3 pos , Quaternion rot , int idToSpawn)//, Transform parent, PlayerStatManager playerStatManager
    {
        GameObject newObject = PhotonNetwork.Instantiate("Prefabs/Objects/" + objectPrefabListToInstantiate[idToSpawn].name , Vector3.zero , Quaternion.identity , 0);
        newObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
        newObject.GetComponent<Object>().Init();

        if (!( thisPlayer.transform.Find("Equipements").childCount > 0 &&
              thisPlayer.transform.Find("Inventory").childCount > 0 )) {

            if (thisPlayer.transform.Find("Equipements").childCount > 0) {
                thisPlayer.transform.Find("Equipements").GetComponent<UseObject>().OnStoreEquipement();
            }
            newObject.GetComponent<Object>().EquipmentDest = thisPlayer.transform.Find("Equipements");
            newObject.GetComponent<Object>().OnEquipmentTriggered(thisPlayer.transform);
            newObject.GetComponent<Object>().OnDesequipmentTriggered();
            newObject.GetComponent<Object>().OnEquipmentTriggered(thisPlayer.transform);

        }


    }

    public void OnDestroy () {
        //DesequipmentTriggeredWhenPlayerLeaveGame();
    }

    Transform FindPlayerByID(int id)
    {
        foreach (Transform child in transform.parent)
        {
            if (child.GetComponent<PhotonView>().ViewID == id)
            {
                return child;
            }
        }
        return null;
    }

    public void setPlayerColor(float _r, float _g, float _b, int idPlayer)
    {
        
        Transform player = FindPlayerByID(idPlayer);
        if (player != null)
        {
            player.GetComponentInChildren<RandPlayerColor>().setSkinColor(_r, _g, _b);
            player.GetComponent<PlayerStatManager>().playerColor = new Vector3(_r, _g, _b);
        }
    }
}