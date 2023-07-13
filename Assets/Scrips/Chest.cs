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
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        text.enabled = false;
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
            if(Input.GetKey(KeyCode.F)) 
            {
                if(animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
                {
                    text.enabled = false;
                    Destroy(gameObject);
                }
                else
                {
                    text.text = "ïêäÌÇïœçXÅiFÅj";
                    animator.SetBool("Push Key", true);
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
