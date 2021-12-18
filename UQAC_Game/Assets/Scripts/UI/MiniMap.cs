using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public RenderTexture mapTexture;
    [SerializeField] private RawImage miniMaptexture, bigMiniMaptexture;
    [SerializeField] private Transform bigMiniMapParent;

    // Start is called before the first frame update
    private void Awake()
    {
        OnBigMiniMapClick();//disable big map
    }

    // toggle big map
    public void OnMiniMapClick()
    {
        bigMiniMapParent.parent.gameObject.SetActive(!bigMiniMapParent.parent.gameObject.activeSelf);
    }

    // hide big map
    public void OnBigMiniMapClick()
    {
        bigMiniMapParent.parent.gameObject.SetActive(false);
    }
}
