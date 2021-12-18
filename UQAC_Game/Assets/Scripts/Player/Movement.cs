using System;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;

/// <summary>
/// Manage movements of a player and stop an infinite fall<br/>
/// Rigidbody necessary
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class Movement : MonoBehaviourPun {
    public float defaultMoveSpeed = 5.0f;
    private float moveSpeed;
    private float previousMoveSpeed;
    public float sprintSpeed = 9.0f;
    public float rotationSpeed = 45.0f;
    private Vector3 movementDirection;
    private Vector3 previousMovementDirection;

    public float jumpForce = 180.0f;
    public bool isGrounded = false;

    public float dashSpeed = 15f;
    public static float defaultDashTime = 0.5f;
    private float dashTime = defaultDashTime;
    private bool inDash = false;
    public static float defaultDashCoolDown = 1.5f;
    private float dashCoolDown = defaultDashCoolDown;
    //You can totally disable the boost dash to initalise canDash to false

    public bool canDash = true;
    public bool inMove = false;
    public bool inJump = false;
    public bool inRun = false;
    public bool canRun = true; // modified by stamina
    private bool inWallCollide = false;

    public bool canMove;

    public Animator playerAnim;
    public Rigidbody rb;

    // Y limit to not fall under
    public float deathLimitY = -100.0f;
    
    void Start ()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (playerAnim == null) playerAnim = GetComponent<Animator>();
        moveSpeed = defaultMoveSpeed;
    }

    void Update () {
        
        // Prevent control is connected to Photon and represent the localPlayer
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) {
            return;
        }
        
        canMove = gameObject.GetComponent<PlayerStatManager>().canMove;

        if (canMove) {
            Move();
        }
        else
        {
            inRun = false;
            inJump = false;
            inMove = false;
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
        Animations();
        CheckDeathLimitY();
        
    }

    // use fixed update to update rigidbody (avoid different nbr of FPS between PC)
    // it coold be called multiple time per frame, so be careful with multiplication !
    private void FixedUpdate()
    {
        if (canMove)
        {
            // use temporary vector to avoid override previous value more than 1 time in this fixed update
            Vector3 tempMovementDirection = previousMovementDirection * Time.deltaTime * 1000.0f * previousMoveSpeed; // apply speed in m/s
            tempMovementDirection.y = rb.velocity.y; // get jump velocity
            rb.velocity = tempMovementDirection; // apply velocity (after move update)
        }
    }


    private void Move () {

        inMove = false;
        moveSpeed = defaultMoveSpeed;
        movementDirection = Vector3.zero;
        
        // Moves Keys
        // forward
        if (Input.GetKey(KeyCode.Z) /*|| Input.GetKey(KeyCode.UpArrow)*/)
        {
            inMove = true;
            movementDirection += Vector3.forward;
        }
        // backwards
        if (Input.GetKey(KeyCode.S) /*|| Input.GetKey(KeyCode.DownArrow)*/) {
            inMove = true;
            movementDirection += Vector3.back;
        }
        // left
        if (Input.GetKey(KeyCode.Q) /*|| Input.GetKey(KeyCode.LeftArrow)*/) {
            inMove = true;
            movementDirection += Vector3.left;
        }

        // right
        if (Input.GetKey(KeyCode.D) /*|| Input.GetKey(KeyCode.RightArrow)*/) {
            inMove = true;
            movementDirection += Vector3.right;
        }

        //// Rotate with Mouse
        if (Input.GetAxis("Mouse X") != 0 && Input.GetMouseButton(1)) { // mouse's left click
            transform.Rotate(Vector3.up , rotationSpeed * moveSpeed * Time.deltaTime * Input.GetAxis("Mouse X"));
        }

        // Jump (after velocity to add force without create super jump)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !inJump) {
            if (rb != null) {
                inJump = true;
                isGrounded = false;
                rb.AddForce(Vector3.up * 2.0f * jumpForce , ForceMode.Impulse);
                //StartCoroutine(ApplyForce(rb, Vector3.up * 2.0f * jumpForce, ForceMode.Impulse));
                                                                                              
            }
        }
        
        //// Sprint 
        //start
        if (canRun) {
            if (Input.GetKey(KeyCode.LeftShift) && inMove) {
                moveSpeed = sprintSpeed;
                inRun = true;
            }
            else
            {
                inRun = false;
            }
        } else {
            moveSpeed = defaultMoveSpeed;
            inRun = false;
        }
        //stop
        if (Input.GetKeyUp(KeyCode.LeftShift) && inRun) {
            moveSpeed = defaultMoveSpeed;
            inRun = false;
        }


        //// Dash
        if (Input.GetKeyDown(KeyCode.Alpha1) && canDash && inMove) {
            inDash = true;
            canDash = false;
        }
        if (inDash && !inWallCollide) // if not in collision with a wall
        {
            moveSpeed = dashSpeed;
            dashTime -= Time.deltaTime;
            if (dashTime < 0) { // if dash ended
                inDash = false;
                dashTime = defaultDashTime;
                moveSpeed = defaultMoveSpeed;
            }
        } else {
            if (dashCoolDown > 0) { // if in dash, decrease time
                dashCoolDown -= Time.deltaTime;
            } else if (!canDash) { // if can't dash: reset
                dashCoolDown = defaultDashCoolDown;
                canDash = true;
            }
        }
        previousMoveSpeed = moveSpeed;
        // appli movement (use rigidbody velocity because we had bugs (walks throughout walls) with translation at high speed (in run)
        movementDirection = transform.TransformDirection(movementDirection); // apply move with body rotation
        previousMovementDirection = movementDirection;
    }

    void Animations () {
        // in move
        if (inMove && canMove) {
            playerAnim.SetBool("isWalking" , true);

            if (canRun && inRun) { // Sprint
                playerAnim.SetBool("isRunning" , true);
            } else {
                playerAnim.SetBool("isRunning" , false);
            }

            if (inDash && !inWallCollide) { // Dash
                playerAnim.SetBool("inDash" , true);
            } else {
                playerAnim.SetBool("inDash" , false);
            }

        } else { // in stay mode
            playerAnim.SetBool("isWalking" , false);
            playerAnim.SetBool("isRunning" , false);
            playerAnim.SetBool("inDash" , false);
        }

        if (inJump) { // in Jump
            playerAnim.SetBool("inJump" , true);
        } else {
            playerAnim.SetBool("inJump" , false);
        }
    }

    void OnCollisionStay (Collision collision) {
        // if in collision with a wall
        if (collision.gameObject.CompareTag("Wall")) {
            inWallCollide = true;
        } else {
            inWallCollide = false;
        }
        // disable double jump or simple jump if not 2 foots on ground
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = true;
            inJump = false;
        }
    }
    void OnCollisionExit () {
        inWallCollide = false;
        isGrounded = false;
    }

    /// <summary>
    /// Check altitude to stop an infinite fall
    /// </summary>
    void CheckDeathLimitY () {
        
        if (transform.GetComponent<PhotonView>().IsMine)
        {
            // respawn if player fall too low
            if (transform.position.y < deathLimitY)
            {
                StartCoroutine(Respawn());
            }
        }
        
        // disable collider to enable quick respawn and avoid bugs with photon latency and avoid player stay under ground (because we had some collisions bugs with walls - fixed)
        if (transform.position.y < -4)
        {
            transform.GetComponent<Collider>().enabled = false;
        }
        else if (transform.position.y > -0.5)
        {
            transform.GetComponent<Collider>().enabled = true;
        }

    }

    // coroutine for respawn when fall out of the map
    IEnumerator Respawn()
    {
        gameObject.GetComponent<PlayerStatManager>().canMove = false;
        rb.isKinematic = true; // all force at 0
        transform.position = new Vector3(0, 2.5f, 0);
        yield return new WaitForSeconds(2);
        
        gameObject.GetComponent<PlayerStatManager>().canMove = true;
        rb.isKinematic = false; // all force at 0
        transform.GetComponent<Collider>().enabled = true;
    }

    IEnumerator ApplyForce(Rigidbody rb, Vector3 force, ForceMode forceMode) {
        yield return new WaitForFixedUpdate();
        inJump = true;
        isGrounded = false;
        rb.AddForce(force, forceMode);
        yield return new WaitForSeconds(1f);//WaitUntil(() => rb.velocity.y < 0);
        inJump = false;

        yield break;
    }

}
