using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skybox : MonoBehaviour
{
    [SerializeField] Material sky = null;
    /// <summary>
    /// 回転し続ける値
    /// </summary>
    float rotationRepeatValue = 0.0f;
    /// <summary>
    /// 回転する速さ
    /// </summary>
    float rotateSpeed = 1.0f;
    /// <summary>
    /// 最大値
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
        //maxの値までをループで回転し続ける
        rotationRepeatValue = Mathf.Repeat(sky.GetFloat("_Rotation") + rotateSpeed * Time.deltaTime, max);
        sky.SetFloat("_Rotation", rotationRepeatValue);
    }
}
