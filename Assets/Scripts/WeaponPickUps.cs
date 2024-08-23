
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class WeaponPickUps : MonoBehaviourPun
{
    private AudioSource audioPlayer;
    public float respawnTime = 5;
    private PhotonView _photonView;
    public int weaponType = 1;
    public int ammoRefillAmount = 60;

    void Start()
    {
        _photonView = this.GetComponent<PhotonView>();
        audioPlayer = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _photonView.RPC("PlayPickUpAudio", RpcTarget.All);
            _photonView.RPC("TurnOff", RpcTarget.All);
            other.GetComponent<WeaponChange>().ammoAmounts[weaponType - 1] += ammoRefillAmount;
            other.GetComponent<WeaponChange>().UpdatePickup();
        }
    }

    [PunRPC]
    void PlayPickUpAudio()
    {
        audioPlayer.Play();
    }

    [PunRPC]
    void TurnOff()
    {
        if (weaponType == 1)
        {
            this.transform.gameObject.GetComponent<Renderer>().enabled = false;
            
        }
        else
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }
        this.transform.gameObject.GetComponent<Collider>().enabled = false;
        StartCoroutine(WaitToRespawn());
    }
    
    [PunRPC]
    void TurnOn()
    {
        if (weaponType == 1)
        {
            this.transform.gameObject.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
        }
        this.transform.gameObject.GetComponent<Collider>().enabled = true;
    }

    IEnumerator WaitToRespawn()
    {
            yield return new WaitForSeconds(respawnTime);
            _photonView.RPC("TurnOn", RpcTarget.All);
    }
}
