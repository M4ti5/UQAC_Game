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
/// Handles fire Input and Beams.
/// </summary>
public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Public Fields

    //[Tooltip("The current Health of our player")]
    //public float Health = 1f;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    public GameObject playerCamera;

    #endregion

    #region Private Fields

    /*
    [Tooltip("The Beams GameObject to control")]
    [SerializeField]
    private GameObject beams;*/

    //True, when the user is firing
    bool IsFiring;

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
        if (playerCamera != null)
        {
            if (photonView.IsMine == false)
            {
                playerCamera.SetActive(false); // disable camera if note mine to avoid multicamera cast
            }
        }
        else
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> player camera Reference .", this);
        }

    }


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity on every frame.
    /// Process Inputs if local player.
    /// Show and hide the beams
    /// Watch for end of game, when local player health is 0.
    /// </summary>
    public void Update()
    {
        // we only process Inputs and check health if we are the local player
        if (photonView.IsMine)
        {
            // do some thing

            /*if (this.Health <= 0f)
            {
                GameManager.Instance.LeaveRoom();
            }*/
        }

        /*// we dont' do anything if we are not the local player.
        if (!photonView.IsMine)
        {
            return;
        }*/

    }

    #endregion

    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        /*if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(this.IsFiring);
            //stream.SendNext(this.Health);
        }
        else
        {
            // Network player, receive data
            this.IsFiring = (bool)stream.ReceiveNext();
            //this.Health = (float)stream.ReceiveNext();
        }*/
    }

    #endregion
}
