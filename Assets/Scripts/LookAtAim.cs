using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LookAtAim : MonoBehaviour
{
    private Vector3 _worldPosition;
    public Vector3 screenPosition;
    public GameObject crosshair;

    private void Start(){
	    // Draw crosshair on screen
	    screenPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f - 30f, 6f);   //Input.mousePosition;
        crosshair.transform.position = screenPosition;
    }

    void FixedUpdate(){
       
       // screenPosition.z = 6f;
       
        // get world position of crosshair
        _worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        //_worldPosition.y = Input.mousePosition.y;
        transform.position = _worldPosition;
		
        
    }
}
