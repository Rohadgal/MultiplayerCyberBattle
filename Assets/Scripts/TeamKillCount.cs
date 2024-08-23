using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TeamKillCount : MonoBehaviour
{
    public List<Kills> highestKills = new List<Kills>();
    public Text[] killAmounts;
    private GameObject killCountPanel;
    private GameObject namesObject;
    private bool killCountOn = false;
    public bool countDown = true;
    public GameObject winnerPanel;
    public Text winnerText;
    private int numOfPlayers = 6;
    private int blueTeamKills, orangeTeamKills;
    
    void Start()
    {
        killCountPanel = GameObject.Find("KillCountPanel");
        namesObject = GameObject.Find("NamesBackground");
        killCountPanel.SetActive(false);
        winnerPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && countDown) {
            
            if (!killCountOn) {
                killCountPanel.SetActive(true);
                killCountOn = true;
                highestKills.Clear();
                for (int i = 0; i < numOfPlayers; i++) {
                    highestKills.Add(new Kills(namesObject.GetComponent<NicknamesScript>().names[i].text, namesObject.GetComponent<NicknamesScript>().kills[i]));
                    // Debug.Log(highestKills[i].playerName);
                }
                blueTeamKills = highestKills[0].playerKills + highestKills[1].playerKills + highestKills[2].playerKills;
                orangeTeamKills = highestKills[3].playerKills + highestKills[4].playerKills + highestKills[5].playerKills;
                killAmounts[0].text = blueTeamKills.ToString();
                killAmounts[1].text = orangeTeamKills.ToString();
                return;
            }
            killCountPanel.SetActive(false);
            killCountOn = false;
        }
    }

    public void TimeOver(){
        killCountPanel.SetActive(true);
        winnerPanel.SetActive(true);
        killCountOn = true;
        highestKills.Clear();
        for (int i = 0; i < numOfPlayers; i++) {
            highestKills.Add(new Kills(namesObject.GetComponent<NicknamesScript>().names[i].text,
                namesObject.GetComponent<NicknamesScript>().kills[i]));
        }
        blueTeamKills = highestKills[0].playerKills + highestKills[1].playerKills + highestKills[2].playerKills;
        orangeTeamKills = highestKills[3].playerKills + highestKills[4].playerKills + highestKills[5].playerKills;
        killAmounts[0].text = blueTeamKills.ToString();
        killAmounts[1].text = orangeTeamKills.ToString();
        winnerText.text = (blueTeamKills > orangeTeamKills) ? "BLUE TEAM WINS" : "ORANGE TEAM WINS";
    }   
}
