using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerJoinedCheck : MonoBehaviour{
    
    public int maxPlayerInRoom = 2;
    public Text currentPlayers;
    public GameObject hint1, hint2, enterButton;
    void Update()
    {
        if (!enterButton.activeInHierarchy) {
            currentPlayers.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + " / " + maxPlayerInRoom.ToString();
        }
        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayerInRoom) {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            hint1.SetActive(false);
            hint2.SetActive(false);
            currentPlayers.text = "";
            enterButton.SetActive(true);
        }
    }

    public void EnterTheArena(){
        this.gameObject.SetActive(false);
    }
}
