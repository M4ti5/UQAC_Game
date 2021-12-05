using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShowWinners : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI textMessage;

    private EndGame _endGame;
    public Sprite enqueteursWin;
    public Sprite criminelWin;
    public Image bkgImage;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("EndGame") != null)
        {
            _endGame = GameObject.Find("EndGame").GetComponent<EndGame>();
            foreach (EndGame.PlayerInfoEndGame looser in _endGame.loosers)
            {
                if (looser.isMine)
                {
                    textMessage.text = "Dommage "+ GetNameAndRoleOfPayer(looser) + "!\n" + 
                                       "<color=\"red\">You loose !</color>";
                    DisplayWinners();
                    break;
                }
            }

            foreach (EndGame.PlayerInfoEndGame winner in _endGame.winners)
            {
                if (winner.isMine)
                {
                    textMessage.text = "Félicitation "+ GetNameAndRoleOfPayer(winner) + "!\n" + 
                                       "<color=\"green\">You win !</color>";

                    DisplayLoosers();
                    break;
                }
            }
            
            
        }
        else
        {
            textMessage.text = "<color=\"red\">Error !</color>";
        }
        
    }

    void DisplayWinners()
    {
        if (_endGame.winners.Count > 0)
        {
            if (_endGame.winners.Count == 1)
            {
                textMessage.text += "\nLe gagnant est : ";
            }
            else
            {
                textMessage.text += "\nLes gagnants sont : ";
            }
            
            // si le gagnant est criminel
            if (_endGame.winners[0].isCriminal)
            {
                bkgImage.sprite = criminelWin;
            }
            // si le/les gagnant(s) est/sont enqueteur(s)
            else
            {
                bkgImage.sprite = enqueteursWin;
            }

            foreach (EndGame.PlayerInfoEndGame winner in _endGame.winners)
            {
                textMessage.text += "\n" + GetNameAndRoleOfPayer(winner);
            }
        }
    }

    void DisplayLoosers()
    {
        if (_endGame.loosers.Count > 0)
        {
            if (_endGame.loosers.Count == 1)
            {
                textMessage.text += "\nLe perdant est : ";
            }
            else
            {
                textMessage.text += "\nLes perdants sont : ";
            }
            
            // si le gagnant est criminel
            if (_endGame.winners[0].isCriminal)
            {
                bkgImage.sprite = criminelWin;
            }
            // si le/les gagnant(s) est/sont enqueteur(s)
            else
            {
                bkgImage.sprite = enqueteursWin;
            }

            foreach (EndGame.PlayerInfoEndGame looser in _endGame.loosers)
            {
                textMessage.text += "\n" + GetNameAndRoleOfPayer(looser);
            }
        }
    }

    private string GetNameAndRoleOfPayer(EndGame.PlayerInfoEndGame playerStatManager)
    {
        return "<b>" + playerStatManager.name + "</b> (" + (playerStatManager.isCriminal ? "Criminel" : "Enquêteur") + ")";
    }
    
    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {   
            if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }


            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene("Launcher");
        }
    }
    
    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Launcher");
    }
}
