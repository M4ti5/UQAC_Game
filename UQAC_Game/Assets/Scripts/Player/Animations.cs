using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour
{
    private Animator playerAnim;
    void Start () {
        playerAnim = transform.GetComponent<Animator>();
    }

    public void DeathAnim () {
        if (gameObject.GetComponent<PlayerStatManager>().isDead) {
            playerAnim.SetBool("isDead" , true);
        }
    }

    public void HitAnim () {
        playerAnim.Play("Hit");
    }

    /* Désactivé car effet pas jolie
     public void GrabAnim () {
        playerAnim.Play("Grab");
    } 
    */

}
