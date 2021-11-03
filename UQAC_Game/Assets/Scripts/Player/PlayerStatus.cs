using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerStatus : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Public Fields

    //[Tooltip("The current Health of our player")]
    //public float Health = 1f;

    [Tooltip("The current Task Progession of all player")]
    public float ProgessionTasksGlobal = 0f;
    [SerializeField] private GlobalScore globalScore;

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        globalScore = GameObject.FindGameObjectWithTag("Score").GetComponent<GlobalScore>();
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
            // TODO: do some thing

            /*if (this.Health <= 0f)
            {
                GameManager.Instance.LeaveRoom();
            }*/
        }
        // we dont' do anything if we are not the local player.
        else
        {
            return;
        }

    }


    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            //stream.SendNext(this.IsFiring);
            //stream.SendNext(this.Health);
            stream.SendNext(GetProgessionTasksGlobal());
        }
        else
        {
            // Network player, receive data
            //this.IsFiring = (bool)stream.ReceiveNext();
            //this.Health = (float)stream.ReceiveNext();
            SetProgessionTasksGlobal((float)stream.ReceiveNext());
        }
    }

    #endregion

    private void SetProgessionTasksGlobal(float value)
    {
        this.ProgessionTasksGlobal = value;
        if (globalScore != null) globalScore.SetScore((int)this.ProgessionTasksGlobal);
    }

    private float GetProgessionTasksGlobal()
    {
        if (globalScore != null) this.ProgessionTasksGlobal = (float)globalScore.GetScore();
        return this.ProgessionTasksGlobal;
    }
}
