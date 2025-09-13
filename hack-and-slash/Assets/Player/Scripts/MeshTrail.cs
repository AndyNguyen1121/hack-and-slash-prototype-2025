using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public float activeTime = 2;
    public bool trailActive = false;
    public float meshRefreshRate = 0.1f;
    public float cloneDestroy = 0.05f;
    public Coroutine dashCoroutine;

    [Header("Mesh")]
    private SkinnedMeshRenderer[] skinnedMeshRenderer;
    public Material mat;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AttemptToExecuteDash()
    {
        if (trailActive)
        {
            StopDash();
        }

        StartCoroutine(ActivateTrail(activeTime));
    }

    public void StopDash()
    {
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
            trailActive = false;
        }
    }

    private IEnumerator ActivateTrail(float time)
    {
        trailActive = true;
        while (time > 0)
        {
            time -= meshRefreshRate;

            if (skinnedMeshRenderer == null)
            {
                skinnedMeshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();
            }

            for (int i = 0; skinnedMeshRenderer.Length > i; i++)
            {
                GameObject gObj = new GameObject();

                gObj.transform.SetPositionAndRotation(transform.position, transform.rotation);

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedMeshRenderer[i].BakeMesh(mesh);

                mf.mesh = mesh;
                mr.material = mat;

                Destroy(gObj, cloneDestroy);
            }


            yield return new WaitForSeconds(meshRefreshRate);
        }

        trailActive = false;
    }


}

