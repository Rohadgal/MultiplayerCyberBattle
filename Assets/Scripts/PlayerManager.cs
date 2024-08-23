using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Unity.VisualScripting;

public class PlayerManager : MonoBehaviourPunCallbacks{
	//public int[] buttonNumbers;
	public int[] viewID;
	public Color32[] colors;
	public Color32[] teamColors;
	public AudioClip[] gunshotSounds;
	
	private bool teamMode = false;
	private bool isRespawn = false;
	private PhotonView _photonView;
	private NicknamesScript _nicknamesScript;
	private PlayerMovement _playerMovement;
	
	private GameObject namesObject;
	private GameObject waitForPlayers;
	
	private void Start(){
		_photonView = GetComponent<PhotonView>();
		namesObject = GameObject.Find("NamesBackground");
		waitForPlayers = GameObject.Find("WaitingBackground");
		_nicknamesScript = namesObject.GetComponent<NicknamesScript>();
		_playerMovement = GetComponent<PlayerMovement>();
		InitializeSettings();
	}

	private void InitializeSettings(){
		InvokeRepeating("CheckTime", 1, 1);
		teamMode = _nicknamesScript.teamMode;
		isRespawn = _nicknamesScript.noRespawn;
		_playerMovement.noRespawn = isRespawn;
	}

	private void Update(){
		HandleEscapeKey();

		HandleHitState();
	}

	private void HandleHitState(){
		if (this.GetComponent<Animator>().GetBool("isHit")) {
			StartCoroutine(Recover());
		}
	}

	private void HandleEscapeKey(){
		if (Input.GetKeyDown(KeyCode.Escape) && _photonView.IsMine && !waitForPlayers.activeInHierarchy) {
			RemoveData();
			RoomExit();
		}
	}

	public void NoRespawnExit(){
		_nicknamesScript.eliminationPanel.SetActive(true);
		StartCoroutine(WaitToExit());
	}

	void CheckTime(){
		if (namesObject.GetComponent<Timer>().timeStop) {
			this.gameObject.GetComponent<PlayerMovement>().isDead = true;
			this.gameObject.GetComponent<PlayerMovement>().gameOver = true;
			this.gameObject.GetComponent<WeaponChange>().isDead = true;
			this.gameObject.GetComponentInChildren<AimLookAtRef>().isDead = true;
			this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		}
	}

	public void Respawn(string name){
		_photonView.RPC("ResetForReplay", RpcTarget.AllBuffered, name);
	}

	[PunRPC]
	void ResetForReplay(string name){
		for (int i = 0; i < namesObject.GetComponent<NicknamesScript>().names.Length; i++) {
			if (name == namesObject.GetComponent<NicknamesScript>().names[i].text) {
				this.GetComponent<Animator>().SetBool("isDead", false);
				this.gameObject.GetComponent<WeaponChange>().isDead = false;
				this.gameObject.GetComponentInChildren<AimLookAtRef>().isDead = false;
				this.gameObject.layer = LayerMask.NameToLayer("Default");
				namesObject.GetComponent<NicknamesScript>().healthbars[i].gameObject.GetComponent<Image>().fillAmount =
					1;
			}
		}
	}

	public void DeliverDamage(string shooterName, string name, float damageAmount){
		_photonView.RPC("GunDamage", RpcTarget.AllBuffered, shooterName, name, damageAmount);	
	}

	[PunRPC]
	void GunDamage(string shooterName, string name, float damageAmount){
		for (int i = 0; i < namesObject.GetComponent<NicknamesScript>().names.Length; i++) {
			if (name == namesObject.GetComponent<NicknamesScript>().names[i].text) {
				if (namesObject.GetComponent<NicknamesScript>().healthbars[i].gameObject.GetComponent<Image>()
					    .fillAmount > 0.1f) {
					this.GetComponent<Animator>().SetBool("isHit", true);
					namesObject.GetComponent<NicknamesScript>().healthbars[i].gameObject.GetComponent<Image>().fillAmount -= damageAmount;
					return;
				}
				namesObject.GetComponent<NicknamesScript>().healthbars[i].gameObject.GetComponent<Image>().fillAmount =
					0;
				this.GetComponent<Animator>().SetBool("isDead", true);
				this.gameObject.GetComponent<PlayerMovement>().isDead = true;
				this.gameObject.GetComponent<WeaponChange>().isDead = true;
				this.gameObject.GetComponentInChildren<AimLookAtRef>().isDead = true;
				namesObject.GetComponent<NicknamesScript>().RunMessage(shooterName, name);
				this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
			}
		}
	}
	
	private void RoomExit(){
		StartCoroutine(GetReadyToLeave());
	}

	private IEnumerator GetReadyToLeave(){
		yield return new WaitForSeconds(1);
		namesObject.GetComponent<NicknamesScript>().Leaving();
		Cursor.visible = true;
		PhotonNetwork.LeaveRoom();
	}

	private void RemoveData(){
		_photonView.RPC("RemoveMe", RpcTarget.AllBuffered);
	}

	public void ChooseColor(){
		_photonView.RPC("AssignColor", RpcTarget.AllBuffered);
	}

	public void PlayGunShot(string name, int weaponNumber) {
		_photonView.RPC("PlaySound", RpcTarget.All, name, weaponNumber);
	}

	[PunRPC]
	void PlaySound(string name, int weaponNumber){
		for (int i = 0; i < namesObject.GetComponent<NicknamesScript>().names.Length; i++) {
			if (name == namesObject.GetComponent<NicknamesScript>().names[i].text) {
				GetComponent<AudioSource>().clip = gunshotSounds[weaponNumber];
				GetComponent<AudioSource>().Play();
				return; // this was added
			}
		}
	}

	[PunRPC]
	void AssignColor(){
		for (int i = 0; i < viewID.Length; i++) {
			if (_photonView.ViewID == viewID[i]) {
				this.transform.GetChild(1).GetComponent<Renderer>().material.color = (!teamMode) ? colors[i] : teamColors[i];
				namesObject.GetComponent<NicknamesScript>().names[i].gameObject.SetActive(true);
				namesObject.GetComponent<NicknamesScript>().healthbars[i].gameObject.SetActive(true);
				namesObject.GetComponent<NicknamesScript>().names[i].text = _photonView.Owner.NickName;
			}	
		}
	}

	[PunRPC]
	void RemoveMe(){
		for (int i = 0; i < namesObject.gameObject.GetComponent<NicknamesScript>().names.Length; i++) {
			if (_photonView.Owner.NickName ==
			    namesObject.GetComponent<NicknamesScript>().names[i].text) {
				namesObject.GetComponent<NicknamesScript>().names[i].gameObject.SetActive(false);
				namesObject.GetComponent<NicknamesScript>().healthbars[i].gameObject.SetActive(false);
			}
		}
	}

	IEnumerator Recover(){
		yield return new WaitForSeconds(0.03f);
		this.GetComponent<Animator>().SetBool("isHit", false);
	}

	IEnumerator WaitToExit(){
		yield return new WaitForSeconds(3f);
		RemoveMe();
		RoomExit();
	}
}
