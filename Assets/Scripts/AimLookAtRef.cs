using UnityEngine;
using Photon.Pun;

public class AimLookAtRef : MonoBehaviour{
    private GameObject _LookAtObject;

    public bool isDead = false;

    private PhotonView _photonView;
  
    void Start(){
        _LookAtObject = GameObject.Find("AimRef");
        _photonView = gameObject.GetComponentInParent<PhotonView>();
    }
    
    void FixedUpdate()
    {
        if (_photonView.IsMine && !isDead) // this changed into _photonview var
        {
            transform.position = new Vector3(_LookAtObject.transform.position.x, _LookAtObject.transform.position.y - .5f, _LookAtObject.transform.position.z);
        }
    }
}
