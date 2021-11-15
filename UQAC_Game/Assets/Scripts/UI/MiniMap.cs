using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public RenderTexture mapTexture;
    [SerializeField] private RawImage miniMaptexture, bigMiniMaptexture;
    private Transform bigMiniMapParent;

    // Start is called before the first frame update
    private void Awake()
    {
        bigMiniMapParent = bigMiniMaptexture.transform.parent;

        OnBigMiniMapClick();//disable big map

    }

    public void SetTexture(RenderTexture texture)
    {
        mapTexture = texture;
        miniMaptexture.texture = mapTexture;
        SizeToParent(miniMaptexture);

        bigMiniMapParent.gameObject.SetActive(true);//hide big map
        bigMiniMapParent.GetComponent<AspectRatioFitter>().aspectRatio = mapTexture.width / (float)mapTexture.height;
        bigMiniMaptexture.texture = mapTexture;
        SizeToParent(bigMiniMaptexture);
        bigMiniMapParent.gameObject.SetActive(false);//hide big map

    }

    public void OnMiniMapClick()
    {
        bigMiniMapParent.gameObject.SetActive( !bigMiniMapParent.gameObject.activeSelf);//toogle big map
    }

    public void OnBigMiniMapClick()
    {
        bigMiniMapParent.gameObject.SetActive(false);//hide big map
    }

    //https://forum.unity.com/threads/code-snippet-size-rawimage-to-parent-keep-aspect-ratio.381616/
    public static Vector2 SizeToParent(RawImage image, float padding = 0)
    {
        float w = 0, h = 0;
        var parent = image.GetComponentInParent<RectTransform>();
        var imageTransform = image.GetComponent<RectTransform>();

        // check if there is something to do
        if (image.texture != null)
        {
            if (!parent) { return imageTransform.sizeDelta; } //if we don't have a parent, just return our current width;
            padding = 1 - padding;
            float ratio = image.texture.width / (float)image.texture.height;
            var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);
            if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90)
            {
                //Invert the bounds if the image is rotated
                bounds.size = new Vector2(bounds.height, bounds.width);
            }
            //Size by height first
            h = bounds.height * padding;
            w = h * ratio;
            if (w > bounds.width * padding)
            { //If it doesn't fit, fallback to width;
                w = bounds.width * padding;
                h = w / ratio;
            }
        }
        imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        return imageTransform.sizeDelta;
    }
}
