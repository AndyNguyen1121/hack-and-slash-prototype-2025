using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MenuAnimator : MonoBehaviour
{
    public float scaleYDuration = 0.25f;
    public Image backGroundImage;
    private void OnEnable()
    {
        if (backGroundImage == null)
            return;
        backGroundImage.transform.DOScaleY(1, scaleYDuration).From(0).SetEase(Ease.OutExpo);
    }
    
    public void DisableMenu()
    {
        backGroundImage.transform.DOScaleY(0, scaleYDuration).From(backGroundImage.transform.localScale);
    }
}
