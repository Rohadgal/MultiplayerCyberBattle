using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NicknamesScript : MonoBehaviourPunCallbacks{
	public Text[] names;
	public Image[] healthbars;
	private GameObject waitCanvasObject;
	public GameObject displayPanel;
	public Text messageText;
	public int[] kills;
	public bool teamMode = false;
	public bool noRespawn = false;
	public GameObject eliminationPanel;

	private void Start(){
		if (noRespawn) {
			eliminationPanel.SetActive(false);
		}
		displayPanel.SetActive(false);
		for (int  i = 0;  i < names.Length;  i++) {
			names[i].gameObject.SetActive(false);
			healthbars[i].gameObject.SetActive(false); 
		}
		waitCanvasObject = GameObject.Find("WaitingBackground");
	}

	public void Leaving(){
		StartCoroutine("BackToLobby");
	}

	private IEnumerator BackToLobby(){
		yield return new WaitForSeconds(0.5f);
		PhotonNetwork.LoadLevel("Lobby");
	}
	
	//This is for the waiting screen
	public void ReturnToLobby(){
		waitCanvasObject.SetActive(false);
		RoomExit();
	}

	private void RoomExit(){
		StartCoroutine(ToLobby());
	}

	public void RunMessage(string win, string lose){
		this.GetComponent<PhotonView>().RPC("DisplayMessage", RpcTarget.All, win, lose);
		UpdateKills(win);
	}

	void UpdateKills(string win){
		for (int i = 0; i < names.Length; i++) {
			if (win == names[i].text) {
				kills[i]++;
			}
		}
	}
	
	[PunRPC]
	void DisplayMessage(string win, string lose){
		displayPanel.SetActive(true);
		messageText.text = win + " killed " + lose;
		StartCoroutine(SwitchOffMessage());
	}

	IEnumerator SwitchOffMessage(){
		yield return new WaitForSeconds(3f);
		this.GetComponent<PhotonView>().RPC("MessageOff", RpcTarget.All);
	}

	[PunRPC]
	void MessageOff(){
		displayPanel.SetActive(false);
	}

	IEnumerator ToLobby(){
		yield return new WaitForSeconds(0.4f);
		Cursor.visible = true;
		PhotonNetwork.LeaveRoom();
	}

	public override void OnLeftRoom(){
		PhotonNetwork.LoadLevel("Lobby");
	}
}
