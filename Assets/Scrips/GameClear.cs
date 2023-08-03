using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClear : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    float sceneChangeTime = 2;
    bool isClear = false;

    public bool IsClear
    { 
        get { return isClear; }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (text.enabled)
        {
            isClear = true;
            StartCoroutine(ChangeScene());
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            text.enabled = true;
        }
    }

    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(sceneChangeTime);
        Cursor.visible = true;
        SceneManager.LoadScene("DesertScene");
    }
}
