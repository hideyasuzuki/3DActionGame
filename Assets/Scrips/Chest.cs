using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] GameObject TwinDaggerLeft;
    [SerializeField] GameObject TwinDaggerRight;
    [SerializeField] GameObject chest;
    Animator chestAnimator;

    // Start is called before the first frame update
    void Start()
    {
        text.enabled = false;
        chestAnimator = chest.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            text.enabled = true;
            if(Input.GetKeyDown(KeyCode.F)) 
            {
                if(chestAnimator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
                {
                    text.enabled = false;
                    Destroy(chest);
                }
                else
                {
                    text.text = "ïêäÌÇïœçXÅiFÅj";
                    chestAnimator.SetBool("IsKeyPush", true);
                    TwinDaggerLeft.SetActive(true);
                    TwinDaggerRight.SetActive(true);
                }
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            text.enabled = false;
        }
    }
}
