using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Script that describes behavior of a target in the minigame Targets
/// When a target is clicked, the target disappears
/// </summary>
public class Target : MonoBehaviour, IPointerClickHandler
{
    public bool isClicked = false;
    //click listener implementation
    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameObject.activeSelf)
        {
            isClicked = true;
            gameObject.SetActive(false);
        }
    }
}
