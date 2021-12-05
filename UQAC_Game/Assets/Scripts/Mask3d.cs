using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mask3d : MonoBehaviour {

    void Start () {
        GetComponent<MeshRenderer>().material.renderQueue = 3002;
    }

}
