using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class SwordTrail : MonoBehaviour
{
    public GameObject tip;
    public GameObject swordBase;
    public GameObject meshParent;

    public GameObject trailMesh;
    public int trailFrameLength;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private int frameCount;
    private Vector2[] uvs;
    private Vector3 previousTipPosition;
    private Vector3 previousBasePosition;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        trailMesh.GetComponent<MeshFilter>().mesh = mesh;

        vertices = new Vector3[trailFrameLength * 12];
        triangles = new int[vertices.Length];

        previousBasePosition = meshParent.transform.InverseTransformPoint(swordBase.transform.position);
        previousTipPosition = meshParent.transform.InverseTransformPoint(tip.transform.position);

        uvs = new Vector2[vertices.Length];

    }

    // Update is called once per frame
    void Update()
    {
        if (frameCount == (trailFrameLength * 12))
        {
            frameCount = 0;
        }

        Vector3 localTip = meshParent.transform.InverseTransformPoint(tip.transform.position);
        Vector3 localBase = meshParent.transform.InverseTransformPoint(swordBase.transform.position);

        Vector3 localPreviousTip = meshParent.transform.InverseTransformPoint(previousTipPosition);
        Vector3 localPreviousBase = meshParent.transform.InverseTransformPoint(previousBasePosition);

        vertices[frameCount] = swordBase.transform.position;
        vertices[frameCount + 1] = tip.transform.position;
        vertices[frameCount + 2] = previousTipPosition;
        vertices[frameCount + 3] = swordBase.transform.position;
        vertices[frameCount + 4] = previousTipPosition;
        vertices[frameCount + 5] = tip.transform.position;

        //Draw fill in triangle vertices
        vertices[frameCount + 6] = previousTipPosition;
        vertices[frameCount + 7] = swordBase.transform.position;
        vertices[frameCount + 8] = previousBasePosition;
        vertices[frameCount + 9] = previousTipPosition;
        vertices[frameCount + 10] = previousBasePosition;
        vertices[frameCount + 11] = swordBase.transform.position;

        //Set triangles
        triangles[frameCount] = frameCount;
        triangles[frameCount + 1] = frameCount + 1;
        triangles[frameCount + 2] = frameCount + 2;
        triangles[frameCount + 3] = frameCount + 3;
        triangles[frameCount + 4] = frameCount + 4;
        triangles[frameCount + 5] = frameCount + 5;
        triangles[frameCount + 6] = frameCount + 6;
        triangles[frameCount + 7] = frameCount + 7;
        triangles[frameCount + 8] = frameCount + 8;
        triangles[frameCount + 9] = frameCount + 9;
        triangles[frameCount + 10] = frameCount + 10;
        triangles[frameCount + 11] = frameCount + 11;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        //Track the previous base and tip positions for the next frame
        previousBasePosition = swordBase.transform.position;
        previousTipPosition = tip.transform.position;
        frameCount += 12;


        for(int i = 0; i < frameCount + 12; i += 6)
{
            // Calculate the segment index along the trail
            float segment = i / 6f; // each 6 vertices = 1 segment
            float v0 = segment / trailFrameLength;       // start of segment along trail
            float v1 = (segment + 1) / trailFrameLength; // end of segment along trail

            // Bottom-left, top-left, top-right for first triangle
            uvs[i] = new Vector2(0, v0); // base-left
            uvs[i + 1] = new Vector2(1, v0); // base-right
            uvs[i + 2] = new Vector2(1, v1); // tip-right

            // Second triangle
            uvs[i + 3] = new Vector2(0, v0); // base-left duplicate
            uvs[i + 4] = new Vector2(1, v1); // tip-right duplicate
            uvs[i + 5] = new Vector2(0, v1); // tip-left
        }


        mesh.uv = uvs;
        mesh.RecalculateBounds();

    }
}
