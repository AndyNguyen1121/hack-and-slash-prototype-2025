using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using Unity.Entities;

public class PositionAnimator : MonoBehaviour
{
    public bool useLocalPosition;
    public Vector3 startingPosition;
    public Vector3 endPosition;
    public float duration = 1f;
    public Ease easeFunction;

    public UnityEvent OnAnimationStart;
    public UnityEvent OnAnimationEnd;

    private Tween animationTween;

    private bool animating = false;
    private bool reverseAnimating = false;

    public void AnimatePosition()
    {
        if (animating)
            return;

        OnAnimationStart.Invoke();
        animating = true;
        reverseAnimating = false;

        if (animationTween != null)
        {
            animationTween.Kill();
        }

        if (useLocalPosition)
        {
            animationTween = transform.DOLocalMove(endPosition, duration, false).From(transform.localPosition).SetEase(easeFunction).OnComplete(() => OnAnimationEnd.Invoke());
        }
        else
        {
            animationTween = transform.DOMove(endPosition, duration, false).From(transform.position).SetEase(easeFunction).OnComplete(() => OnAnimationEnd.Invoke());
        }
    }

    public void ReverseAnimatePosition()
    {
        if (reverseAnimating)
            return; 

        OnAnimationStart.Invoke();
        animating = false;
        reverseAnimating = true;

        if (animationTween != null)
        {
            animationTween.Kill();
        }

        if (useLocalPosition)
        {
            animationTween = transform.DOLocalMove(startingPosition, duration, false).From(transform.localPosition).SetEase(easeFunction).OnComplete(() => OnAnimationEnd.Invoke());
        }
        else
        {
            animationTween = transform.DOMove(startingPosition, duration, false).From(transform.position).SetEase(easeFunction).OnComplete(() => OnAnimationEnd.Invoke());
        }
    }

    private void OnDestroy()
    {
        if (animationTween != null)
        {
            animationTween.Kill();
        }
    }
}
