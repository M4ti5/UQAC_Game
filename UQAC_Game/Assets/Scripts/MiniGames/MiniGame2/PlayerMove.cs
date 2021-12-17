using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


public class PlayerMove : MonoBehaviour
{
    //Variables creation
    bool victory = false;
    private float update;
    private Vector3 previousPlayerPos;

    public GameObject victoryText;
    RectTransform rect;

    private Vector3 canvaScale;

    // Start is called before the first frame update
    private void Start()
    {
        //Be sure that the victory text in desactivated
        victoryText.SetActive(false);

        //get the scale of the canva for adapting to the screen size
        canvaScale = gameObject.transform.parent.parent.localScale;

        //get RectTransform of the player
        rect = GetComponent<RectTransform>();
    }

    IEnumerator EndMiniGame()
    {
        //Activate the victory text
        victoryText.SetActive(true);
        yield return new WaitForSeconds(1);
        //After 1 seconde, desactivate the gameObject
        gameObject.transform.parent.parent.parent.gameObject.SetActive(false);
    }

    //When the player go into a Wall
    private void OnTriggerStay(Collider other)
    {
        //If the player go into the arrive wall, the mini game end
        //Else the player go back to his previous position
        if (other.name == "Arrive" && rect.name == "Player")
        {
            victory = true;
        }
        else
        {
            rect.anchoredPosition = previousPlayerPos;
        }
    }

    private void Update()
    {
        update += Time.deltaTime;
        if (update > 1.0f/40f)
        {
            update = 0.0f;
            if (victory == false)
            {
                //Get the position of the player before the translation
                previousPlayerPos = rect.anchoredPosition;
                int moveSpeed = 10;

                //Move the player depending on the key used by the player
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    //rect.transform.localPosition += new Vector3(0, moveSpeed, 0);
                    rect.anchoredPosition += new Vector2(0, moveSpeed * canvaScale.y);
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    //rect.transform.localPosition += new Vector3(0, -moveSpeed, 0);
                    rect.anchoredPosition += new Vector2(0, -moveSpeed * canvaScale.y);
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    //rect.transform.localPosition += new Vector3(moveSpeed, 0, 0);
                    rect.anchoredPosition += new Vector2(moveSpeed * canvaScale.x, 0);
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    //rect.transform.localPosition += new Vector3(-moveSpeed, 0, 0);
                    rect.anchoredPosition += new Vector2(-moveSpeed * canvaScale.x, 0);
                }
            }
            if (victory == true && !victoryText.activeSelf)
            {
                StartCoroutine(EndMiniGame());
            }
        }
    } 
}