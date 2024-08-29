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
	    // Place crosshair on screen
	    screenPosition = new Vector3(Screen.width * 0.5f, (Screen.height * 0.5f) - 50f, 2f);  
        crosshair.transform.position = new Vector3(screenPosition.x, screenPosition.y, 0);
    }

    void FixedUpdate(){
	    
        _worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        transform.position = _worldPosition;
		
        
    }
}
