using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveMiniGame : MonoBehaviour
{
    //Exit mini game
    public void ExitMiniGame()
    {
        Destroy(gameObject.transform.parent.parent.parent.gameObject);
    }
}
