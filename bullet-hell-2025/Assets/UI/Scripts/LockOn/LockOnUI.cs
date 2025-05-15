using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LockOnUI : MonoBehaviour
{
    public static LockOnUI instance;
    public Transform target;
    public Camera mainCam;
    public Image lockOnUI;
    private float initialDistance;
    private bool isLocked = false;
    private RectTransform rectTransform;

    private float defaultReticleWidth;
    private float defaultReticleHeight;
    private Sequence sequence;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        rectTransform = GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            defaultReticleHeight = rectTransform.sizeDelta.y;
            defaultReticleWidth = rectTransform.sizeDelta.x;
        }

        lockOnUI = GetComponent<Image>();
    }

    private void Start()
    {
        mainCam = Camera.main;
        initialDistance = Vector3.Distance(lockOnUI.transform.position, mainCam.transform.position);
    }

    private void Update()
    {
        TrackTarget();
        MaintainWorldSpaceScale();
    }

    private void TrackTarget()
    {
        if (target != null)
        {
            lockOnUI.transform.LookAt(mainCam.transform);
            transform.position = target.position;

            Quaternion lookRotation = Quaternion.LookRotation(transform.forward, Vector3.up);

            // Manually rotate around Z axis
            float zRotation = 0;
            zRotation += 360f / 3f * Time.deltaTime; // 360 degrees every 3 seconds
            Quaternion zSpin = Quaternion.AngleAxis(zRotation, Vector3.forward);

            transform.rotation = lookRotation * zSpin;

            if (!isLocked)
            {
                FadeInLockOnReticle();
                isLocked = true;
            }
        }
        else if (isLocked)
        {
            isLocked = false;
            FadeOutLockOnReticle();
        }
    }

    private void MaintainWorldSpaceScale()
    {
        float dist = Vector3.Distance(lockOnUI.transform.position, mainCam.transform.position);
        lockOnUI.transform.localScale = Vector3.one * dist / initialDistance;
    }

    public void ChangeTarget(Transform target)
    {
        // can be set to null to deactivate
        this.target = target;
    }

    private void FadeInLockOnReticle()
    {
        lockOnUI.enabled = true;
        rectTransform.sizeDelta = new Vector2(150, 150);

        Color desiredColor = lockOnUI.color;
        desiredColor.a = 1;

        Vector3 desiredRotation = new Vector3(0, 0, 1800);
        Vector3 rotationOverTime = new Vector3(0, 0, 360);

        if (sequence != null) 
            sequence.Kill();

        sequence = DOTween.Sequence()
            .Append(DOTween.To(() => rectTransform.sizeDelta, x => rectTransform.sizeDelta = x, new Vector2(defaultReticleWidth, defaultReticleHeight), 0.2f))
            .Join(rectTransform.DORotate(desiredRotation, 0.3f, RotateMode.LocalAxisAdd))
            .Join(lockOnUI.DOColor(desiredColor, 0.2f));

        float startZ = rectTransform.eulerAngles.z;
        float targetZ = startZ + 360f;

        DOTween.To(() => startZ, z =>
        {
            Vector3 currentEuler = rectTransform.eulerAngles;
            rectTransform.rotation = Quaternion.Euler(currentEuler.x, currentEuler.y, z);

        }, targetZ, 3f)
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Restart);
    }
    private void FadeOutLockOnReticle()
    {
        Color desiredColor = lockOnUI.color;
        desiredColor.a = 0;

        Vector3 desiredRotation = new Vector3(0, 0, -1800);

        if (sequence != null)
            sequence.Kill();

        sequence = DOTween.Sequence()
            .Append(DOTween.To(() => rectTransform.sizeDelta, x => rectTransform.sizeDelta = x, new Vector2(150, 150), 0.2f))
            .Join(lockOnUI.DOColor(desiredColor, 0.2f))
            .OnComplete(() => lockOnUI.enabled = false)
            .Join(rectTransform.DORotate(desiredRotation, 0.4f, RotateMode.LocalAxisAdd));
    }

  
}
