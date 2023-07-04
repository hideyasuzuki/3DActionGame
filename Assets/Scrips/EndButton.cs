using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndButton : MonoBehaviour, IPointerClickHandler
{
    Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// ÉQÅ[ÉÄÇèIóπÇ≥ÇπÇÈ
    /// </summary>
    /// <param name="pointerData"></param>
    public void OnPointerClick(PointerEventData pointerData)
    {
        image.color = Color.cyan;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
