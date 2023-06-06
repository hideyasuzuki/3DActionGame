using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClear : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    float sceneTime = 2;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (text.enabled)
        {
            timer += Time.deltaTime;
            if (timer > sceneTime)
            {
                Cursor.visible = true;
                SceneManager.LoadScene("TitleScene");
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.name == "Set Costume_02 SD Unity-Chan WGS")
        {
            text.enabled = true;
        }
    }
}
