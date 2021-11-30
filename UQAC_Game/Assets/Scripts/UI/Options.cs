using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using Photon.Pun.UtilityScripts;

public class Options : MonoBehaviourPun
{
    public GameObject panel;
    public bool visible = false;

    public AudioSource[] volumeMusique;
    public Slider slider;
    const string volumeMusiquePrefKey = "VolumeMusique";

    public GameObject allPlayers;

    public Button leaveBtn;

    private void Start()
    {
        panel.SetActive(visible);
        volumeMusique = FindObjectsOfType<AudioSource>();
        if (PlayerPrefs.HasKey(volumeMusiquePrefKey))
        {
            string defaultVolumeMusique = PlayerPrefs.GetString(volumeMusiquePrefKey); 
            slider.value = (float)Convert.ToDouble(defaultVolumeMusique); ;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            visible = !visible;
            panel.SetActive(visible);
            
        }
        
        leaveBtn.interactable = PhotonNetwork.InRoom;

    }

    public void OnVolumeMusiqueSliderChange()
    {
        foreach (AudioSource v in volumeMusique)
        {
            v.volume = slider.value;
            PlayerPrefs.SetString(volumeMusiquePrefKey, Convert.ToString(slider.value)); // � voir si on peut aussi mettre des float
        }
    }

    /// <summary>
    /// Close programme or stop playing mode if we use unity
    /// </summary>
    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
    
    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {   
            if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }

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

            PhotonNetwork.LeaveRoom();
        }
    }
    

}