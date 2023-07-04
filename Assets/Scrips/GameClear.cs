using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClear : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    float sceneChangeTime = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (text.enabled)
        {
            StartCoroutine(ChangeScene());
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.name == "Set Costume_02 SD Unity-Chan WGS")
        {
            text.enabled = true;
        }
    }

    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(sceneChangeTime);
        Cursor.visible = true;
        SceneManager.LoadScene("TitleScene");
    }
}
