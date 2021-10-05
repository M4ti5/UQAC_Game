using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage movements of a player and stop an infinite fall<br/>
/// Rigidbody necessary
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour {
    private float defaultMoveSpeed = 5.0f;
    public float moveSpeed = 5.0f;
    public float sprintSpeed = 9.0f;
    public float rotationSpeed = 45.0f; 

    public float jumpForce = 180.0f;
    public bool isGrounded;

    private Rigidbody rb;

    /// <summary>
    /// Y limit to not fall under
    /// </summary>
    public float deathLimitY = -100.0f;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        Move();
        CheckDeathLimitY();
    }


    void Move() {

        //// Moves Key
        // forward
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)) {
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
        }
        // backwards
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            transform.Translate(Vector3.back * Time.deltaTime * moveSpeed);
        }
        // left
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) {
            transform.Translate(Vector3.left * Time.deltaTime * moveSpeed);
        }

        // right
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
        }

        //// Rotate Mouse
        if (Input.GetAxis("Mouse X") != 0 && Input.GetMouseButton(1)) { // mouse's left click
            transform.Rotate(Vector3.up, rotationSpeed * moveSpeed * Time.deltaTime * Input.GetAxis("Mouse X"));
        }

        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
            if (rb != null) {
                rb.AddForce(Vector3.up * 2.0f * jumpForce, ForceMode.Impulse);
                isGrounded = false;
            }
        }

        //Sprint start
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            moveSpeed = sprintSpeed;
        }
        //Sprint stop
        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            moveSpeed = defaultMoveSpeed;
        }

        // TODO:  ctrl ou c pour s'accoupir

    }

    void OnCollisionStay(Collision collision) {
        if (collision.gameObject.tag == "Wall") {
            //Stop l'annimation
        }
        isGrounded = true;
    }
    void OnCollisionExit() {
        isGrounded = false;
    }

    /// <summary>
    /// Check altitude to stop an infinite fall
    /// </summary>
    void CheckDeathLimitY() {
        if (transform.localPosition.y < deathLimitY) {
            rb.isKinematic = true;// all force at 0
        }
    }



}
