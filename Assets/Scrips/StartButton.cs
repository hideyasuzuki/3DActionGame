using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour, IPointerClickHandler
{
    Image image;
    void Start()
    {
        image = gameObject.GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        image.color = Color.cyan;
        Cursor.visible = false;
        SceneManager.LoadScene("GameScene");
    }
}