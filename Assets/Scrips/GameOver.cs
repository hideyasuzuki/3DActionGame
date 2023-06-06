using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] Player player;
    float sceneTime = 2;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {     
        if (!player.IsActive)
        {
            text.enabled = true;
            timer += Time.deltaTime;
            if(timer > sceneTime)
            {
                Cursor.visible = true;
                SceneManager.LoadScene("TitleScene"); 
            }            
        }
    }
}
