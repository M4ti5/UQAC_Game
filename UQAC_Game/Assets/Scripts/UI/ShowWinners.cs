using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// script is called when we are in the ending scene
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
        // find saved players status (winners and loosers)
        if (GameObject.Find("EndGame") != null)
        {
            _endGame = GameObject.Find("EndGame").GetComponent<EndGame>();
            // if we are in the looser list (+ show winners)
            foreach (EndGame.PlayerInfoEndGame looser in _endGame.loosers)
            {
                if (looser.isMine)
                {
                    textMessage.text = "Dommage "+ GetNameAndRoleOfPayer(looser) + "!\n" + 
                                       "<color=\"red\">Tu as perdu !</color>";
                    DisplayWinners();
                    break;
                }
            }
            // if we are in winner list (+ show loosers)
            foreach (EndGame.PlayerInfoEndGame winner in _endGame.winners)
            {
                if (winner.isMine)
                {
                    textMessage.text = "Félicitation "+ GetNameAndRoleOfPayer(winner) + "!\n" + 
                                       "<color=\"green\">Tu as gagné !</color>";

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
            // change text if we have one or multiple winners
            if (_endGame.winners.Count == 1)
            {
                textMessage.text += "\nLe gagnant est : ";
            }
            else
            {
                textMessage.text += "\nLes gagnants sont : ";
            }
            
            // change background image
            // if the winner is a criminal
            if (_endGame.winners[0].isCriminal)
            {
                bkgImage.sprite = criminelWin;
            }
            // if winners are enqueteurs
            else
            {
                bkgImage.sprite = enqueteursWin;
            }
            
            // display list of winners with there role
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
            // change text if we have one or multiple loosers
            if (_endGame.loosers.Count == 1)
            {
                textMessage.text += "\nLe perdant est : ";
            }
            else
            {
                textMessage.text += "\nLes perdants sont : ";
            }
            
            // change background image
            // if the winner is a criminal
            if (_endGame.winners[0].isCriminal)
            {
                bkgImage.sprite = criminelWin;
            }
            // if winners are enqueteurs
            else
            {
                bkgImage.sprite = enqueteursWin;
            }

            // display list of loosers with there role
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
        // leave room if we are in a room
        if (PhotonNetwork.InRoom)
        {   
            // if we are the last one in room, close it
            if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }


            PhotonNetwork.LeaveRoom();
        }
        else // if we start end scene and we are not in a room, load home
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
