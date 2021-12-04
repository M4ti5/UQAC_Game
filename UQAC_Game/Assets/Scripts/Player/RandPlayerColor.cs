using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView), typeof(SkinnedMeshRenderer))]
public class RandPlayerColor : MonoBehaviourPun
{
    public int materialIndice;
    float r, g, b;
    void Start()
    {
        SetRandomSkinColor();
    }

    public void SetRandomSkinColor()
    {
        if (photonView.IsMine)
        {
            r = UnityEngine.Random.Range(0.000f, 1.000f);
            g = UnityEngine.Random.Range(0.000f, 1.000f);
            b = UnityEngine.Random.Range(0.000f, 1.000f);

            photonView.RPC(nameof(RandomSkinColor), RpcTarget.AllBuffered, r, g, b);
        }
    }

    [PunRPC]
    private void RandomSkinColor(float _r, float _g, float _b)
    {
        transform.parent.parent.GetComponent<PlayerStatManager>().setPlayerColor(_r, _g, _b);
    }

    public void setSkinColor(float _r, float _g, float _b)
    {
        SkinnedMeshRenderer skinMeshRenderer = GetComponent<SkinnedMeshRenderer>();

        if (skinMeshRenderer != null)
        {
            Vector3 playerColor = new Vector3(r, g, b);
            transform.parent.parent.GetComponent<PlayerStatManager>().playerColor = playerColor;
            skinMeshRenderer.materials[materialIndice].color = new Color(_r, _g, _b);
        }
    }
}
