using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Timer : MonoBehaviour{
    public Text minutesText;
    public Text secondsText;
    public int minutes = 4;
    public int seconds = 20;
    public GameObject Canvas;
    [HideInInspector]
    public bool timeStop = false;

    public void BeginTimer(){
        GetComponent<PhotonView>().RPC("Count", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void Count(){
        BeginCounting();
    }

    void BeginCounting(){
        CancelInvoke();
        InvokeRepeating("TimeCountDown", 1, 1);
    }

    void TimeCountDown(){
        if (gameObject.GetComponent<NicknamesScript>().noRespawn) {
            minutesText.text = "";
            secondsText.text = "";
            return;
        }
        switch (seconds) {
            case > 10:
                seconds--;
                secondsText.text = seconds.ToString();
                break;
            case > 0 and < 11:
                seconds--;
                secondsText.text = "0" + seconds.ToString();
                break;
            case 0 when minutes > 0:
                secondsText.text = "0" + seconds.ToString();
                minutes--;
                seconds = 59;
                minutesText.text = minutes.ToString();
                secondsText.text = seconds.ToString();
                break;
        }

        if (seconds == 0 && minutes <= 0) {
            if (!this.gameObject.GetComponent<NicknamesScript>().teamMode) {
                Canvas.GetComponent<KillCount>().countDown = false;
                Canvas.GetComponent<KillCount>().TimeOver();
                timeStop = true;
                return;
            }
            Canvas.GetComponent<TeamKillCount>().countDown = false;
            Canvas.GetComponent<TeamKillCount>().TimeOver();
            timeStop = true;
        }
    }
}
