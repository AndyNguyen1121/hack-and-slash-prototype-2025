using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.XR;

public class DissolveObject : MonoBehaviour
{
    public Material dissolveMaterial;
    public Material reverseDissolveMaterial;
    public Material originalMaterial;
    public float fadeSpeed = 2f;

    private MeshRenderer[] meshRenderers;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;

    private Material[][] originalMeshRendererMaterials;
    private Material[][] originalSkinnedMeshRendererMaterials;

    void Awake()
    {
        Initialize();
        ReverseDissolve();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Initialize()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        originalMeshRendererMaterials = new Material[meshRenderers.Length][];
        originalSkinnedMeshRendererMaterials = new Material[skinnedMeshRenderers.Length][];

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            originalMeshRendererMaterials[i] = meshRenderers[i].materials;
        }

        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
            originalSkinnedMeshRendererMaterials[i] = skinnedMeshRenderers[i].materials;
        }
    }

    public void StartDissolve()
    {
        foreach (MeshRenderer meshes in meshRenderers)
        {
            Material[] allMaterials = meshes.materials;

            for (int i = 0; i < allMaterials.Length; i++)
            {
                Texture originalTexture = allMaterials[i].GetTexture("_BaseMap");

                Material materialInstance = new Material(dissolveMaterial);
                materialInstance.SetTexture("_BaseMap", originalTexture);

                allMaterials[i] = materialInstance;

                DOTween.To(() => materialInstance.GetFloat("_FadeValue"),
                           x => materialInstance.SetFloat("_FadeValue", x),
                           1, fadeSpeed).SetEase(Ease.Linear);
            }

            meshes.materials = allMaterials;
        }

        foreach (SkinnedMeshRenderer meshes in skinnedMeshRenderers)
        {
            Material[] allMaterials = meshes.materials;

            for (int i = 0; i < allMaterials.Length; i++)
            {
                Texture originalTexture = allMaterials[i].GetTexture("_BaseMap");

                Material materialInstance = new Material(dissolveMaterial);
                materialInstance.SetTexture("_BaseMap", originalTexture);

                allMaterials[i] = materialInstance;

                DOTween.To(() => materialInstance.GetFloat("_FadeValue"),
                           x => materialInstance.SetFloat("_FadeValue", x),
                           1, fadeSpeed).SetEase(Ease.Linear);
            }

            meshes.materials = allMaterials;
        }


    }

    public void ReverseDissolve()
    {
        foreach (MeshRenderer meshes in meshRenderers)
        {
            Material[] allMaterials = meshes.materials;

            for (int i = 0; i < allMaterials.Length; i++)
            {
                Texture originalTexture = allMaterials[i].GetTexture("_BaseMap");

                Material materialInstance = new Material(reverseDissolveMaterial);
                materialInstance.SetTexture("_BaseMap", originalTexture);

                allMaterials[i] = materialInstance;

                DOTween.To(() => materialInstance.GetFloat("_FadeValue"),
                           x => materialInstance.SetFloat("_FadeValue", x),
                           0f, fadeSpeed).SetEase(Ease.Linear).OnComplete(RecoverOriginalMaterials);
            }

            meshes.materials = allMaterials;
        }

        foreach (SkinnedMeshRenderer meshes in skinnedMeshRenderers)
        {
            Material[] allMaterials = meshes.materials;

            for (int i = 0; i < allMaterials.Length; i++)
            {
                Texture originalTexture = allMaterials[i].GetTexture("_BaseMap");

                Material materialInstance = new Material(reverseDissolveMaterial);
                materialInstance.SetTexture("_BaseMap", originalTexture);

                allMaterials[i] = materialInstance;

                DOTween.To(() => materialInstance.GetFloat("_FadeValue"),
                           x => materialInstance.SetFloat("_FadeValue", x),
                           0f, fadeSpeed).SetEase(Ease.Linear).OnComplete(RecoverOriginalMaterials);
            }

            meshes.materials = allMaterials;
        }


    }

    private void RecoverOriginalMaterials()
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].materials = originalMeshRendererMaterials[i];
        }

        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
            skinnedMeshRenderers[i].materials = originalSkinnedMeshRendererMaterials[i];
        }
    }
}
