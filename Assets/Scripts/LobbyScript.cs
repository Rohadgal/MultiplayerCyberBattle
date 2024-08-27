using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LobbyScript : MonoBehaviourPunCallbacks
{
    private TypedLobby _killCount = new TypedLobby("killCount", LobbyType.Default);
    private TypedLobby _teamBattle = new TypedLobby("teamBattle", LobbyType.Default);
    private TypedLobby _noRespawn = new TypedLobby("noRespawn", LobbyType.Default);

    public GameObject roomNumber;
    private string levelName = "";

    private void Start(){
        roomNumber.SetActive(false);
    }

    public void BackToMenu()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
    }

    public void JoinGameKillCount(){
        levelName = "KillCount";
        PhotonNetwork.JoinLobby(_killCount);
    }
    
    public void JoinTeamBattle()
    {
        levelName = "TeamBattle";
        PhotonNetwork.JoinLobby(_teamBattle);
    }
    
    public void JoinNoRespawn()
    {
        levelName = "NoRespawn";
        PhotonNetwork.JoinLobby(_noRespawn);
    }

    public override void OnJoinedLobby(){
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message){
        //Debug.Log("Joined random room failed, creating a new room");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 6;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        PhotonNetwork.CreateRoom("Arena" + Random.Range(1, 1000), roomOptions);
    }

    public override void OnJoinedRoom(){
        roomNumber.SetActive(true);
        PhotonNetwork.LoadLevel(levelName);
    }
}
