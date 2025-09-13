using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.Rendering;

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


        mesh.uv = UvCalculator.CalculateUVs(vertices, 1);
        mesh.RecalculateTangents();

    }

    public class UvCalculator
    {
        private enum Facing { Up, Forward, Right };

        public static Vector2[] CalculateUVs(Vector3[] v/*vertices*/, float scale)
        {
            var uvs = new Vector2[v.Length];

            for (int i = 0; i < uvs.Length; i += 3)
            {
                int i0 = i;
                int i1 = i + 1;
                int i2 = i + 2;

                Vector3 v0 = v[i0];
                Vector3 v1 = v[i1];
                Vector3 v2 = v[i2];

                Vector3 side1 = v1 - v0;
                Vector3 side2 = v2 - v0;
                var direction = Vector3.Cross(side1, side2);
                var facing = FacingDirection(direction);
                switch (facing)
                {
                    case Facing.Forward:
                        uvs[i0] = ScaledUV(v0.x, v0.y, scale);
                        uvs[i1] = ScaledUV(v1.x, v1.y, scale);
                        uvs[i2] = ScaledUV(v2.x, v2.y, scale);
                        break;
                    case Facing.Up:
                        uvs[i0] = ScaledUV(v0.x, v0.z, scale);
                        uvs[i1] = ScaledUV(v1.x, v1.z, scale);
                        uvs[i2] = ScaledUV(v2.x, v2.z, scale);
                        break;
                    case Facing.Right:
                        uvs[i0] = ScaledUV(v0.y, v0.z, scale);
                        uvs[i1] = ScaledUV(v1.y, v1.z, scale);
                        uvs[i2] = ScaledUV(v2.y, v2.z, scale);
                        break;
                }
            }
            return uvs;
        }

        private static bool FacesThisWay(Vector3 v, Vector3 dir, Facing p, ref float maxDot, ref Facing ret)
        {
            float t = Vector3.Dot(v, dir);
            if (t > maxDot)
            {
                ret = p;
                maxDot = t;
                return true;
            }
            return false;
        }

        private static Facing FacingDirection(Vector3 v)
        {
            var ret = Facing.Up;
            float maxDot = Mathf.NegativeInfinity;

            if (!FacesThisWay(v, Vector3.right, Facing.Right, ref maxDot, ref ret))
                FacesThisWay(v, Vector3.left, Facing.Right, ref maxDot, ref ret);

            if (!FacesThisWay(v, Vector3.forward, Facing.Forward, ref maxDot, ref ret))
                FacesThisWay(v, Vector3.back, Facing.Forward, ref maxDot, ref ret);

            if (!FacesThisWay(v, Vector3.up, Facing.Up, ref maxDot, ref ret))
                FacesThisWay(v, Vector3.down, Facing.Up, ref maxDot, ref ret);

            return ret;
        }

        private static Vector2 ScaledUV(float uv1, float uv2, float scale)
        {
            return new Vector2(uv1 / scale, uv2 / scale);
        }
    }
}
