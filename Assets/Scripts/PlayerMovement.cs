using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3.5f;
    public float rotateSpeed = 100.0f;
    public float jumpForce = 100.0f;
    private Rigidbody _rb;
    private Animator _anim;
    private bool _canJump = true;
    public bool isDead = false, gameOver = false;
    private Vector3 startPos;
    private bool respawned = false;
    private GameObject respawnPanel;
    public bool noRespawn;
    private bool startChecking = false;
    private GameObject Canvas;
    private GameObject cam;
    private PhotonView _photonView;
    
    //private float xRotation = 0f; // Current vertical rotation

    private void Awake(){
        respawnPanel = GameObject.Find("RespawnPanel");
        Canvas = GameObject.Find("Canvas");
    }

    void Start(){
        _photonView = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        startPos = transform.position;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDead) {
            MovementAndRotation();
        }
    }

    private void MovementAndRotation(){
        respawnPanel.SetActive(false);
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
            
        Vector3 rotateY = new Vector3(0, Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime, 0);
        float mouseY = Input.GetAxis("Mouse Y") * 3f;
           
        // xRotation -= mouseY;
        // xRotation = Mathf.Clamp(xRotation, -30f, 30f);
           
        _rb.MoveRotation(_rb.rotation * Quaternion.Euler(rotateY));
     
        _rb.MovePosition(_rb.position + (((transform.forward * movement.z) + (transform.right * movement.x)) * (moveSpeed * Time.deltaTime)));
            
        _anim.SetFloat("BlendVertical", movement.z);
        _anim.SetFloat("BlendHorizontal", movement.x);
    }

    private void Update()
    {
        if (noRespawn && PhotonNetwork.CurrentRoom.PlayerCount > 1 && !startChecking) {
            startChecking = true;
            InvokeRepeating("CheckForWinner", 3, 3);
        }
        if (!isDead) {
            if(Input.GetButtonDown("Jump") && _canJump) {
                _canJump = false;
                _rb.AddForce(Vector3.up * (jumpForce * Time.deltaTime), ForceMode.VelocityChange);
                StartCoroutine(JumpAgain());
            }
            return;
        }
        if (!respawned && !gameOver) {
                respawned = true;
            if (!noRespawn) {
                respawnPanel.SetActive(true);
                respawnPanel.GetComponent<RespawnTimer>().enabled = true;
                StartCoroutine(RespawnWait());
                return;
            }
            GetComponent<PlayerManager>().NoRespawnExit();
        }
    }

    void CheckForWinner(){
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
            Canvas.GetComponent<KillCount>().NoRespawnWinner(_photonView.Owner.NickName);
        }
    }

    IEnumerator JumpAgain()
    {
        yield return new WaitForSeconds(1.2f);
        _canJump = true;
    }

    IEnumerator RespawnWait(){
        yield return new WaitForSeconds(3f);
        isDead = false;
        respawned = false;
        transform.position = startPos;
        GetComponent<PlayerManager>().Respawn(_photonView.Owner.NickName);
    }
}