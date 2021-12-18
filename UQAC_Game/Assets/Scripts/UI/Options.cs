using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using Photon.Pun.UtilityScripts;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviourPunCallbacks
{
    public GameObject panel;
    public bool visible = false;

    public AudioSource[] volumeMusique;
    public Slider slider;
    const string volumeMusiquePrefKey = "VolumeMusique";
    
    const string photonDebugModePrefKey = "PhotonDebugMode";
    public bool photonDebugMode;
    public Toggle toggleDebugMode;
    public GameObject photonStatus;

    public GameObject allPlayers;

    public Button leaveBtn;

    private void Start()
    {
        // hide option to be sure
        panel.SetActive(visible);
        // get all audio in scene
        volumeMusique = FindObjectsOfType<AudioSource>();
        
        // load saved volume of previous session
        if (PlayerPrefs.HasKey(volumeMusiquePrefKey))
        {
            string defaultVolumeMusique = PlayerPrefs.GetString(volumeMusiquePrefKey); 
            slider.value = (float)Convert.ToDouble(defaultVolumeMusique); ;
        }
        
        // load saved value of previous session to show or hide debug photon panel
        if (PlayerPrefs.HasKey(photonDebugModePrefKey))
        {
            string defaultPhotonDebugMode = PlayerPrefs.GetString(photonDebugModePrefKey); 
            photonDebugMode = (bool)Convert.ToBoolean(defaultPhotonDebugMode);
            toggleDebugMode.isOn = photonDebugMode;
            if (GameObject.Find("PhotonStatus") != null)
            {
                photonStatus = GameObject.Find("PhotonStatus").transform.GetChild(0).GetChild(0).gameObject;
            }
        }
    }

    void Update()
    {
        // escape key to toggle menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
        
        // automaticly active or not leave button (disable in home scene/in lobby)
        leaveBtn.interactable = PhotonNetwork.InRoom;
        
        // find photonstatus panel if not found prevously to avoid problems of order start scripts
        if(photonStatus == null)
            if (GameObject.Find("PhotonStatus") != null)
                photonStatus = GameObject.Find("PhotonStatus").transform.GetChild(0).GetChild(0).gameObject;
        if (photonStatus != null)
        {
            // change if toggle is different
            if (photonStatus.activeSelf != photonDebugMode)
            {
                // hide/show photon panel
                photonStatus.SetActive(photonDebugMode);
                // disable check information if is hidden
                photonStatus.transform.parent.parent.GetComponent<PhotonStatus>().activated = photonDebugMode;
            }
        }
    }

    public void ToggleMenu()
    {
        visible = !visible;
        panel.SetActive(visible);
    }

    // adjust same volume on all audio depending on slider value
    public void OnVolumeMusiqueSliderChange()
    {
        foreach (AudioSource v in volumeMusique)
        {
            v.volume = slider.value;
            // save value in pref file
            PlayerPrefs.SetString(volumeMusiquePrefKey, Convert.ToString(slider.value));
        }
    }

    // Close programme or stop playing mode if we use unity
    public void QuitApplication()
    {
#if UNITY_EDITOR // to stop play mode when using unity
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
    
    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            // close room if we are the last one
            if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
            
            // desequipe our perso
            if (allPlayers != null)
            {
                PlayerStatManager playerStatManager = GetComponent<PlayerStatManager>();
                int playerCount = allPlayers.transform.childCount;
                for (int i = 0; i < playerCount; i++)
                {
                    if (allPlayers.transform.GetChild(i).GetComponent<PhotonView>().IsMine)
                    {
                        playerStatManager =
                            allPlayers.transform.GetChild(i).transform.GetComponent<PlayerStatManager>();
                    }
                }

                if (playerStatManager != null)
                    playerStatManager.DesequipmentTriggeredWhenPlayerLeaveGame();
            }
            
            // now we can realy leave room
            PhotonNetwork.LeaveRoom();
        }
    }
    
    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Launcher");
    }
    


    public void OnDebugModeChange()
    {
        photonDebugMode = toggleDebugMode.isOn;
        // save value in pref file
        PlayerPrefs.SetString(photonDebugModePrefKey, Convert.ToString(photonDebugMode));
    }
}