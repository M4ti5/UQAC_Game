using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideShowPanel : MonoBehaviour
{

    enum Emplacement{
        Left,
        Right,
        Top,
        Bottom
    };
    [SerializeField] private Emplacement emplacement;
    [SerializeField] private RectTransform parent;
    private Vector3 pos;
    [SerializeField] private RectTransform Arrow;

    public bool reduced;
    //private float btnHeight = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        /*if(emplacement == Emplacement.Right || emplacement == Emplacement.Left)
            btnHeight = transform.GetComponent<RectTransform>().rect.width;
        else
            btnHeight = transform.GetComponent<RectTransform>().rect.height;
        */
        if(parent == null)
            parent = transform.parent.GetComponent<RectTransform>();
        pos = parent.anchoredPosition3D;

        transformParentPosition();
        rotateArrow();
    }
    public void toggleHideShow()
    {
        reduced = !reduced;
        transformParentPosition();
        rotateArrow();
    }

    private void transformParentPosition()
    {
        parent.anchoredPosition3D = new Vector3(
            pos.x + (parent.sizeDelta.x) * (reduced ? emplacement == Emplacement.Right ? 1 : emplacement == Emplacement.Left ? -1 : 0 : 0),
            pos.y + (parent.sizeDelta.y) * (reduced ? emplacement == Emplacement.Top ? 1 : emplacement == Emplacement.Bottom ? -1 : 0 : 0),
            pos.z);
    }

    private void rotateArrow()
    {
        float arrowAngle = 0f;
        switch (emplacement)
        {
            case Emplacement.Right:
                arrowAngle = reduced ? -90 : 90;
                break;
            case Emplacement.Left:
                arrowAngle = reduced ? 90 : -90;

                break;
            case Emplacement.Top:
                arrowAngle = reduced ? 0 : 180;

                break;
            case Emplacement.Bottom:
                arrowAngle = reduced ? 180 : 0;
                break;
        }
        Arrow.rotation = Quaternion.Euler(Arrow.rotation.eulerAngles.x, Arrow.rotation.eulerAngles.y, arrowAngle);
    }
}
