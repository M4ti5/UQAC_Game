
using Photon.Pun;
using UnityEngine;

public class Cut : Object
{
    //max distance that can be reached (z axis)
    public float distanceToCut = 1.5f;
    //max distance in termsof wideness (x axis)
    public float rangeCut = 1;
    private RaycastHit hit;
    bool m_HitDetect;
    public int damage = 10;
    
    [PunRPC]
    protected override void CustomBehaviour()
    {
        //set param
        Vector3 center = HitObj.position;
        Vector3 halfExtents = new Vector3((rangeCut / 2), 0.25f, 0.1f);
        Vector3 direction = HitObj.forward;
        Quaternion orientation = HitObj.rotation;
        
        m_HitDetect = Physics.BoxCast(center, halfExtents, direction, out hit, orientation, distanceToCut);
        if (m_HitDetect)
        {
            if (hit.transform.tag == "Player" && hit.transform.GetComponent<PlayerStatManager>().isDead == false)// si le joueur n'est pas déjà mort
            {
                //hit.transform.GetComponent<PlayerStatManager>().TakeDamage(damage, hit.transform.GetComponent<PhotonView>().ViewID);
                photonView.RPC(nameof(PlayerStatManager.TakeDamage), RpcTarget.AllBuffered, damage, hit.transform.GetComponent<PhotonView>().ViewID);
                ObjectUsed();
            }
        }

    }
}
