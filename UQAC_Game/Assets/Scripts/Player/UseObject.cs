using System.Collections;
using System.Collections.Generic;
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
        if(Input.GetKeyUp(KeyCode.E) && hasObject){
            //add equipement behavior script
            Destroy(this.transform.GetChild(0).gameObject);
            hasObject = false;
        }

    }
}
