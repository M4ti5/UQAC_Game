using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviourPun
{
    public new bool enabled = true;
    public bool useInstanciate = false;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    private void Awake()
    {
        if (useInstanciate)
        {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }
        }

        if (enabled)
        {
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(gameObject);
        }
    }
}
