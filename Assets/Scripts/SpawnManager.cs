using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    public GameObject character;
    public Transform[] spawnPoints;
    public GameObject[] weapons;
    public Transform[] weaponSpawnPoints;
    
   // public float weaponRespawnTime = 10;
    
    // Start is called before the first frame update

    private void OnEnable(){
        WeaponManager.OnNoWeaponsFound += SpawnWeaponStart;
    }
    private void OnDisable(){
        WeaponManager.OnNoWeaponsFound -= SpawnWeaponStart;
    }

    void Start(){
       // _weaponChange = GameObject.Find("YBot(clone)").GetComponent<WeaponChange>();
        if (PhotonNetwork.IsConnected)
        {
            int spawnIndex = PhotonNetwork.LocalPlayer.ActorNumber % spawnPoints.Length;
            
            PhotonNetwork.Instantiate(character.name, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
        }
    }
    

    private void SpawnWeaponStart()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            PhotonNetwork.Instantiate(weapons[i].name, weaponSpawnPoints[i].position, weaponSpawnPoints[i].rotation);
        }
    }
}
