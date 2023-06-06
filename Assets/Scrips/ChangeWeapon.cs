using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeWeapon : MonoBehaviour
{
    [SerializeField] GameObject TwinDaggerLeft;
    [SerializeField] GameObject TwinDaggerRight;
    [SerializeField] GameObject GreatSword;
    GameObject Chest;
    bool isChange = false;
    public bool IsChange
    {
        get { return isChange; }
    }
    // Start is called before the first frame update
    void Start()
    {
        Chest = GameObject.Find("Wooden_Chest");
    }

    // Update is called once per frame
    void Update()
    {
        if(isChange == false)
        {
            if (Chest == null)
            {
                TwinDaggerLeft.SetActive(true);
                TwinDaggerRight.SetActive(true);
                GreatSword.SetActive(false);
                isChange = true;
            }
        }     
    }
}
