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
	private Timer _timer;
	private Animator _animator;
	
	private GameObject namesObject;
	private GameObject waitForPlayers;
	
	private void Start(){
		_photonView = GetComponent<PhotonView>();
		namesObject = GameObject.Find("NamesBackground");
		waitForPlayers = GameObject.Find("WaitingBackground");
		_nicknamesScript = namesObject.GetComponent<NicknamesScript>();
		_playerMovement = GetComponent<PlayerMovement>();
		_timer = namesObject.GetComponent<Timer>();
		_animator = GetComponent<Animator>();
		
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
		if (_animator.GetBool("isHit")) {
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
		if (_timer.timeStop) {
			_playerMovement.gameOver = true;
			_playerMovement.isDead = true;
			
			SetPlayerDeadState(true);
		}
	}

	private void SetPlayerDeadState(bool isDead){
		gameObject.GetComponent<WeaponChange>().isDead = isDead;
		gameObject.GetComponentInChildren<AimLookAtRef>().isDead = isDead;
		gameObject.layer = (isDead)?LayerMask.NameToLayer("Ignore Raycast") : LayerMask.NameToLayer("Default");
	}

	public void Respawn(string name){
		_photonView.RPC("ResetForReplay", RpcTarget.AllBuffered, name);
	}

	[PunRPC]
	void ResetForReplay(string name){
		for (int i = 0; i < _nicknamesScript.names.Length; i++) {
			if (name == _nicknamesScript.names[i].text) {
				_animator.SetBool("isDead", false);
				
				SetPlayerDeadState(false);
				
				_nicknamesScript.healthbars[i].gameObject.GetComponent<Image>().fillAmount = 1;
			}
		}
	}

	public void DeliverDamage(string shooterName, string name, float damageAmount){
		_photonView.RPC("GunDamage", RpcTarget.AllBuffered, shooterName, name, damageAmount);	
	}

	[PunRPC]
	void GunDamage(string shooterName, string name, float damageAmount){
		for (int i = 0; i < _nicknamesScript.names.Length; i++) {
			if (name == _nicknamesScript.names[i].text) {
				if (_nicknamesScript.healthbars[i].gameObject.GetComponent<Image>()
					    .fillAmount > 0.1f) {
					_animator.SetBool("isHit", true);
					_nicknamesScript.healthbars[i].gameObject.GetComponent<Image>().fillAmount -= damageAmount;
					return;
				}
				_nicknamesScript.healthbars[i].gameObject.GetComponent<Image>().fillAmount = 0;
				_animator.SetBool("isDead", true);
				
				_playerMovement.isDead = true;
				_nicknamesScript.RunMessage(shooterName, name);
				
				SetPlayerDeadState(true);
			}
		}
	}
	
	private void RoomExit(){
		StartCoroutine(GetReadyToLeave());
	}

	private IEnumerator GetReadyToLeave(){
		yield return new WaitForSeconds(1);
		_nicknamesScript.Leaving();
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
		for (int i = 0; i < _nicknamesScript.names.Length; i++) {
			if (name == _nicknamesScript.names[i].text) {
				AudioSource audioSource = GetComponent<AudioSource>();
				audioSource.clip = gunshotSounds[weaponNumber];
				audioSource.Play();
				return; // this was added
			}
		}
	}

	[PunRPC]
	void AssignColor(){
		for (int i = 0; i < viewID.Length; i++) {
			if (_photonView.ViewID == viewID[i]) {
				transform.GetChild(1).GetComponent<Renderer>().material.color = (!teamMode) ? colors[i] : teamColors[i];
				_nicknamesScript.names[i].text = _photonView.Owner.NickName;
				_nicknamesScript.healthbars[i].gameObject.SetActive(true);
				_nicknamesScript.names[i].gameObject.SetActive(true);
			}	
		}
	}

	[PunRPC]
	void RemoveMe(){
		for (int i = 0; i < _nicknamesScript.names.Length; i++) {
			if (_photonView.Owner.NickName == _nicknamesScript.names[i].text) {
				_nicknamesScript.healthbars[i].gameObject.SetActive(false);
				_nicknamesScript.names[i].gameObject.SetActive(false);
			}
		}
	}

	IEnumerator Recover(){
		yield return new WaitForSeconds(0.03f);
		_animator.SetBool("isHit", false);
	}

	IEnumerator WaitToExit(){
		yield return new WaitForSeconds(3f);
		RemoveMe();
		RoomExit();
	}
}
