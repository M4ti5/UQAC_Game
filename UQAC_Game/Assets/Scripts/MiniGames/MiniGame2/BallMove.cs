using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class BallMove : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // Start is called before the first frame update
    private Vector2 lastMousePosition;
    private bool collision = false;
    Collider collide = new Collider();
    bool victory = false;
    float timer;
    public GameObject victoryText;

    Vector2 diff;


    private void Start()
    {
        victoryText.SetActive(false);
    }

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
        if (!victory)
        {
            Vector2 currentMousePosition = eventData.position;
            diff = currentMousePosition - lastMousePosition;
            RectTransform rect = GetComponent<RectTransform>();


            if (Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y) < rect.sizeDelta.y / 3)
            {
                rect.transform.Translate(new Vector3(diff.x, diff.y, transform.position.z));
            }

            while (timer < 10000)
            {
                timer += Time.deltaTime;
            }

            if (collision)
            {

                collision = false;
                if ((collide.name == "Horizontal Wall(Clone)" || collide.name == "Horizontal External Wall(Clone)") && rect.position.y < collide.transform.position.y)
                {
                    Debug.Log(rect.position + "     " + collide.name + "     " + collide.transform.position);
                    rect.position = rect.position + new Vector3(0, -rect.sizeDelta.y / 2, 0);
                }
                else if ((collide.name == "Horizontal Wall(Clone)" || collide.name == "Horizontal External Wall(Clone)") && rect.position.y > collide.transform.position.y)
                {
                    Debug.Log(rect.position + "     " + collide.name + "     " + collide.transform.position);
                    rect.position = rect.position + new Vector3(0, rect.sizeDelta.y / 2, 0);
                }
                else if ((collide.name == "Vertical Wall(Clone)" || collide.name == "Vertical External Wall(Clone)") && rect.position.x < collide.transform.position.x)
                {
                    Debug.Log(rect.position + "     " + collide.name + "     " + collide.transform.position);
                    rect.position = rect.position + new Vector3(-rect.sizeDelta.x / 2, 0, 0);
                }
                else if ((collide.name == "Vertical Wall(Clone)" || collide.name == "Vertical External Wall(Clone)") && rect.position.x > collide.transform.position.x)
                {
                    Debug.Log(rect.position + "     " + collide.name + "     " + collide.transform.position);
                    rect.position = rect.position + new Vector3(rect.sizeDelta.x / 2, 0, 0);
                }
                else if (collide.name == "Entrance")
                {
                    rect.position = rect.position + new Vector3(rect.sizeDelta.x / 2, 0, 0);
                }
            }
            lastMousePosition = currentMousePosition;
        }
        else
        {
            Debug.Log("victory");
            victoryText.SetActive(true);

        }
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

    //private void OnTriggerEnter(Collider other)
    //{
    //    RectTransform rect = GetComponent<RectTransform>();
    //    rect.transform.Translate(new Vector3(-diff.x*5, -diff.y*5, 0));
    //}

    private void OnTriggerStay(Collider other)
    {
        collision = true;
        collide = other;
        if (other.name == "Arrive")
        {
            victory = true;
        }
    }

}
