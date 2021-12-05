using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
        StartCoroutine(DesactiveImageStartGame());
    }

    IEnumerator DesactiveImageStartGame()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
}
