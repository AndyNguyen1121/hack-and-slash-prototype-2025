using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

public class MenuClickManager : MonoBehaviour
{
    public GameObject nextMenuToEnable;
    public GameObject menuToDisable;
    public float shrinkDuration = 0.2f;
    public float disableDuration = 0.1f;
    public List<Button> otherButtons;

    private Tween animationTween;
    private void OnEnable()
    {
        if (animationTween != null)
        {
            animationTween.Kill();
        }

        transform.localScale = Vector3.one;

        TextMeshProUGUI[] text = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var mainText in text)
        {
            Color color = mainText.color;
            color.a = 1;

            mainText.color = color;
            
        }
    }

    public void FadeOut()
    {
        TextMeshProUGUI[] text = GetComponentsInChildren<TextMeshProUGUI>();

        foreach (var mainText in text)
        {
            Color color = mainText.color;
            color.a = 0;
            mainText.DOColor(color, disableDuration).SetUpdate(true);
        }
    }
    public void OnPointerClick()
    {

        if (animationTween != null)
        {
            animationTween.Kill();
        }

        if (nextMenuToEnable != null)
        {
            nextMenuToEnable.SetActive(true);
        }


        transform.DOScaleY(0, shrinkDuration).SetUpdate(true);
        animationTween = transform.DOScaleX(1.25f, shrinkDuration).SetUpdate(true).OnComplete(() =>
        {
            if (nextMenuToEnable != null)
            {
                nextMenuToEnable.SetActive(true);
            }

            if (menuToDisable != null)
            {
                menuToDisable.SetActive(false);
            }
        });

        foreach (var button in otherButtons)
        {
            if (button.gameObject == this.gameObject)
                continue;

            if (button.TryGetComponent<MenuClickManager>(out MenuClickManager clickManager))
            {
                clickManager.FadeOut();
            }
        }
    }
}
