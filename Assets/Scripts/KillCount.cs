using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class KillCount : MonoBehaviour{
    public List<Kills> highestKills = new List<Kills>();
    public Text[] names;
    public Text[] killAmounts;
    private GameObject killCountPanel;
    private GameObject namesObject;
   // private bool killCountOn = false;
    public bool countDown = true;
    public GameObject winnerPanel;
    public Text winnerText;
    private NicknamesScript _nicknamesScript;
    
    void Start()
    {
        killCountPanel = GameObject.Find("KillCountPanel");
        namesObject = GameObject.Find("NamesBackground");
        killCountPanel.SetActive(false);
        winnerPanel.SetActive(false);
        _nicknamesScript = namesObject.GetComponent<NicknamesScript>();
    }

    void Update()
    {
        if (!countDown) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.T)) {
            killCountPanel.SetActive(true);
            //killCountOn = true;
            highestKills.Clear();
            for (int i = 0; i < names.Length; i++) {
                highestKills.Add(new Kills(_nicknamesScript.names[i].text,_nicknamesScript.kills[i]));
               // Debug.Log(highestKills[i].playerName);
            }
            highestKills.Sort();
            for (int i = 0; i < names.Length; i++) {
                names[i].text = highestKills[i].playerName;
                killAmounts[i].text = highestKills[i].playerKills.ToString();
                if (names[i].text == "Name") {
                    names[i].text = "";
                    killAmounts[i].text = "";
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.T)) {
            killCountPanel.SetActive(false);
            //killCountOn = false;
        }
    }

    public void TimeOver(){
        killCountPanel.SetActive(true);
        winnerPanel.SetActive(true);
        //killCountOn = true;
        highestKills.Clear();
        for (int i = 0; i < names.Length; i++) {
            highestKills.Add(new Kills(namesObject.GetComponent<NicknamesScript>().names[i].text, namesObject.GetComponent<NicknamesScript>().kills[i]));
            // Debug.Log(highestKills[i].playerName);
        }
        highestKills.Sort();
        winnerText.text = highestKills[0].playerName;
        for (int i = 0; i < names.Length; i++) {
            names[i].text = highestKills[i].playerName;
            killAmounts[i].text = highestKills[i].playerKills.ToString();
            if (names[i].text == "Name") {
                names[i].text = "";
                killAmounts[i].text = "";
            }
        }
    }

    public void NoRespawnWinner(string name){
        winnerPanel.SetActive(true);
        winnerText.text = name;
    }
}
