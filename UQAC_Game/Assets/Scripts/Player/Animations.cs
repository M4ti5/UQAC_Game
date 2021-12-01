using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour
{
    private Animator playerAnim;
    void Start () {
        
        playerAnim = GetComponent<Animator>();

        //Perpendicularized the player
        transform.rotation = Quaternion.Euler(new Vector3(0 , transform.rotation.eulerAngles.y , 0));
    }

    public void DeathAnim () {
        if (gameObject.GetComponent<PlayerStatManager>().isDead) {
            playerAnim.SetBool("isDead" , true);
        }
    }
    
    public void HitAnim () {
        playerAnim.Play("Hit");
    }
    
    public void AttackAnim (string objectUse) {
        switch (objectUse) {
            case "Gun":
                playerAnim.Play("Shooting",-1);
            break;

            case "Knife":

            IEnumerator Knife () {
                playerAnim.applyRootMotion = true;
                playerAnim.Play("Cut",-1);
                yield return new WaitForSeconds(1.32f);
                playerAnim.applyRootMotion = false;
            }

            StartCoroutine(Knife());
            break;
        }

        ;
    }  

    /* Désactivé car effet pas jolie
     public void GrabAnim () {
        playerAnim.Play("Grab");
    } 
    */

}
