using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skybox : MonoBehaviour
{
    [SerializeField] Material sky = null;
    /// <summary>
    /// ��]��������l
    /// </summary>
    float rotationRepeatValue = 0.0f;
    /// <summary>
    /// ��]���鑬��
    /// </summary>
    float rotateSpeed = 1.0f;
    /// <summary>
    /// �ő�l
    /// </summary>
    float max = 360;
    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.skybox = sky;
    }

    // Update is called once per frame
    void Update()
    {
        //max�̒l�܂ł����[�v�ŉ�]��������
        rotationRepeatValue = Mathf.Repeat(sky.GetFloat("_Rotation") + rotateSpeed * Time.deltaTime, max);
        sky.SetFloat("_Rotation", rotationRepeatValue);
    }
}
