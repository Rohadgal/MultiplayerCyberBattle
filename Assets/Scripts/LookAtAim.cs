using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LookAtAim : MonoBehaviour
{
    private Vector3 worldPosition;
    public Vector3 screenPosition;
    public GameObject crosshair;
    
    void FixedUpdate(){
        screenPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f - 30f, 6f);   //Input.mousePosition;
        screenPosition.z = 6f;
        
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        transform.position = worldPosition;

        crosshair.transform.position = screenPosition;
    }
}
