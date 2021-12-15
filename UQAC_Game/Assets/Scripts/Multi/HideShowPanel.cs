using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dynamique class to enable close/open panel as photonstatus on left side in debug mode enable in options)
/// </summary>
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
    [SerializeField] private RectTransform arrow;

    public bool reduced;

    // Start is called before the first frame update
    void Start()
    {
        if(parent == null)
            parent = transform.parent.GetComponent<RectTransform>();
        pos = parent.anchoredPosition3D;

        TransformParentPosition();
        RotateArrow();
    }
    public void ToggleHideShow()
    {
        reduced = !reduced;
        TransformParentPosition();
        RotateArrow();
    }

    private void TransformParentPosition()
    {
        // reduce panel = move position out of screen
        parent.anchoredPosition3D = new Vector3(
            pos.x + (parent.sizeDelta.x) * (reduced ? emplacement == Emplacement.Right ? 1 : emplacement == Emplacement.Left ? -1 : 0 : 0),
            pos.y + (parent.sizeDelta.y) * (reduced ? emplacement == Emplacement.Top ? 1 : emplacement == Emplacement.Bottom ? -1 : 0 : 0),
            pos.z);
    }

    // rotate arrow in the good angle depending on config and reduced or not
    private void RotateArrow()
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
        arrow.rotation = Quaternion.Euler(arrow.rotation.eulerAngles.x, arrow.rotation.eulerAngles.y, arrowAngle);
    }
}
