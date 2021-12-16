using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveMiniGame : MonoBehaviour
{
    //Exit mini game
    public void ExitMiniGame()
    {
        //When the LeaveMiniGame buttun is clicked, destroy the instance of the mini game
        Destroy(gameObject.transform.parent.parent.parent.gameObject);
    }
}
