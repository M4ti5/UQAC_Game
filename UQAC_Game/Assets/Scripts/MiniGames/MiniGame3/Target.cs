using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Target : MonoBehaviour, IPointerClickHandler
{
    public bool isClicked = false;
    

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(name + " Game Object Clicked!");
        if (gameObject.activeSelf)
        {
            isClicked = true;
            gameObject.SetActive(false);
        }
    }
}
