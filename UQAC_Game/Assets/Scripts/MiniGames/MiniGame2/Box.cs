using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Box : MonoBehaviour
{
    public bool up;
    public bool down;
    public bool right;
    public bool left;
    public int number;


    // Start is called before the first frame update
    void Start()
    {
        up = false;
        down = false;
        right = false;
        left = false;
    }
}
