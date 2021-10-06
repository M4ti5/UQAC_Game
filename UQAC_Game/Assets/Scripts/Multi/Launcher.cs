using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

// changed from https://www.raywenderlich.com/1142814-introduction-to-multiplayer-games-with-unity-and-photon

/// <summary>
/// Launch manager. Connect, join a random room or create one if none or all full.
/// </summary>
public class Launcher : MonoBehaviourPunCallbacks
{
    public GameObject roomJoinUI;
    public GameObject buttonStart;
	public TMP_InputField inputFieldPlayerName, inputFieldRoomName;
	public TextMeshProUGUI connectionStatus;
	public TextMeshProUGUI feedbackText;
	private string gameVersion;
    private bool isConnecting;

    [Tooltip("The maximum number of players per room")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;

    private string roomName;
	
	const string playerNamePrefKey = "PlayerName"; // Store the PlayerPref Key to avoid typos
	const string roomNamePrefKey = "RoomName";// Store the PlayerPref Key to avoid typos

	void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;

		// get Application version
		this.gameVersion = Application.version;
	}

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start launcher script");

        //PlayerPrefs.DeleteAll(); // To avoid any anomalies

        Debug.Log("Connecting to Photon Network");

		// The UI elements are hidden by default, and are activated once a connection to a Photon server is established
		//this.roomJoinUI.SetActive(false);
		//this.buttonStart.SetActive(false);

		//ConnectToPhoton(); // to connect to the Photon network

		LoadDefaultPlayerName();
		LoadDefaultRoomName();

	}

	// Helper Methods

	public void LoadDefaultPlayerName()
    {
		if (inputFieldPlayerName != null)
		{
			if (PlayerPrefs.HasKey(playerNamePrefKey))
			{
				string defaultName = PlayerPrefs.GetString(playerNamePrefKey);
				inputFieldPlayerName.text = defaultName;
			}
        }
        else
        {
			Debug.LogError("<Color=Red><b>Missing</b></Color> inputField PlayerName Reference.", this);
		}
	}

	/// <summary>
	/// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
	/// </summary>
	/// <param name="value">The name of the Player</param>
	public void SetPlayerName(string value)
	{
		// #Important
		if (string.IsNullOrEmpty(value))
		{
			Debug.LogError("Player Name is null or empty");
			return;
		}
		PhotonNetwork.NickName = value;

		PlayerPrefs.SetString(playerNamePrefKey, value);
	}
	public void LoadDefaultRoomName()
	{
		if (inputFieldRoomName != null)
		{
			if (PlayerPrefs.HasKey(roomNamePrefKey))
			{
				string defaultName = PlayerPrefs.GetString(roomNamePrefKey);
				inputFieldRoomName.text = defaultName;
			}
		}
		else
		{
			Debug.LogError("<Color=Red><b>Missing</b></Color> inputField RoomName Reference.", this);
		}
	}
	public void SetRoomName(string value)
    {
		if (string.IsNullOrEmpty(value))
		{
			Debug.LogError("Room Name is null or empty");
			this.roomName = null;
			return;
		}
		this.roomName = value;
		PlayerPrefs.SetString(roomNamePrefKey, value);
	}
	

	/// <summary>
	/// Start the connection process. 
	/// - If already connected, we attempt joining a random room
	/// - if not yet connected, Connect this application instance to Photon Cloud Network
	/// </summary>
	public void Connect()
	{
		// we want to make sure the log is clear everytime we connect, we might have several failed attempted if connection failed.
		this.feedbackText.text = "";

		// keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
		this.isConnecting = true;

		// hide the Play button for visual consistency
		this.roomJoinUI.SetActive(false);

		// we check if we are connected or not, we join if we are , else we initiate the connection to the server.
		if (PhotonNetwork.IsConnected)
		{
			LogFeedback("Joining Room...");
			// #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
			PhotonNetwork.JoinRandomRoom();
		}
		else
		{

			LogFeedback("Connecting...");

			// #Critical, we must first and foremost connect to Photon Online Server.
			PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.GameVersion = this.gameVersion;
		}
	}

	/// <summary>
	/// Logs the feedback in the UI view for the player, as opposed to inside the Unity Editor for the developer.
	/// </summary>
	/// <param name="message">Message.</param>
	void LogFeedback(string message)
	{
		// we do not assume there is a feedbackText defined.
		if (this.feedbackText == null)
		{
			return;
		}

		// add new messages as a new line and at the bottom of the log.
		this.feedbackText.text += System.Environment.NewLine + message;
	}

	#region MonoBehaviourPunCallbacks CallBacks
	// below, we implement some callbacks of PUN
	// you can find PUN's callbacks in the class MonoBehaviourPunCallbacks


	/// <summary>
	/// Called after the connection to the master is established and authenticated
	/// </summary>
	public override void OnConnectedToMaster()
	{
		// we don't want to do anything if we are not attempting to join a room. 
		// this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
		// we don't want to do anything.
		if (this.isConnecting)
		{
			LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");
			Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");

			// #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
			PhotonNetwork.JoinRandomRoom();
		}
	}

	/// <summary>
	/// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
	/// </summary>
	/// <remarks>
	/// Most likely all rooms are full or no rooms are available. <br/>
	/// </remarks>
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		LogFeedback("<color=red>OnJoinRandomFailed</color>: Next -> Create a new Room");
		Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

		// #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
		PhotonNetwork.CreateRoom(this.roomName, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom, IsVisible = true });
	}


	/// <summary>
	/// Called after disconnecting from the Photon server.
	/// </summary>
	public override void OnDisconnected(DisconnectCause cause)
	{
		LogFeedback("<color=red>OnDisconnected</color> " + cause);
		Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");

		this.isConnecting = false;
		this.roomJoinUI.SetActive(true);

	}

	/// <summary>
	/// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
	/// </summary>
	/// <remarks>
	/// This method is commonly used to instantiate player characters.
	/// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
	///
	/// When this is called, you can usually already access the existing players in the room via PhotonNetwork.PlayerList.
	/// Also, all custom properties should be already available as Room.customProperties. Check Room..PlayerCount to find out if
	/// enough players are in the room to start playing.
	/// </remarks>
	public override void OnJoinedRoom()
	{
		LogFeedback("<color=green>OnJoinedRoom</color> with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)");
		Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");

		// #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
		if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
		{
			Debug.Log("We load the 'Room for 1' ");

			// #Critical
			// Load the Room Level. 
			PhotonNetwork.LoadLevel("Game");

		}
	}

	#endregion
}
