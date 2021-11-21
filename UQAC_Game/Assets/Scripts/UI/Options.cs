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
    bool visible = false;

    public AudioSource[] volumeMusique;
    public Slider slider;
    const string volumeMusiquePrefKey = "VolumeMusique";

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
        //ClearPlayer();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
    
    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {   
            //ClearPlayer();
            if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
            
            PhotonNetwork.LeaveRoom();
        }
    }
    
    /*
    public void ClearPlayer()
    {
        GameObject allPlayers = GameObject.Find("Players");
        
        int id = -1;
        //Debug.Log(GetComponent<PhotonView>().ViewID);
        
        for (int i = 0; i < allPlayers.transform.childCount; i++)
        {
            if (allPlayers.transform.GetChild(i).GetComponent<PhotonView>().IsMine)
            {
                id = i;
                
            }
        }
        
        if (id >= 0)
        {
            if (allPlayers.transform.GetChild(id).Find("Equipements").childCount > 0)
            {
                allPlayers.transform.GetChild(id).Find("Equipements").GetChild(0).GetComponent<Object>().OnDesequipmentTriggered();
            }
            //allPlayers.transform.GetChild(id).Find("Equipements").GetChild(0).GetComponent<Object>().OnDesequipmentTriggered();

        }
        
    }*/

}