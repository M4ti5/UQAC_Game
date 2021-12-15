// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerManager.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in PUN Basics Tutorial to deal with the networked player instance
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;

/// <summary>
/// Player manager.
/// </summary>
public class PlayerManager : MonoBehaviourPun
{
    #region Public Fields

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    public GameObject playerCamera;

    #endregion


    #region MonoBehaviour CallBacks

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    public void Awake()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
            LocalPlayerInstance.name += " Mine"; // rename my player
        }
        // Group all players
        AddToPlayerParent();

        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Group all players to one parent
    /// </summary>
    private void AddToPlayerParent()
    {
        GameObject playersParent = GameObject.Find("Players");
        if (playersParent != null)
        {
            transform.parent = playersParent.transform;
            transform.SetParent(playersParent.transform);
        }
        else
        {
            Debug.LogError("<Color=Red><b>Not Found</b></Color> all players Reference 'Players'", this);
        }
    }

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    public void Start()
    {
        // disable camera if note mine to avoid multicamera cast
        if (playerCamera != null)
        {
            if (photonView.IsMine == false)
            {
                playerCamera.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> player camera Reference .", this);
        }

    }

    #endregion
}
