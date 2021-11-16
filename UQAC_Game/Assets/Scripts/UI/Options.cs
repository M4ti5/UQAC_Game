using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Options : MonoBehaviour
{
    public GameObject Panel;
    bool visible = false;

    public AudioSource[] volumeMusique;
    public Slider slider;

    private void Start()
    {
        Panel.SetActive(visible);
        volumeMusique = FindObjectsOfType<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            visible = !visible;
            Panel.SetActive(visible);
        }
        
    }

    public void SliderChange()
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

}