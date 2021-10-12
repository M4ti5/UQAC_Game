using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnWallCollision : MonoBehaviour
{
    private bool collision = false;
    private Vector3 position;
    int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Destroy(gameObject);
    }
}
