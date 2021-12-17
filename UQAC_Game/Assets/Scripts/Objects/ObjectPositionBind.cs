using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Set dynamically position and rotation when object is used in animation
 */
public class ObjectPositionBind : MonoBehaviourPun {

    public GameObject bindRef;
    public int x = 100;
    public int y = -35;
    public int z = 0;

    private void Update () {
        transform.position = bindRef.transform.position;
        transform.rotation = Quaternion.Euler(bindRef.transform.rotation.eulerAngles);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + x , transform.rotation.eulerAngles.y + y , transform.rotation.eulerAngles.z + z);
    }


}
