using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TpsCamera : MonoBehaviour
{
    /// <summary>
    /// 注視対象プレイヤー
    /// </summary>
    [SerializeField] GameObject target;
    /// <summary>
    /// 注視対象プレイヤーからカメラを離す距離
    /// </summary>
    [SerializeField] float distance = 3.0f;
    /// <summary>
    /// カメラの垂直回転(見下ろし回転)
    /// </summary>
    Quaternion verticalRotation;
    /// <summary>
    /// カメラの水平回転
    /// </summary>
    Quaternion horizontalRotation;
    /// <summary>
    /// レイヤー名
    /// </summary>
    [SerializeField] LayerMask obstacle;
    /// <summary>
    /// マウスのx軸
    /// </summary>
    float mouseX;
    /// <summary>
    /// マウスのy軸
    /// </summary>
    float mouseY;
    /// <summary>
    /// マウスの感度
    /// </summary>
    float turnSpeed = 1.0f;
    /// <summary>
    /// 最小値
    /// </summary>
    float minX = 10;
    /// <summary>
    /// 最大値
    /// </summary>
    float maxX = 80;
    /// <summary>
    /// 現在のy軸の角度
    /// </summary>
    float currentY;
    /// <summary>
    /// 現在のx軸の角度
    /// </summary>
    float currentX;
    /// <summary>
    /// 鋭角
    /// </summary>
    float acuteAngle = -90.0f;
    /// <summary>
    /// 鈍角
    /// </summary>
    float obtuseAngle = 90.0f;
    /// <summary>
    /// 半円の角度
    /// </summary>
    float semicircleAngle = 180.0f;
    /// <summary>
    /// 円の角度
    /// </summary>
    float circleAngle = 360.0f;

    public Quaternion HorizontalRotation
    {
        get { return horizontalRotation; }
    }
    
    void Start()
    {
        //回転の初期化
        // 垂直回転は、35度見下ろす回転
        verticalRotation = Quaternion.Euler(35, 0, 0);

        // 水平回転は、無回転
        horizontalRotation = Quaternion.identity;

        // 最終的なカメラの回転
        transform.rotation = horizontalRotation * verticalRotation;     

        //位置の初期化
        //player位置から距離distanceだけ手前に引いた位置
        transform.position = target.transform.position - transform.rotation * Vector3.forward * distance;
    }

    void Update()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        currentY = transform.localEulerAngles.y;
        
        if (currentY > semicircleAngle)
        {
            currentY = currentY - circleAngle;
        }

        if (Mathf.Abs(mouseX) > 0.1f)
        {
            horizontalRotation *= Quaternion.Euler(0, mouseX * turnSpeed, 0);
        }

        if (Mathf.Abs(mouseY) > 0.1f)
        {
            if(currentY <= acuteAngle || currentY > obtuseAngle)
            {
                transform.RotateAround(target.transform.position, Vector3.left, mouseY);
            }
            else if(currentY >= acuteAngle || currentY < obtuseAngle)
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
        transform.position = target.transform.position - transform.rotation * Vector3.forward * distance;

        RaycastHit hit;
        //レイが障害物を検知したらカメラの位置を障害物の手前にする
        if (Physics.Linecast(target.transform.position, transform.position, out hit, obstacle))
        {
            transform.position = hit.point;
            Debug.DrawLine(target.transform.position, transform.position, Color.red, 0f, false);
        }
    }

    public void Shake(float width, int count, float duration)
    {
       
        Sequence sequence = DOTween.Sequence();
        float partDuration = duration / count / 2f;
        float widthHalf = width / 2f;

        for(int i = 0; i < count - 1; i++)
        {
            sequence.Append(transform.DOLocalRotate(new Vector3(-widthHalf, 0f), partDuration));
            sequence.Append(transform.DOLocalRotate(new Vector3(widthHalf, 0f), partDuration));
        }

        sequence.Append(transform.DOLocalRotate(new Vector3(-widthHalf, 0f), partDuration));
        sequence.Append(transform.DOLocalRotate(Vector3.zero, partDuration));
    }
}