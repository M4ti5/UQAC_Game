using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Animations : MonoBehaviourPun
{
    private Animator playerAnim;
    void Start () {
        
        playerAnim = GetComponent<Animator>();

        //Perpendicularized the player
        transform.rotation = Quaternion.Euler(new Vector3(0 , transform.rotation.eulerAngles.y , 0));
    }

    public void DeathAnim () {
        IEnumerator Death()
        {
            if (gameObject.GetComponent<PlayerStatManager>().isDead && playerAnim.GetBool("isDead") == false)
            {
                playerAnim.SetBool("isDead", true);
                yield return new WaitForSeconds(3f); // wait duration animation
                gameObject.GetComponent<PlayerStatManager>().DesequipmentTriggeredWhenPlayerLeaveGame();// unequipement
            }
        }
        StartCoroutine(Death());
    }
     
    public void HitAnim () {
        IEnumerator Hit () {
            playerAnim.SetBool("inHit",true);
            yield return new WaitForSeconds(1.25f);// wait duration animation
            playerAnim.SetBool("inHit" , false);
        }

        StartCoroutine(Hit());
    }
    
    public void AttackAnim (string objectUse) {
        switch (objectUse) {

            // distance animation
            case "Gun":
            case "Color Switch" :    
                IEnumerator Shoot () {
                playerAnim.SetBool("inShoot" , true);
                yield return new WaitForSeconds(1.10f);// wait duration animation
                playerAnim.SetBool("inShoot" , false);
            }

            StartCoroutine(Shoot());
            break;
            
            // body to body animation
            case "Knife":
            case "Pan":

            IEnumerator Knife () {
                playerAnim.applyRootMotion = true;
                playerAnim.SetBool("inCut" , true);
                yield return new WaitForSeconds(1.32f);// wait duration animation
                playerAnim.SetBool("inCut" , false);
                playerAnim.applyRootMotion = false;
            }
            StartCoroutine(Knife());
            break;
            

            // other animations
            case "Switcher": 
                
                Debug.Log("oui");
                IEnumerator Switch() {
                    playerAnim.SetBool("inSwitch" , true);
                    yield return new WaitForSeconds(0.5f);// wait duration animation
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
