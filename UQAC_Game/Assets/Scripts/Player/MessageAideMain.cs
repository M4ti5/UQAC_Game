using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageAideMain : MonoBehaviour
{
    // gameobject global avec un background + text + btn fermeture
    public GameObject panelAide; 
    
    // Component de text pour modifier le texte au démarrage
    public TextMeshProUGUI texteGameObject;
    
    // btn principal pour afficher l'aide
    public GameObject aideBtn;
    
    // zone de texte qui sera copié dans le component text
    [TextArea(3,50)]
    public string texte;

    private void Start()
    {
        // on affecte le texte au component
        texteGameObject.text = texte;
        // on s'assure que l'aide est tjrs cachée au démarrage
        Cacher();
    }

    public void Afficher()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            aideBtn.gameObject.SetActive(false);
            panelAide.gameObject.SetActive(true);
        }
    }

    public void Cacher()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            aideBtn.gameObject.SetActive(true);
            panelAide.gameObject.SetActive(false);
        }
    }

    public void Toggle()
    {
        if(Input.GetKey(KeyCode.LeftAlt))
            panelAide.gameObject.SetActive(!panelAide.gameObject.activeSelf);
    }
}
