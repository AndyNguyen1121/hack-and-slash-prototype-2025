using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutImage : MonoBehaviour
{
    public float duration = 1;
    public Image backgroundImage;
    private void Awake()
    {
        backgroundImage = GetComponentInChildren<Image>();
    }

    private void OnEnable()
    {
        backgroundImage.color = Color.black;

        Color targetColor = backgroundImage.color;
        targetColor.a = 0;

        backgroundImage.DOColor(targetColor, duration).OnComplete(() => Destroy(gameObject));
    }
}
