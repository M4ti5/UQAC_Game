using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour
{
    private Animator playerAnim;
    void Start () {
        playerAnim = GetComponent<Animator>();
    }

    
    public void DeathAnim () {
        if (gameObject.GetComponent<PlayerStatManager>().isDead) {
            playerAnim.SetBool("isDead" , true);
        }
}
}
