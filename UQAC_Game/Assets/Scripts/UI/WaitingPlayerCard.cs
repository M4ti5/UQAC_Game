using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaitingPlayerCard : MonoBehaviour
{
    public TextMeshProUGUI cardPlayerName;
    private string defaultPlayerName;
        
    private void Start()
    {
        defaultPlayerName = cardPlayerName.text;
    }

    public void SetCardPlayerName(string playerName)
    {
        cardPlayerName.text = playerName;
    }
    public bool IsPlayerNameEmpty()
    {
        return defaultPlayerName == cardPlayerName.text;
    }

    public bool IsPlayerNameNull()
    {
        return cardPlayerName == null;
    }

    public void SetDefaultCardPlayerName()
    {
        if (defaultPlayerName != null)
            cardPlayerName.text = defaultPlayerName;
    }
}
