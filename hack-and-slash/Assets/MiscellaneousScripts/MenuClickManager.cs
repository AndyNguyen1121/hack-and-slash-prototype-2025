using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

public class MenuClickManager : MonoBehaviour, ISelectHandler, ISubmitHandler, IPointerClickHandler
{
    public GameObject nextMenuToEnable;
    public GameObject menuToDisable;
    public float shrinkDuration = 0.2f;
    public float disableDuration = 0.1f;
    public List<Transform> otherButtons;

    private Tween animationTween;
    private Camera mainCam;
    private void OnEnable()
    {
        if (animationTween != null)
        {
            animationTween.Kill();
        }

        if (mainCam == null)
        {
            mainCam = Camera.main;
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
            animationTween = mainText.DOColor(color, disableDuration).SetUpdate(true);
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


        animationTween = transform.DOScaleY(0, shrinkDuration).SetUpdate(true).OnComplete(() =>
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

        foreach (var targetTransform in otherButtons)
        {
            if (targetTransform.gameObject == this.gameObject)
                continue;

            if (targetTransform.TryGetComponent<MenuClickManager>(out MenuClickManager clickManager))
            {
                clickManager.FadeOut();
            }
        }

        
    }
    public void OnSelect(BaseEventData eventData)
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.buttonSwitch, mainCam.transform.position);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.buttonPress, mainCam.transform.position);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.buttonPress, mainCam.transform.position);
    }
    private void OnDestroy()
    {
        if (animationTween != null)
        {
            animationTween.Kill();
        }
    }
}
