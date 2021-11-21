using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;

public class Options : MonoBehaviour
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
            PlayerPrefs.SetString(volumeMusiquePrefKey, Convert.ToString(slider.value)); // à voir si on peut aussi mettre des float
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

            PhotonNetwork.LeaveRoom();
        }
    }

}