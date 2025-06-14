using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;

public class PostProcessManager : MonoBehaviour
{
    [HideInInspector]
    public static PostProcessManager instance;

    [SerializeField] Volume postProcessVolume;

    ChromaticAberration chromaticAberration;
    DG.Tweening.Sequence chromaticTween;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;

        postProcessVolume.profile.TryGet(out chromaticAberration);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateParryPostProcessingEffect()
    {
        if (chromaticTween != null)
            chromaticTween.Kill();

        chromaticTween = DOTween.Sequence();

        chromaticTween.Append(DOTween.To(() => chromaticAberration.intensity.value, x => chromaticAberration.intensity.value = x, 0.35f, 0.02f))
         .AppendInterval(0.15f)
            .OnComplete(() => DOTween.To(() => chromaticAberration.intensity.value, x => chromaticAberration.intensity.value = x, 0, 0.1f));
    }
}
