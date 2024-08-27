using System.Collections;
using UnityEngine;
using Photon.Pun;

public class WeaponPickUps : MonoBehaviourPun
{
    private AudioSource _audioPlayer;
    private PhotonView _photonView;
    public float respawnTime = 5;
    public int weaponType = 1;
    public int ammoRefillAmount = 60;

    void Start()
    {
        _photonView = this.GetComponent<PhotonView>();
        _audioPlayer = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other){
        if (other.CompareTag("Player")) {
            _photonView.RPC("PlayPickUpAudio", RpcTarget.All);
            _photonView.RPC("TurnOffOn", RpcTarget.All, false);
            other.GetComponent<WeaponManager>().ammoAmounts[weaponType - 1] += ammoRefillAmount;
            other.GetComponent<WeaponManager>().UpdatePickup();
        }
    }

    [PunRPC]
    void PlayPickUpAudio(){
        _audioPlayer.Play();
    }
    
    [PunRPC]
    void TurnOffOn(bool isOff){
        if (weaponType == 1) {
            transform.gameObject.GetComponent<Renderer>().enabled = isOff;
        }
        else {
            transform.GetChild(0).gameObject.SetActive(isOff);
        }
        transform.gameObject.GetComponent<Collider>().enabled = isOff;
        if (!isOff) {
            StartCoroutine(WaitToRespawn());
        }
    }
    //
    // [PunRPC]
    // void TurnOff(){
    //     if (weaponType == 1) {
    //         transform.gameObject.GetComponent<Renderer>().enabled = false;
    //     }
    //     else {
    //         transform.GetChild(0).gameObject.SetActive(false);
    //     }
    //     transform.gameObject.GetComponent<Collider>().enabled = false;
    //     StartCoroutine(WaitToRespawn());
    // }
    //
    // [PunRPC]
    // void TurnOn(){
    //     if (weaponType == 1) {
    //         transform.gameObject.GetComponent<Renderer>().enabled = true;
    //     }
    //     else {
    //         transform.GetChild(0).gameObject.SetActive(true);
    //     }
    //     transform.gameObject.GetComponent<Collider>().enabled = true;
    // }

    IEnumerator WaitToRespawn(){
            yield return new WaitForSeconds(respawnTime);
            _photonView.RPC("TurnOffOn", RpcTarget.All, true);
    }
}
