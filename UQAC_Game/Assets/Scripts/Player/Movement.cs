using System.Collections;
using Photon.Pun;
using UnityEngine;

/// <summary>
/// Manage movements of a player and stop an infinite fall<br/>
/// Rigidbody necessary
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviourPun {
    public float defaultMoveSpeed = 5.0f;
    private float moveSpeed;
    public float sprintSpeed = 9.0f;
    public float rotationSpeed = 45.0f;
    public Rigidbody rigidbody;

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

    private Animator playerAnim;
    private Rigidbody rb;

    /// <summary>
    /// Y limit to not fall under
    /// </summary>
    public float deathLimitY = -100.0f;

    void Start () {
        rb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        moveSpeed = defaultMoveSpeed;
    }

    // Update is called once per frame  
    void Update () {

        canMove = gameObject.GetComponent<PlayerStatManager>().canMove;

        // Prevent control is connected to Photon and represent the localPlayer
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) {
            return;
        }

        if (canMove) {
            Move();
        }
        else
        {
            inRun = false;
            inJump = false;
            inMove = false;
            rigidbody.velocity = Vector3.zero;
        }
        Animations();
        CheckDeathLimitY();
    }


    void Move () {

        //// Moves Key
        inMove = false;
        Vector3 movementDirection = Vector3.zero;
        
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

        //// Rotate Mouse
        if (Input.GetAxis("Mouse X") != 0 && Input.GetMouseButton(1)) { // mouse's left click
            transform.Rotate(Vector3.up , rotationSpeed * moveSpeed * Time.deltaTime * Input.GetAxis("Mouse X"));
        }

        //// Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
            if (rb != null) {
                rb.AddForce(Vector3.up * 2.0f * jumpForce , ForceMode.Impulse);
                isGrounded = false;
                inJump = true;
            }
        }

        //// Sprint 
        //start
        if (canRun) {
            if (Input.GetKey(KeyCode.LeftShift) && inMove) {
                moveSpeed = sprintSpeed;
                inRun = true;
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
        if (inDash && !inWallCollide)
        {
            moveSpeed = dashSpeed;
            dashTime -= Time.deltaTime;
            if (dashTime < 0) {
                inDash = false;
                dashTime = defaultDashTime;
                moveSpeed = defaultMoveSpeed;
            }
        } else {
            if (dashCoolDown > 0) {
                dashCoolDown -= Time.deltaTime;
            } else if (!canDash) {
                dashCoolDown = defaultDashCoolDown;
                canDash = true;
            }
        }
        
        // appli movement
        movementDirection = transform.TransformDirection(movementDirection); // apply move with body rotation
        movementDirection *= Time.deltaTime * 1000.0f * moveSpeed; // apply speed in m/s
        movementDirection.y = rigidbody.velocity.y; // get jump velocity
        rigidbody.velocity = movementDirection; // apply velocity

    }

    void Animations () {

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

        } else {
            playerAnim.SetBool("isWalking" , false);
            playerAnim.SetBool("isRunning" , false);
            playerAnim.SetBool("inDash" , false);
        }

        if (inJump) { // Jump
            playerAnim.SetBool("inJump" , true);
        } else {
            playerAnim.SetBool("inJump" , false);
        }
    }

    void OnCollisionStay (Collision collision) {

        if (collision.gameObject.CompareTag("Wall")) {
            playerAnim.SetBool("inCollide" , true);
            playerAnim.SetBool("isWalking" , false);
            inWallCollide = true;
        } else {
            playerAnim.SetBool("inCollide" , false);
            inWallCollide = false;
        }

        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = true;
            inJump = false;
        }
    }
    void OnCollisionExit () {
        isGrounded = false;
    }

    /// <summary>
    /// Check altitude to stop an infinite fall
    /// </summary>
    void CheckDeathLimitY () {
        if (transform.GetComponent<PhotonView>().IsMine)
        {
            if (transform.position.y < deathLimitY)
            {
                StartCoroutine(Respawn());
            }
        }
        
        if (transform.position.y < -4)
        {
            transform.GetComponent<Collider>().enabled = false;
        }
        else if (transform.position.y > -0.5)
        {
            transform.GetComponent<Collider>().enabled = true;
        }

    }

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



}
