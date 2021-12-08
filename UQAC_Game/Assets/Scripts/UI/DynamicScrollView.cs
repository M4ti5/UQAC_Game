using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DynamicScrollView : MonoBehaviour
{
    //public RectTransform viewPort;
    public RectTransform content;
    public RectTransform horizontalScrollBar;
    public RectTransform verticalScrollBar;
    public TextMeshProUGUI textMeshPro;


    // Start is called before the first frame update
    void Start()
    {
        print("textMeshPro.fontSize :"+textMeshPro.fontSize +"\n"+
              "textMeshPro.textInfo.lineCount :"+textMeshPro.textInfo.lineCount +"\n"+
              "content.sizeDelta.x :"+content.sizeDelta.x +"\n"+
              "content.sizeDelta.y :"+content.sizeDelta.y +"\n"+
              "-textMeshPro.GetComponent<RectTransform>().offsetMax.y :"+-textMeshPro.GetComponent<RectTransform>().offsetMax.y +"\n"+
              "textMeshPro.GetComponent<RectTransform>().offsetMin.y :"+textMeshPro.GetComponent<RectTransform>().offsetMin.y +"\n"
              );
    }

    // Update is called once per frame
    void Update()
    {
        content.sizeDelta = new Vector2(content.sizeDelta.x, 
            textMeshPro.fontSize * textMeshPro.textInfo.lineCount + 
            (-textMeshPro.GetComponent<RectTransform>().offsetMax.y) + // top
            textMeshPro.GetComponent<RectTransform>().offsetMin.y); // bottom
        /*float sizeViewPort =
            content.rect.height + horizontalScrollBar.rect.height;
        sizeViewPort > 
        viewPort.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            sizeViewPort
        );*/
    }
}
