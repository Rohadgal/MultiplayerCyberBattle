using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AimLookAtRef : MonoBehaviour
{
    private GameObject LookAtObject;

    public bool isDead = false;
    
    private float xRotation = 0f;
  
    void Start()
    {
        LookAtObject = GameObject.Find("AimRef");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (this.gameObject.GetComponentInParent<PhotonView>().IsMine && !isDead)
        { 
            // float mouseY = Input.GetAxis("Mouse Y") * 300f;
            // xRotation -= mouseY;
            // xRotation = Mathf.Clamp(xRotation, -30f, 30f);
            this.transform.position = LookAtObject.transform.position; //+ new Vector3(0,xRotation,0);
        }
    }
}
