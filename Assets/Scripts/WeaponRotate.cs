using UnityEngine;

public class WeaponRotate : MonoBehaviour
{
    public float speed = 20;
    
    void Update()
    {
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}
