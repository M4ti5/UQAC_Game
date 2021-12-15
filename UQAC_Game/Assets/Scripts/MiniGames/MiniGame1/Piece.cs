using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// Manage one piece of a puzzle game
/// </summary>
public class Piece : MonoBehaviour
{
    /// <summary>
    /// Save right position
    /// </summary>
    private Vector3 correctPosition;
    /// <summary>
    /// Pop up zone
    /// </summary>
    public GameObject tempZone;
    /// <summary>
    /// Canvas to get scale factor
    /// </summary>
    public Canvas canvas;

    /// <summary>
    /// Is piece in the right position
    /// </summary>
    public bool inRightPosition;
    /// <summary>
    /// Distance between mouse position and right position
    /// </summary>
    public float precision = 20f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start Piece script");

        // save right position
        correctPosition = transform.localPosition;
        
        // move piece in reserve zone
        RandomStartPosition();
    }

    // Update is called once per frame
    void Update()
    {
        // If not in place : check distance
        if(inRightPosition == false)
            CheckCorrectionPosition();
    }

    /// <summary>
    /// Set a random position in the pop up zone<br/>
    /// (By getting canvas scale factor and position & size of popup zone)
    /// </summary>
    void RandomStartPosition()
    {
        // Get temporaly recttransform of the pop up zone 
        RectTransform tempZoneRectTransform = tempZone.GetComponent<RectTransform>();
        // Set random position
        transform.localPosition = new Vector3(
            Random.Range(
                tempZoneRectTransform.localPosition.x - transform.parent.parent.localPosition.x - (tempZone.GetComponent<RectTransform>().rect.width) / 2f + (GetComponent<RectTransform>().sizeDelta.x) / 2f,
                tempZoneRectTransform.localPosition.x - transform.parent.parent.localPosition.x + (tempZone.GetComponent<RectTransform>().rect.width) / 2f - (GetComponent<RectTransform>().sizeDelta.x) / 2f
                ),
            Random.Range(
                tempZoneRectTransform.localPosition.y - transform.parent.parent.localPosition.y - (tempZone.GetComponent<RectTransform>().rect.height) / 2f + (GetComponent<RectTransform>().sizeDelta.y) / 2f ,
                tempZoneRectTransform.localPosition.y - transform.parent.parent.localPosition.y + (tempZone.GetComponent<RectTransform>().rect.height) / 2f - (GetComponent<RectTransform>().sizeDelta.y) / 2f - (tempZone.GetComponent<RectTransform>().rect.height * 0.1f)
                ),
            1f
            );
        // Reset right position indicator
        inRightPosition = false; 
        
    }

    /// <summary>
    /// Check distance between mouse position and right position
    /// </summary>
    void CheckCorrectionPosition()
    {
        if (Vector2.Distance(transform.localPosition, correctPosition) < precision)
        {
            inRightPosition = true;
            transform.localPosition = correctPosition;
            // Disable script use to drag piece
            GetComponent<MouseDragBehaviour>().enabled = false;
        }
    }
}
