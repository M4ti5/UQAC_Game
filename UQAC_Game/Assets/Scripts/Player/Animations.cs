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
        IEnumerator Hit () {
            playerAnim.SetBool("inHit",true);
            yield return new WaitForSeconds(1.25f);
            playerAnim.SetBool("inHit" , false);
        }

        StartCoroutine(Hit());
    }
    
    public void AttackAnim (string objectUse) {
        switch (objectUse) {
            case "Gun":
            IEnumerator Shoot () {
                playerAnim.SetBool("inShoot" , true);
                yield return new WaitForSeconds(1.10f);
                playerAnim.SetBool("inShoot" , false);
            }

            StartCoroutine(Shoot());
            break;

            case "Knife":

            IEnumerator Knife () {
                playerAnim.applyRootMotion = true;
                playerAnim.SetBool("inCut" , true);
                yield return new WaitForSeconds(1.32f);
                playerAnim.SetBool("inCut" , false);
                playerAnim.applyRootMotion = false;
            }
            StartCoroutine(Knife());
            break;
            
            case "Switcher": 
                
                Debug.Log("oui");
                IEnumerator Switch() {
                    playerAnim.SetBool("inSwitch" , true);
                    yield return new WaitForSeconds(0.5f);
                    playerAnim.SetBool("inSwitch" , false);
                }

                StartCoroutine(Switch());
                break;
        }

        ;
    }  

    /* D�sactiv� car effet pas jolie
     public void GrabAnim () {
        playerAnim.Play("Grab");
    } 
    */

}
