using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TeamKillCount : MonoBehaviour
{
    private GameObject killCountPanel;
    private GameObject namesObject;
    private NicknamesScript _nicknamesScript;
    //private bool killCountOn = false;
    private int numOfPlayers = 6;
    private int blueTeamKills, orangeTeamKills;
    
    public List<Kills> highestKills = new List<Kills>();
    public Text[] killAmounts;
    public Text winnerText;
    public GameObject winnerPanel;
    public bool countDown = true;
    
    void Start()
    {
        killCountPanel = GameObject.Find("KillCountPanel");
        namesObject = GameObject.Find("NamesBackground");
        _nicknamesScript = namesObject.GetComponent<NicknamesScript>();
        killCountPanel.SetActive(false);
        winnerPanel.SetActive(false);
    }

    void Update()
    {
        if (!countDown) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.T)) {
            killCountPanel.SetActive(true);
            highestKills.Clear();
            for (int i = 0; i < numOfPlayers; i++) {
                highestKills.Add(new Kills(_nicknamesScript.names[i].text, _nicknamesScript.kills[i]));
            }
            blueTeamKills = highestKills[0].playerKills + highestKills[1].playerKills + highestKills[2].playerKills;
            orangeTeamKills = highestKills[3].playerKills + highestKills[4].playerKills + highestKills[5].playerKills;
            killAmounts[0].text = blueTeamKills.ToString();
            killAmounts[1].text = orangeTeamKills.ToString();
        }
        
        if(Input.GetKeyUp(KeyCode.T)) {
            killCountPanel.SetActive(false);
        }
    }

    public void TimeOver(){
        killCountPanel.SetActive(true);
        winnerPanel.SetActive(true);
        //killCountOn = true;
        highestKills.Clear();
        for (int i = 0; i < numOfPlayers; i++) {
            highestKills.Add(new Kills(_nicknamesScript.names[i].text, _nicknamesScript.kills[i]));
        }
        blueTeamKills = highestKills[0].playerKills + highestKills[1].playerKills + highestKills[2].playerKills;
        orangeTeamKills = highestKills[3].playerKills + highestKills[4].playerKills + highestKills[5].playerKills;
        killAmounts[0].text = blueTeamKills.ToString();
        killAmounts[1].text = orangeTeamKills.ToString();
        winnerText.text = (blueTeamKills > orangeTeamKills) ? "BLUE TEAM WINS" : "ORANGE TEAM WINS";
    }   
}
