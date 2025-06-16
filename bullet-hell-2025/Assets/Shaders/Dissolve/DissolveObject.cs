using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.XR;

public class DissolveObject : MonoBehaviour
{
    public Material dissolveMaterial;
    public float fadeSpeed = 2f;

    private MeshRenderer[] meshRenderers;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    
    void Start()
    {
        Initialize();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Initialize()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
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
                           1, fadeSpeed);
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
                           1, fadeSpeed);
            }

            meshes.materials = allMaterials;
        }


    }

}
