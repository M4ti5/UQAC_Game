
using Photon.Pun;
using UnityEngine;

/**
 * Script that implements the behavior of the Gun object
 * the player takes deals damage to a player in a ray in front of them.
 */
public class Gun : Object
{
    private RaycastHit hit;
    public int maxDistance;

    public int damage;

    [PunRPC]
    protected override void CustomBehaviour(){
     
        //cast a ray in front of the player to a max distance
        if(Physics.Raycast(HitObj.position, HitObj.forward, out hit, maxDistance)){
    
            if(hit.transform.tag == "Player" && hit.transform.GetComponent<PlayerStatManager>().isDead == false)// si le joueur n'est pas d�j� mort
            {
                if (player.GetComponent<PhotonView>().IsMine)
                {
                    //synchro for all players - a player takes damage
                    photonView.RPC(nameof(TakeDamage), RpcTarget.AllBuffered, damage,
                        hit.transform.GetComponent<PhotonView>().ViewID);
                    //launch animation and destroy object
                    StartCoroutine(WaitEndAnimation(transform.parent.parent, "inShoot"));
                }
            }
        }
        
    }

    //deals damage to a player via the player viewID
    [PunRPC]
    private void TakeDamage(int damage, int viewId)
    {
        Transform player = FindPlayerByID(viewId);
        player.GetComponent<PlayerStatManager>().TakeDamage(damage, viewId);
    }
}
