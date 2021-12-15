using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public new bool enabled = true;

    [Tooltip("The local instance. Use this to know if the local gameobject is already represented in the Scene")]
    public static GameObject instance;

    private void Awake()
    {
        if (enabled)
        {
            if (instance == null)
            {
                instance = gameObject;
                DontDestroyOnLoad(gameObject);
                return;
            }
            if (instance == this) return;
            Destroy(gameObject);
        }
    }
}
