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
        for (int i = 3; i > 0; i--) {
            spawnTime.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        // spawnTime.text = "3";
        // yield return new WaitForSeconds(1f);
        // spawnTime.text = "2";
        // yield return new WaitForSeconds(1f);
        // spawnTime.text = "1";
        // yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
