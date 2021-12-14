using Photon.Pun;
using UnityEngine;


/**
 * Color Switch is a type of Equipement that allows the player to change their color to the color of another player in front of them
 */

public class ColorSwitch : Object
{
    private RaycastHit swap;
    //maw distance to reach in front of the player
    public int maxDistance;

    [PunRPC]
    protected override void CustomBehaviour()
    {
        //cast a ray in front of the player from the hit position to the max distance
        if (Physics.Raycast(HitObj.position,HitObj.forward, out swap, maxDistance))
        {
            if(swap.transform.tag == "Player" && swap.transform.GetComponent<PlayerStatManager>().isDead == false)// si le joueur n'est pas d�j� mort
            {
                if (player.GetComponent<PhotonView>().IsMine)
                {
                    Vector3 otherPlayerColor = swap.transform.GetComponent<PlayerStatManager>().playerColor;
                    //change color of player of photonViewID = viewID
                    photonView.RPC(nameof(setPlayerColor), RpcTarget.AllBuffered, otherPlayerColor.x,
                        otherPlayerColor.y, otherPlayerColor.z,
                        transform.parent.parent.GetComponent<PhotonView>().ViewID);
                    //load animation for the player and destroy object from the world
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