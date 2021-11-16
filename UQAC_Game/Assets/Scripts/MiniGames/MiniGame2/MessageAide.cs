using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageAide2 : MonoBehaviour
{
    public GameObject Texte;
    public GameObject Aide;

    public void Afficher()
    {
        Aide.gameObject.SetActive(false);
        Texte.gameObject.SetActive(true);
    }
    
    public void Cacher()
    {
        Aide.gameObject.SetActive(true);
        Texte.gameObject.SetActive(false);
    }
}