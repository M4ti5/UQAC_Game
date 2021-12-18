using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// controle content height of scroll view depending on minimal size of parent and number of lines in textMeshPro
public class DynamicScrollView : MonoBehaviour
{
    public RectTransform content;
    public RectTransform horizontalScrollBar;
    public RectTransform verticalScrollBar;
    public TextMeshProUGUI textMeshPro;


    // Update is called once per frame
    void Update()
    {
        // get minimal size depanding on number of lines (with space)
        float height = textMeshPro.preferredHeight +
                       (-textMeshPro.GetComponent<RectTransform>().offsetMax.y) + // top
                       textMeshPro.GetComponent<RectTransform>().offsetMin.y; // bottom
        // minimum size is parent viewport
        if (height < content.parent.GetComponent<RectTransform>().rect.height)
        {
            height = content.parent.GetComponent<RectTransform>().rect.height;
        }
        // update size of the content section
        content.sizeDelta = new Vector2(content.sizeDelta.x, height);
    }
}
