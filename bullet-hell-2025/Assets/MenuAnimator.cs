using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MenuAnimator : MonoBehaviour
{
    public Image backGroundImage;

    [Space(10)]
    public bool shrinkY = true;
    public float scaleYDuration = 0.25f;
    

    public bool slideOutFromLeft;
    public float slideOutDuration;
    public Vector3 startPosition;
    public Vector3 endPosition;

    private Tween animationTween;
    private Camera mainCam;

    private void OnEnable()
    {
        if (backGroundImage == null)
            return;

        if (mainCam == null)
        {
            mainCam = Camera.main;
        }

        if (animationTween != null)
        {
            animationTween.Kill();
        }

        if (shrinkY)
        {
           animationTween = backGroundImage.rectTransform.DOScaleY(1, scaleYDuration).From(0).SetEase(Ease.OutExpo).SetUpdate(true);
        }
        else if (slideOutFromLeft)
        {
           animationTween = backGroundImage.rectTransform.DOAnchorPos(endPosition, slideOutDuration).From(startPosition).SetUpdate(true);
        }

        AudioManager.instance.PlayOneShot(FMODEvents.instance.menuOpen, mainCam.transform.position);
    }
    
    public void DisableMenu()
    {
        if (animationTween != null)
        {
            animationTween.Kill();
        }

        if (shrinkY)
        {
            animationTween = backGroundImage.rectTransform.DOScaleY(0, scaleYDuration).From(backGroundImage.transform.localScale).SetUpdate(true);
        }
        else if (slideOutFromLeft)
        {
            animationTween = backGroundImage.rectTransform.DOAnchorPos(startPosition, slideOutDuration).From(endPosition).SetUpdate(true).OnComplete(() => gameObject.SetActive(false));
        }

        AudioManager.instance.PlayOneShot(FMODEvents.instance.menuClose, mainCam.transform.position);
    }

    private void OnDestroy()
    {
        if (animationTween != null)
        {
            animationTween.Kill();
        }
    }
}
