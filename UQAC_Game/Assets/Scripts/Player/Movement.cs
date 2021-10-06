using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

/// <summary>
/// Manage movements of a player and stop an infinite fall<br/>
/// Rigidbody necessary
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviourPun
{

    public float rotationSpeed = 1.0f;
    public float defaultMoveSpeed = 1.0f;
    public float sprintSpeed = 3.0f;
    private float moveSpeed;

    public Vector3 jump = new Vector3(0.0f, 2.0f, 0.0f);
    public float jumpForce = 2.0f;

    public bool isGrounded;
    private Rigidbody rb;

    /// <summary>
    /// y limit to not fall under
    /// </summary>
    public float deathLimitY = -100.0f;

    // Start is called before the first frame update  
    void Start()
    {
        Debug.Log("Start Movement script");
        // Set self rigidbody
        rb = GetComponent<Rigidbody>();
        // Set move speed
        moveSpeed = defaultMoveSpeed;
    }

    // Update is called once per frame  
    void Update()
    {
        // Prevent control is connected to Photon and represent the localPlayer
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        MoveRotate();
        Jump();
        CheckDeathLimitY();
        Sprint();
    }

    /// <summary>
    /// Check keys pressed and translate position of self object
    /// </summary>
    void MoveRotate()
    {
        // Move forward
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
        }

        // Move backwards
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.back * Time.deltaTime * moveSpeed);
        }

        // Rotate left
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.up, -rotationSpeed * moveSpeed);
        }

        // Rotate right
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up, rotationSpeed * moveSpeed);
        }
    }


    void OnCollisionStay()
    {
        isGrounded = true;
    }
    void OnCollisionExit()
    {
        isGrounded = false;
    }

    /// <summary>
    /// Jump when space key is pressed and when is not in the air
    /// </summary>
    void Jump()
    {
        // source : https://answers.unity.com/questions/1020197/can-someone-help-me-make-a-simple-jump-script.html

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // check if we have a rigidbody
            if (rb != null)
            {
                rb.AddForce(jump * jumpForce, ForceMode.Impulse);
                isGrounded = false;
            }
        }
    }

    /// <summary>
    /// Check altitude to stop an infinite fall
    /// </summary>
    void CheckDeathLimitY()
    {
        if (transform.localPosition.y < deathLimitY)
        {
            rb.isKinematic = true;// all force at 0
        }
    }

    /// <summary>
    /// Sprint when Left Shift key is pressing
    /// </summary>
    void Sprint()
    {
        // Sprint start
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            moveSpeed = sprintSpeed;
        }
        // Sprint stop
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = defaultMoveSpeed;
        }
    }

    // TODO:  ctrl ou c pour s'accoupir
}
