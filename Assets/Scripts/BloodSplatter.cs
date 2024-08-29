using System;
using System.Collections;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;

public class BloodSplatter : MonoBehaviour{
	[SerializeField]
	private GameObject _bloodGO;
	[SerializeField]
	private GameObject _redGO;
	private Image _bloodSplatter;
	private Image _redColor;

	//private NicknamesScript _nicknamesScript;

	private void Start(){
		if (!_bloodGO || !_redGO) {
			return;
		}
		_bloodSplatter = _bloodGO.gameObject.GetComponent<Image>();
		_redColor = _redGO.gameObject.GetComponent<Image>();
		_bloodGO.gameObject.SetActive(false);
		_redGO.gameObject.SetActive(false);
	}

	public void showDamage(float val, string name){

		if (_bloodSplatter is null || _redColor is null) {
			Debug.LogError("Blood splatter or red color references are missing.");
			return;
		}

		
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

	IEnumerator HideImage(){
		yield return new WaitForSeconds(0.3f);
		_bloodGO.gameObject.SetActive(false);
		_redGO.gameObject.SetActive(false);
	}
}
