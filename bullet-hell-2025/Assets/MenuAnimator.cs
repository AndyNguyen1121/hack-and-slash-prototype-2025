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
    private void OnEnable()
    {
        if (backGroundImage == null)
            return;

        if (animationTween != null)
        {
            animationTween.Kill();
        }

        if (shrinkY)
        {
           animationTween = backGroundImage.transform.DOScaleY(1, scaleYDuration).From(0).SetEase(Ease.OutExpo).SetUpdate(true);
        }
        else if (slideOutFromLeft)
        {
           animationTween = backGroundImage.rectTransform.DOAnchorPos(endPosition, slideOutDuration).From(startPosition).SetUpdate(true);
        }
    }
    
    public void DisableMenu()
    {
        if (animationTween != null)
        {
            animationTween.Kill();
        }
        animationTween = backGroundImage.transform.DOScaleY(0, scaleYDuration).From(backGroundImage.transform.localScale);
    }
}
