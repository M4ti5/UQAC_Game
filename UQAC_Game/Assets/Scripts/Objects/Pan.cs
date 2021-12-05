using Photon.Pun;
using UnityEngine;

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
        
        m_HitDetect = Physics.BoxCast(center, halfExtents, direction, out hit, orientation, distanceToHit);
        if (m_HitDetect)
        {
            if (hit.transform.tag == "Player")
            {
                hit.transform.GetComponent<PlayerStatManager>().TakeDamage(damage);
                ObjectUsed();
            }
        }

    }
}