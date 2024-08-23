using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Descriptions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public GameObject dropdown;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        dropdown.SetActive(true);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        dropdown.SetActive(false);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        dropdown.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
