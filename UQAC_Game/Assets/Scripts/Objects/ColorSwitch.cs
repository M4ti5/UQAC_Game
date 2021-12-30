using Photon.Pun;
using UnityEngine;

/**
 * Script that implements the behavior of the color switch object
 * the player takes the color of the player in front of them.
 */
public class ColorSwitch : Object
{
    private RaycastHit swap;
    //maw distance to reach in front of the player
    public int maxDistance;

    [PunRPC]
    protected override void CustomBehaviour()
    {
        //Cast a ray in front of the player and store the collisions
        if (Physics.Raycast(HitObj.position,HitObj.forward, out swap, maxDistance))
        {

            if(swap.transform.tag == "Player" && swap.transform.GetComponent<PlayerStatManager>().isDead == false)// si le joueur n'est pas d�j� mort
            {
                if (player.GetComponent<PhotonView>().IsMine)
                {
                    Vector3 otherPlayerColor = swap.transform.GetComponent<PlayerStatManager>().playerColor;

                    //Network Task - synchro change color for all players
                    photonView.RPC(nameof(setPlayerColor), RpcTarget.AllBufferedViaServer, otherPlayerColor.x,
                        otherPlayerColor.y, otherPlayerColor.z,
                        transform.parent.parent.GetComponent<PhotonView>().ViewID);

                    StartCoroutine(WaitEndAnimation(transform.parent.parent, "inShoot"));
                }
            }
        }
    }

    [PunRPC]
    private void setPlayerColor(float _r, float _g, float _b, int idPlayer)
    {
        Transform player = FindPlayerByID(idPlayer);
        player.GetComponent<PlayerStatManager>().setPlayerColor(_r, _g, _b, idPlayer);
    }
}