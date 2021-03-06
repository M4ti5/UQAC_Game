using Photon.Pun;
using UnityEngine;
/**
 * Script that implements the behavior of the Pan object
 * the player takes deals damage to a player in a box in front of them.
 */
public class Pan : Object
{
    //max distance that can be reached (z axis)
    public float distanceToHit = 2;
    //max distance in termsof wideness (x axis)
    public float rangeHit = 1;
    private RaycastHit hit;
    bool m_HitDetect;
    public int damage = 10;
    
    [PunRPC]
    protected override void CustomBehaviour()
    {
        //set param
        Vector3 center = HitObj.position;
        Vector3 halfExtents = new Vector3((rangeHit / 2), 0.25f, 0.1f);
        Vector3 direction = HitObj.forward;
        Quaternion orientation = HitObj.rotation;
        
        //cast a box between hit point in front of the player to the max distance to cut and detect collision
        m_HitDetect = Physics.BoxCast(center, halfExtents, direction, out hit, orientation, distanceToHit);
        if (m_HitDetect)
        {
            if (hit.transform.tag == "Player" && hit.transform.GetComponent<PlayerStatManager>().isDead == false)// si le joueur n'est pas d�j� mort
            {
                if (player.GetComponent<PhotonView>().IsMine)
                {
                    //synchro for all players - a player takes damage
                    photonView.RPC(nameof(TakeDamage), RpcTarget.AllBufferedViaServer, damage,
                        hit.transform.GetComponent<PhotonView>().ViewID);
                    //start animation and destroy object
                    StartCoroutine(WaitEndAnimation(transform.parent.parent, "inCut"));
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