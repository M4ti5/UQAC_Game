using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseDragBehaviour2 : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 lastMousePosition;
    private bool collision = false;
    Vector3 oldPos = new Vector3(0, 0, 0);
    float timer;


    /// <summary>
    /// This method will be called on the start of the mouse drag
    /// </summary>
    /// <param name="eventData">mouse pointer event data</param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");
        lastMousePosition = eventData.position;
    }

    /// <summary>
    /// This method will be called during the mouse drag
    /// </summary>
    /// <param name="eventData">mouse pointer event data</param>
    public void OnDrag(PointerEventData eventData)
    {
        timer = 0;

        Vector2 currentMousePosition = eventData.position;
        Vector2 diff = currentMousePosition - lastMousePosition;
        RectTransform rect = GetComponent<RectTransform>();

        Vector3 newPosition = rect.position + new Vector3(diff.x, diff.y, transform.position.z);
        oldPos = rect.position;
        rect.position = newPosition;
        Debug.Log("test1" + collision);
        while(timer < 20000)
        {
            timer += Time.deltaTime;
            //Debug.Log(timer);
        }
        Debug.Log("test2   " + Mathf.Sqrt(diff.x*diff.x + diff.y*diff.y));

        if (collision || Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y)>10)
        {
            Debug.Log("déplacement" + oldPos);
            rect.position = oldPos;
            collision = false;
        }



        lastMousePosition = currentMousePosition;
    }

    



    /// <summary>
    /// This method will be called at the end of mouse drag
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");
        //Implement your funtionlity here
    }

    /// <summary>
    /// This methods will check is the rect transform is inside the screen or not
    /// </summary>
    /// <param name="rectTransform">Rect Trasform</param>
    /// <returns></returns>
    private bool IsRectTransformInsideSreen(RectTransform rectTransform)
    {
        bool isInside = false;
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        int visibleCorners = 0;
        Rect rect = new Rect(0, 0, Screen.width, Screen.height);
        foreach (Vector3 corner in corners)
        {
            if (rect.Contains(corner))
            {
                visibleCorners++;
            }
        }
        if (visibleCorners == 4)
        {
            isInside = true;
        }
        return isInside;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("enter" + oldPos);
        collision = true;
        RectTransform rect = GetComponent<RectTransform>();
        rect.position = oldPos;
    }

   

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exit" + oldPos);
        collision = false;
        RectTransform rect = GetComponent<RectTransform>();
        rect.position = oldPos;
    }

}
