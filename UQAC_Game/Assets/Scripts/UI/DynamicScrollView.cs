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


    // Update is called once per frame
    void Update()
    {
        float height = textMeshPro.preferredHeight +
                       (-textMeshPro.GetComponent<RectTransform>().offsetMax.y) + // top
                       textMeshPro.GetComponent<RectTransform>().offsetMin.y; // bottom
        if (height < content.parent.GetComponent<RectTransform>().rect.height)
        {
            height = content.parent.GetComponent<RectTransform>().rect.height;
        }
        
        content.sizeDelta = new Vector2(content.sizeDelta.x, height);
    }
}
