using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessManager : MonoBehaviour
{
    public List<PostProcessVolume> allPostProcessVolumes = new List<PostProcessVolume>();
    public List<bool> allPostProcessVolumesEnabled = new List<bool>();


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start PostProcessManager script");
        foreach (Transform postProcess in transform)
        {
            if (postProcess.GetComponent<PostProcessVolume>())
            {
                allPostProcessVolumes.Add(postProcess.GetComponent<PostProcessVolume>());
                allPostProcessVolumesEnabled.Add(false);// create list to active or note a mini game
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        int i=0;
        foreach (bool enabled in allPostProcessVolumesEnabled)
        {
            allPostProcessVolumes[i].enabled = enabled;
            i++;
        }
    }
}
