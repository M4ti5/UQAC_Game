using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public float waitingTime = 5;

    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(DesactiveImageStartGame());
    }

    IEnumerator DesactiveImageStartGame()
    {
        yield return new WaitForSeconds(waitingTime);
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
