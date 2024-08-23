using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviourPunCallbacks
{
    public InputField playerNickname;
    private string setName = "";
    public GameObject connectingTextGO;
    private void Start()
    {
        connectingTextGO.SetActive(false);
    }

    public void UpdateText()
    {
        setName = playerNickname.text;
        PhotonNetwork.LocalPlayer.NickName = setName;
    }
    public void EnterButton()
    {
        //UpdateText();
        
        if (setName != "")
        {
            //PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
            connectingTextGO.SetActive(true);
        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public override void OnConnectedToMaster()
    {
        //Debug.Log("I'm connected to the server!");
        SceneManager.LoadScene("Lobby");
    }

    
}