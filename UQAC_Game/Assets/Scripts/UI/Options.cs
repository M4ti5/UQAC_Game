using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public GameObject panel;
    bool visible = false;

    public AudioSource[] volumeMusique;
    public Slider slider;

    private void Start()
    {
        panel.SetActive(visible);
        volumeMusique = FindObjectsOfType<AudioSource>();
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