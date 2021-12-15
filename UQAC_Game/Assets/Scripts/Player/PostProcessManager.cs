using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessManager : MonoBehaviour
{
    public List<PostProcessVolume> allPostProcessVolumes = new List<PostProcessVolume>();
    public List<bool> allPostProcessVolumesEnabled = new List<bool>();

    private void Awake()
    {
        // Get all postProcessVolume
        foreach (Transform postProcess in transform)
        {
            if (postProcess.GetComponent<PostProcessVolume>())
            {
                allPostProcessVolumes.Add(postProcess.GetComponent<PostProcessVolume>());
                allPostProcessVolumesEnabled.Add(false);// create list of mini games
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Enabling the filter
        int i=0;
        foreach (bool enabled in allPostProcessVolumesEnabled)
        {
            allPostProcessVolumes[i].enabled = enabled; 
            i++;
        }
    }
}
