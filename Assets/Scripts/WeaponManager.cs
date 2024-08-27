using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
//using Cinemachine;
using Photon.Pun;
using UnityEngine.UI;
public class WeaponManager : MonoBehaviour{

    public delegate void NoWeaponFound();
    public static NoWeaponFound OnNoWeaponsFound;
    
    public TwoBoneIKConstraint leftHand;
    public TwoBoneIKConstraint rightHand;
    public TwoBoneIKConstraint leftThumb;

    public MultiAimConstraint[] aimObjects;
    
    public RigBuilder rig;
    
    public Transform[] leftTargets;
    public Transform[] rightTargets;
    public Transform[] thumbTargets;
    
    public GameObject[] weapons;
    public GameObject[] muzzleFlash;
    public Sprite[] weaponIcons;
    public int[] ammoAmounts;
    public float[] damageAmount;
    public bool isDead = false;
    
    // private GameObject camObject;
    // private CinemachineVirtualCamera cam;
    private GameObject aimTarget;
    private GameObject choosePanel;
    private GameObject testForWeapons;
    private Image weaponIcon;
    private Text ammoAmountText;
    private int _weaponIndex = 0;
    private PhotonView _photonView;
    
    void Start(){
        choosePanel = GameObject.Find("ChoosePanel");
        weaponIcon = GameObject.Find("WeaponUI").GetComponent<Image>();
        ammoAmountText = GameObject.Find("AmmoAmount").GetComponent<Text>();
       // camObject = GameObject.Find("PlayerCam");
        aimTarget = GameObject.Find("AimRef");
        _photonView = GetComponent<PhotonView>();
        ammoAmounts[0] = 60;
        ammoAmounts[1] = 0;
        ammoAmounts[2] = 0;
        ammoAmountText.text = ammoAmounts[0].ToString();
        
        // if (_photonView.IsMine) {
        //     cam = camObject.GetComponent<CinemachineVirtualCamera>();
        //     cam.Follow = gameObject.transform;
        //     cam.LookAt = gameObject.transform;
        //     //Invoke("SetLookAt", 0.1f);
        // }
        // else {
        //     gameObject.GetComponent<PlayerMovement>().enabled = false;
        // }
        
        testForWeapons = GameObject.Find("WeaponPickUp1(Clone)");
        
        if (!testForWeapons) {
            if (_photonView.Owner.IsMasterClient) {
                // var spawner = GameObject.Find("SpawnManager");
                // spawner.GetComponent<SpawnManager>().SpawnWeaponStart();
                OnNoWeaponsFound?.Invoke();
            }
        }
        
    }

    void Update()
    {
        if (isDead) {
            return;
        }
        if (Input.GetMouseButtonDown(0) && !choosePanel.activeInHierarchy) {
            GunshotAction();
        }
        if (Input.GetKeyDown(KeyCode.Q) && this.gameObject.GetComponent<PhotonView>().IsMine) {
            ChangeWeapon();
        }
    }

    private void GunshotAction(){
        if (GetComponent<PhotonView>().IsMine && ammoAmounts[_weaponIndex] > 0) {
            ammoAmounts[_weaponIndex]--;
            ammoAmountText.text = ammoAmounts[_weaponIndex].ToString();
            GetComponent<PlayerManager>().PlayGunShot(GetComponent<PhotonView>().Owner.NickName, _weaponIndex);
            GetComponent<PhotonView>().RPC("GunMuzzleFlash", RpcTarget.All);
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(aimTarget.GetComponent<LookAtAim>().screenPosition);//ttt
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            if (Physics.Raycast(ray, out hit, 500) && 
                hit.transform.gameObject.GetComponent<PhotonView>() &&
                hit.transform.gameObject.GetComponent<PlayerManager>()) {
                string gotShotName = hit.transform.gameObject.GetComponent<PhotonView>().Owner.NickName;
                string shooterName = GetComponent<PhotonView>().Owner.NickName;
                hit.transform.gameObject.GetComponent<PlayerManager>().DeliverDamage(shooterName, gotShotName, damageAmount[_weaponIndex]);
            }
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    public void UpdatePickup(){
        ammoAmountText.text = ammoAmounts[_weaponIndex].ToString();
    }

    [PunRPC]
    void GunMuzzleFlash(){
        muzzleFlash[_weaponIndex].SetActive(true);
        StartCoroutine(MuzzleOff());
    }
    IEnumerator MuzzleOff(){
        yield return new WaitForSeconds(0.03f);
        this.GetComponent<PhotonView>().RPC("MuzzleFlashOff", RpcTarget.All);
        
    }

    [PunRPC]
    void MuzzleFlashOff(){
        muzzleFlash[_weaponIndex].SetActive(false);
    }

    private void ChangeWeapon()
    {
        this.GetComponent<PhotonView>().RPC("Change", RpcTarget.AllBuffered);
        //_weaponIndex++;
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }

        if (_weaponIndex >= weapons.Length) {
            weaponIcon.GetComponent<Image>().sprite = weaponIcons[0];
            ammoAmountText.text = ammoAmounts[0].ToString();
            _weaponIndex = 0;
        }
        
        weapons[_weaponIndex].SetActive(true);
        weaponIcon.GetComponent<Image>().sprite = weaponIcons[_weaponIndex];
        ammoAmountText.text = ammoAmounts[_weaponIndex].ToString();
        leftHand.data.target = leftTargets[_weaponIndex];
        rightHand.data.target = rightTargets[_weaponIndex];
        leftThumb.data.target = thumbTargets[_weaponIndex];
        rig.Build();
        
    }

    [PunRPC]
    public void Change()
    {
        _weaponIndex++;
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }

        if (_weaponIndex >= weapons.Length)
        {
            _weaponIndex = 0;
        }
        
        weapons[_weaponIndex].SetActive(true);
        
        leftHand.data.target = leftTargets[_weaponIndex];
        rightHand.data.target = rightTargets[_weaponIndex];
        leftThumb.data.target = thumbTargets[_weaponIndex];
        rig.Build();
    }
}
