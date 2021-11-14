using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGamesManager : MonoBehaviour
{
    public List<GameObject> allMiniGames = new List<GameObject>();
    public List<bool> allMiniGamesEnabled = new List<bool>();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start MiniGamesManager script");
        foreach (GameObject miniGame in allMiniGames)
        {
            allMiniGamesEnabled.Add(false);// create list to active or note a mini game
        }
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        foreach (bool enabled in allMiniGamesEnabled)
        {
            //allMiniGames[i].SetActive(enabled);
            if (enabled)
            {
                //var createdMiniGame = Instantiate(allMiniGames[i], allMiniGames[i].transform.position, allMiniGames[i].transform.rotation);
                //createdMiniGame.SetActive(true);
            }
            i++;
        }
    }
}
