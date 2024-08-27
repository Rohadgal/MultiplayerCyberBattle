using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviourPunCallbacks{

	// public delegate void OnHit(float val, string name);
	//
	// public static OnHit onHit;
	
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

	//[SerializeField]
	//public BloodSplatter _bloodSplatter;
	[SerializeField]
	private GameObject _bloodGO;
	[SerializeField]
	private GameObject _redGO;
	private Image _bloodSplatter;
	private Image _redColor;

	private float val;
	
	
	
	private void Start(){
		namesObject = GameObject.Find("NamesBackground");
		waitForPlayers = GameObject.Find("WaitingBackground");
		_photonView = GetComponent<PhotonView>();
		_nicknamesScript = namesObject.GetComponent<NicknamesScript>();
		_playerMovement = GetComponent<PlayerMovement>();
		_timer = namesObject.GetComponent<Timer>();
		_animator = GetComponent<Animator>();
		
		_bloodSplatter = _bloodGO.gameObject.GetComponent<Image>();
		_redColor = _redGO.gameObject.GetComponent<Image>();
		_bloodGO.gameObject.SetActive(false);
		_redGO.gameObject.SetActive(false);
		
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
		gameObject.GetComponent<WeaponManager>().isDead = isDead;
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
		// for (int i = 0; i < _nicknamesScript.names.Length; i++) {
		// 	if (name == _nicknamesScript.names[i].text) {
		// 		//onHit?.Invoke(_nicknamesScript.healthbars[i].gameObject.GetComponent<Image>().fillAmount,name);
		// 		// _bloodSplatter.showDamage(_nicknamesScript.healthbars[i].gameObject.GetComponent<Image>().fillAmount, name);
		// 	}
		// }
		//showDamage(name);
	}

	[PunRPC]
	void GunDamage(string shooterName, string name, float damageAmount){
		for (int i = 0; i < _nicknamesScript.names.Length; i++) {
			if (name == _nicknamesScript.names[i].text) {
				if (_nicknamesScript.healthbars[i].gameObject.GetComponent<Image>().fillAmount > 0.1f) {
					_animator.SetBool("isHit", true);
					_nicknamesScript.healthbars[i].gameObject.GetComponent<Image>().fillAmount -= damageAmount;
					val = _nicknamesScript.healthbars[i].gameObject.GetComponent<Image>().fillAmount;
					//onHit?.Invoke(_nicknamesScript.healthbars[i].gameObject.GetComponent<Image>().fillAmount,name);
					showDamage( name);
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
				// Play gunshotsound
				AudioSource audioSource = GetComponent<AudioSource>();
				audioSource.clip = gunshotSounds[weaponNumber];
				audioSource.volume = .5f;
				audioSource.Play();
				return; // this was added
			}
		}
	}

	[PunRPC]
	void AssignColor(){
		for (int i = 0; i < viewID.Length; i++) {
			if (_photonView.ViewID == viewID[i]) {
				// Set player's color
				transform.GetChild(1).GetComponent<Renderer>().material.color = (!teamMode) ? colors[i] : teamColors[i];
				// Add name and healthbar to UI list
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
	
	public void showDamage(string name){
		if (name == GetComponent<PhotonView>().Owner.NickName && GetComponent<PhotonView>().IsMine) {
			if (_bloodSplatter is null || _redColor is null) {
				Debug.LogError("Blood splatter or red color references are missing.");
				return;
			}
			Debug.Log(name + " " + GetComponent<PhotonView>().Owner.NickName);
			Color tempColBlood = _bloodSplatter.color;
			Color tempColRed = _redColor.color;
			tempColBlood.a = 1 - val;
			tempColRed.a = 1 - val;
			_bloodSplatter.color = tempColBlood;
			_redColor.color = tempColRed;
			_bloodGO.gameObject.SetActive(true);
			_redGO.gameObject.SetActive(true);
			StartCoroutine(HideImage());
		}
	}
	IEnumerator HideImage(){
		yield return new WaitForSeconds(0.3f);
		_bloodGO.gameObject.SetActive(false);
		_redGO.gameObject.SetActive(false);
	}
}
