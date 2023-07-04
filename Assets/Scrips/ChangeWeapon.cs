using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeWeapon : MonoBehaviour
{
    [SerializeField] GameObject twinDaggerLeft;
    [SerializeField] GameObject twinDaggerRight;
    [SerializeField] GameObject greatSword;
    GameObject chest;
    bool isChange = false;
    public bool IsChange
    {
        get { return isChange; }
    }
    // Start is called before the first frame update
    void Start()
    {
        chest = GameObject.Find("Wooden_Chest");
    }

    // Update is called once per frame
    void Update()
    {
        if(isChange == false)
        {
            if (chest == null)
            {
                twinDaggerLeft.SetActive(true);
                twinDaggerRight.SetActive(true);
                greatSword.SetActive(false);
                isChange = true;
            }
        }     
    }
}
