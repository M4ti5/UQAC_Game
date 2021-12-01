using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowWinners : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI textMessage;

    private EndGame _endGame;
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("EndGame") != null)
        {
            _endGame = GameObject.Find("EndGame").GetComponent<EndGame>();
            foreach (PlayerStatManager looser in _endGame.loosers)
            {
                if (looser.isMinePlayer)
                {
                    textMessage.text = "Dommage "+ GetNameAndRoleOfPayer(looser) + "!\n" + 
                                       "<color=\"red\">You loose !</color>";
                    DisplayWinners();
                    break;
                }
            }

            foreach (PlayerStatManager winner in _endGame.winners)
            {
                if (winner.isMinePlayer)
                {
                    textMessage.text = "Félicitation "+ GetNameAndRoleOfPayer(winner) + "!\n" + 
                                       "<color=\"green\">You win !</color>";
                    if (_endGame.winners.Count > 1)// si on est pas le criminel, on affiche aussi les autres joueurs
                    {
                        DisplayWinners();
                    }
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
                textMessage.text += "\nLe gagnant est : ";
            else
                textMessage.text += "\nLes gagnants sont : ";
            foreach (PlayerStatManager winner in _endGame.winners)
            {
                textMessage.text += "\n" + GetNameAndRoleOfPayer(winner);
            }
        }
    }

    private string GetNameAndRoleOfPayer(PlayerStatManager playerStatManager)
    {
        return "<b>" + playerStatManager.playerName + "</b> (" + (playerStatManager.criminal ? "Criminel" : "Enquêteur") + ")";
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
