using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandPlayerColor : MonoBehaviour
{
    public int materialIndice;
    float r,g,b;
    void Start()
    {
        SkinnedMeshRenderer skinMeshRenderer = GetComponent<SkinnedMeshRenderer>();

        r = UnityEngine.Random.Range(0.000f, 1.000f);
        g = UnityEngine.Random.Range(0.000f, 1.000f);
        b = UnityEngine.Random.Range(0.000f, 1.000f);

        skinMeshRenderer.materials[materialIndice].color = new Color(r, g, b);
    }
}
