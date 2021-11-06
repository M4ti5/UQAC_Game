using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class UseObject : MonoBehaviour
{

    public bool hasObject = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start UseObject script");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && hasObject)
        {
            //add equipement behavior script
            this.transform.GetChild(0).GetComponent<Object>().Behaviour();// utiliser l'objet
            //this.transform.GetChild(0).GetComponent<Object>().DestroyObject(PhotonNetwork.LocalPlayer);// détruire l'objet
        }

    }
}
