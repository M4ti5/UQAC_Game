using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PlayerMove : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    //création des variables
    private Vector2 lastMousePosition;
    private bool collision = false;
    Collider collide = new Collider();
    bool victory = false;
    bool dragActive = false;
    float timer;
    public GameObject victoryText;
    Vector2 diff;


    // Start is called before the first frame update
    private void Start()
    {
        //Etre sûr que l'affichage du text de victoire est bien désactivé
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
        dragActive = true;
    }

    /// <summary>
    /// This method will be called during the mouse drag
    /// </summary>
    /// <param name="eventData">mouse pointer event data</param>
    public void OnDrag(PointerEventData eventData)
    {
        //Le déplacement est possible jusqu'à ce que le joueur atteigne l'arrivée
        if (!victory)
        {
            Vector2 currentMousePosition = eventData.position;
            diff = currentMousePosition - lastMousePosition;
            RectTransform rect = GetComponent<RectTransform>();

            //Si la distance de déplacement est trop grande, le joueur ne se déplace pas
            //Cela permet de limiter la possibilité du joueur de passer à travers un mur
            if (Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y) < rect.sizeDelta.y / 3)
            {
                rect.transform.Translate(new Vector3(diff.x, diff.y, transform.position.z));
            }

            //Temporisation pour actualiser collision via OnTriggerStay() si besoin
            while (timer < 10000)
            {
                timer += Time.deltaTime;
            }

            if (collision)
            {
                ActionOnCollision();
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
        dragActive = false;
    }

    //Appelé lorsque le joueur rentre en contact avec un autre collider
    private void OnTriggerStay(Collider other)
    {
        collision = true;
        Debug.Log("on trigger stay: " + other.name);
        collide = other;
        if (other.name == "Arrive")
        {
            victory = true;
        }
    }

    private void Update()
    {
        if (dragActive == false && victory == false)
        {
            RectTransform rect = GetComponent<RectTransform>();
            int moveSpeed = 3;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                rect.transform.Translate(new Vector3(0, moveSpeed, transform.position.z));
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                rect.transform.Translate(new Vector3(0, -moveSpeed, transform.position.z));
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                rect.transform.Translate(new Vector3(moveSpeed, 0, transform.position.z));
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                rect.transform.Translate(new Vector3(-moveSpeed, 0, transform.position.z));
            }


            if (collision)
            {
                Debug.Log("update: " + collide.name);
                ActionOnCollision();
            }
        }
        if (victory == true)
        {
            victoryText.SetActive(true);
        }
    }


    private void ActionOnCollision()
    {
        RectTransform rect = GetComponent<RectTransform>();
        //Si une collision est détectée, on regarde le type d'objet rencontré ainsi que la position du joueur par rapport 
        //à cet objet et on agit en conséquence
        collision = false;
        if ((collide.name == "Horizontal Wall(Clone)" || collide.name == "Horizontal External Wall(Clone)") && rect.position.y < collide.transform.position.y)
        {
            Debug.Log(rect.position + "     " + collide.name + "     " + collide.transform.position);
            rect.position = rect.position + new Vector3(0, -rect.sizeDelta.y / 2, 0);
        }
        if ((collide.name == "Horizontal Wall(Clone)" || collide.name == "Horizontal External Wall(Clone)") && rect.position.y > collide.transform.position.y)
        {
            Debug.Log(rect.position + "     " + collide.name + "     " + collide.transform.position);
            rect.position = rect.position + new Vector3(0, rect.sizeDelta.y / 2, 0);
        }
        if ((collide.name == "Vertical Wall(Clone)" || collide.name == "Vertical External Wall(Clone)") && rect.position.x < collide.transform.position.x)
        {
            Debug.Log(rect.position + "     " + collide.name + "     " + collide.transform.position);
            rect.position = rect.position + new Vector3(-rect.sizeDelta.x / 2, 0, 0);
        }
        if ((collide.name == "Vertical Wall(Clone)" || collide.name == "Vertical External Wall(Clone)") && rect.position.x > collide.transform.position.x)
        {
            Debug.Log(rect.position + "     " + collide.name + "     " + collide.transform.position);
            rect.position = rect.position + new Vector3(rect.sizeDelta.x / 2, 0, 0);
        }
        if (collide.name == "Entrance")
        {
            rect.position = rect.position + new Vector3(rect.sizeDelta.x / 2, 0, 0);
        }
    }


    
}
