using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// for waiting room
public class WaitingPlayerCard : MonoBehaviour
{
    public TextMeshProUGUI cardPlayerName;
    private string defaultPlayerName;
        
    private void Start()
    {
        // save default text
        defaultPlayerName = cardPlayerName.text;
    }

    // set text of the card
    public void SetCardPlayerName(string playerName)
    {
        cardPlayerName.text = playerName;
    }
    // check if player name is same as default value
    public bool IsPlayerNameEmpty()
    {
        return defaultPlayerName == cardPlayerName.text;
    }
    
    public bool IsPlayerNameNull()
    {
        return cardPlayerName == null;
    }

    // set default text (when player leave waiting room)
    public void SetDefaultCardPlayerName()
    {
        if (defaultPlayerName != null)
            cardPlayerName.text = defaultPlayerName;
    }
}
