using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TpsCamera : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] Transform player;          // 注視対象プレイヤー
    [SerializeField] float distance = 3.0f;    // 注視対象プレイヤーからカメラを離す距離
    Quaternion verticalRotation;      // カメラの垂直回転(見下ろし回転)
    Quaternion horizontalRotation;      // カメラの水平回転
    float mouseX;
    float mouseY;
    float turnSpeed = 1.0f;
    float minX = 10;
    float maxX = 80;
    float currentY;
    float currentX;

    public Quaternion HorizontalRotation
    {
        get { return horizontalRotation; }
    }
    
    void Start()
    {
        //回転の初期化
        verticalRotation = Quaternion.Euler(35, 0, 0);         // 垂直回転(X軸を軸とする回転)は、35度見下ろす回転
        horizontalRotation = Quaternion.identity;                // 水平回転(Y軸を軸とする回転)は、無回転
        transform.rotation = horizontalRotation * verticalRotation;     // 最終的なカメラの回転は、垂直回転してから水平回転する合成回転

        //位置の初期化
        //player位置から距離distanceだけ手前に引いた位置を設定します
        transform.position = player.position - transform.rotation * Vector3.forward * distance;
    }

    void Update()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        currentY = transform.localEulerAngles.y;
        if(currentY > 180)
        {
            currentY = currentY - 360;
        }
        if (Mathf.Abs(mouseX) > 0.1)
        {
            transform.RotateAround(target.transform.position, Vector3.up, mouseX);
            horizontalRotation *= Quaternion.Euler(0, mouseX * turnSpeed, 0);
        }

        if (Mathf.Abs(mouseY) > 0.1)
        {
            if(currentY <= -90 || currentY > 90)
            {
                transform.RotateAround(target.transform.position, Vector3.left, mouseY);
            }
            else if(currentY >= -90 || currentY < 90)
            {
                transform.RotateAround(target.transform.position, Vector3.right, mouseY);
            }        
        }

        currentX = transform.localEulerAngles.x;
        currentX = Mathf.Clamp(currentX, minX, maxX);
        verticalRotation = Quaternion.Euler(currentX * turnSpeed, 0, 0);
    }

    void LateUpdate()
    {
        // カメラの位置(transform.position)の更新
        transform.rotation = horizontalRotation * verticalRotation;
        // player位置から距離distanceだけ手前に引いた位置を設定します
        transform.position = player.position - transform.rotation * Vector3.forward * distance;
    }
}
