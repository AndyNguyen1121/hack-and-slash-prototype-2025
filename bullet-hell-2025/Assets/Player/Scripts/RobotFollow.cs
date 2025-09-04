using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RobotFollow : MonoBehaviour
{
    private float initialY;
    private Vector3 velocity;
    public Transform podObject;
    public float smoothTime;

    private Tween movementTween;
    // Start is called before the first frame update
    void Start()
    {
        initialY = PlayerManager.instance.transform.InverseTransformPoint(transform.position).y;
        movementTween = transform.DOLocalMoveY(initialY + 0.5f, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        podObject.position = Vector3.SmoothDamp(podObject.position, transform.position, ref velocity, smoothTime * Time.deltaTime);
    }

    private void OnDestroy()
    {
        if (movementTween != null)
        {
            movementTween.Kill();
        }
    }
}
