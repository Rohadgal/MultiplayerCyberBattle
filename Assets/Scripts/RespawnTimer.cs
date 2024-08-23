using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RespawnTimer : MonoBehaviour{
    public Text spawnTime;

    private void OnEnable(){
        StartCoroutine(SpawnStarting());
    }

    IEnumerator SpawnStarting(){
        spawnTime.text = "3";
        yield return new WaitForSeconds(1f);
        spawnTime.text = "2";
        yield return new WaitForSeconds(1f);
        spawnTime.text = "1";
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }
}
