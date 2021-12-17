using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageAideMain : MonoBehaviour
{
    public GameObject panelAide; 
    
    public TextMeshProUGUI texteGameObject;
    
    public GameObject aideBtn;  
    

    [TextArea(3,50)]
    public string texte;

    private void Start()
    {
        texteGameObject.text = texte;
        //always hide in start
        Cacher();
    }

    public void Afficher()
    {
        aideBtn.gameObject.SetActive(false);
        panelAide.gameObject.SetActive(true);
    }

    public void Cacher()
    {
        aideBtn.gameObject.SetActive(true);
        panelAide.gameObject.SetActive(false);
    }

    public void Toggle()
    {
        panelAide.gameObject.SetActive(!panelAide.gameObject.activeSelf);
    }
}
