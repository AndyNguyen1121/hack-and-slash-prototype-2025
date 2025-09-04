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

    public List<Subscene> unloadScenes;
    public List<Subscene> loadScenes;

    private Tween animationTween;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            AnimatePosition();
        }
    }

    public void AnimatePosition()
    {
        OnAnimationStart.Invoke();

        foreach (var sc in unloadScenes)
        {
            SubsceneLoader.instance.UnloadSubscene(sc);
        }
        
        foreach (var sc in loadScenes)
        {
            SubsceneLoader.instance.LoadSubscene(sc);
        }

        if (animationTween != null)
        {
            animationTween.Kill();
        }

        if (useLocalPosition)
        {
            animationTween = transform.DOLocalMove(endPosition, duration, false).From(startingPosition).SetEase(easeFunction).OnComplete(() => OnAnimationEnd.Invoke());
        }
        else
        {
            animationTween = transform.DOMove(endPosition, duration, false).From(startingPosition).SetEase(easeFunction).OnComplete(() => OnAnimationEnd.Invoke());
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
