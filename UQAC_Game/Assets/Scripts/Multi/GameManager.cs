// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Launcher.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in "PUN Basic tutorial" to handle typical game management requirements
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Game manager.
/// Connects and watch Photon Status, Instantiate Player
/// Deals with quiting the room and the game
/// Deals with level loading (outside the in room synchronization)
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks
{

	#region Public Fields

	static public GameManager Instance;

	#endregion

	#region Private Fields

	private GameObject instance;

    [Tooltip("The prefab to use for representing the player")]
    [SerializeField]
    private GameObject playerPrefab;

    #endregion

    #region MonoBehaviour CallBacks

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void Start()
	{
		Instance = this;

		// in case we started with the wrong scene being active, simply load the menu scene
		if (!PhotonNetwork.IsConnected)
		{
			SceneManager.LoadScene("Launcher");

			return;
		}

		if (playerPrefab == null) // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.
		{

			Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
		}
		else
		{


			if (PlayerManager.LocalPlayerInstance == null)
			{
				Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

				// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
				Vector3 spawnPosition = new Vector3(Random.Range(-4.0f, 4.0f), 0.1f, Random.Range(-4.0f, 4.0f));
				GameObject newPlayer = PhotonNetwork.Instantiate("Prefabs/Player/" + this.playerPrefab.name, spawnPosition, Quaternion.LookRotation(-spawnPosition), 0);
			}
			else
			{

				Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
			}


		}

	}

    #endregion

    #region Photon Callbacks

    /// <summary>
    /// Called when a Photon Player got connected. We need to then load a bigger scene.
    /// </summary>
    /// <param name="other">Other.</param>
    public override void OnPlayerEnteredRoom( Player other  )
	{
		Debug.Log( "OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting

		if ( PhotonNetwork.IsMasterClient )
		{
			Debug.LogFormat( "OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient ); // called before OnPlayerLeftRoom

			//LoadArena();
		}
	}

	/// <summary>
	/// Called when a Photon Player got disconnected. We need to load a smaller scene.
	/// </summary>
	/// <param name="other">Other.</param>
	public override void OnPlayerLeftRoom( Player other  )
	{
		Debug.Log( "OnPlayerLeftRoom() " + other.NickName ); // seen when other disconnects

		if ( PhotonNetwork.IsMasterClient )
		{
			Debug.LogFormat( "OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient ); // called before OnPlayerLeftRoom

			//LoadArena(); 
		}
	}

	/// <summary>
	/// Called when the local player left the room. We need to load the launcher scene.
	/// </summary>
	public override void OnLeftRoom()
	{
		SceneManager.LoadScene("Launcher");
	}

	#endregion

	#region Public Methods

	public void LeaveRoom()
	{
		if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
		{
			PhotonNetwork.CurrentRoom.IsOpen = false;
		}
		PhotonNetwork.LeaveRoom();
	}

	#endregion

	/*
	#region Private Methods

	// reload game (never used because we have 1 level)
	void LoadArena()
	{
		if ( ! PhotonNetwork.IsMasterClient )
		{
			Debug.LogError( "PhotonNetwork : Trying to Load a level but we are not the master Client" );
		}

		//Debug.LogFormat( "PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount );

		//PhotonNetwork.LoadLevel("PunBasics-Room for "+PhotonNetwork.CurrentRoom.PlayerCount);
		Debug.LogFormat("PhotonNetwork : Loading Game - PlayerCount: {0}", PhotonNetwork.CurrentRoom.PlayerCount);
		PhotonNetwork.LoadLevel("Game");
	}

	#endregion
	*/
}