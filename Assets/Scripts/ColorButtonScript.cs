using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class ColorButtonScript : MonoBehaviour
{
    private GameObject[] players;
    private int myID;
    private GameObject panel;
    private GameObject namesBGObject;

    private void Start(){
        Cursor.visible = true;
        panel = GameObject.Find("ChoosePanel");
        namesBGObject = GameObject.Find("NamesBackground");
    }

    public void SelectButton(int buttonNumber){
        players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++) {
            if (players[i].GetComponent<PhotonView>().IsMine) {
                myID = players[i].GetComponent<PhotonView>().ViewID;
                break;
            }
        }
        GetComponent<PhotonView>().RPC("SelectedColor", RpcTarget.AllBuffered, buttonNumber, myID);
        Cursor.visible = false;
        panel.SetActive(false);
    }

    [PunRPC]
    void SelectedColor(int buttonNumber, int myID){
        players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++) {
            // Assign ID and color to player
            players[i].GetComponent<PlayerManager>().viewID[buttonNumber] = myID;
            players[i].GetComponent<PlayerManager>().ChooseColor();
        }
        namesBGObject.GetComponent<Timer>().BeginTimer();
        gameObject.SetActive(false);
    }
}
