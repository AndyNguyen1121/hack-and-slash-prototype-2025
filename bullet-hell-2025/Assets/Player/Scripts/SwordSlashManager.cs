using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class SwordSlashManager : MonoBehaviour
{
    [System.Serializable]
    public class SwordSlash
    {
        public GameObject swordSlash;
        public Transform transform;
        public bool followTransformTarget = false;
        public Vector3 offset;
    }

    public List<SwordSlash> SwordSlashList = new();
    public void ActivateSwordSlash(int i)
    {
        SwordSlash swordSlashPrefab = SwordSlashList[i];

        Quaternion desiredRotation = swordSlashPrefab.swordSlash.transform.rotation;

        if (!swordSlashPrefab.followTransformTarget)
        {
            desiredRotation = Quaternion.FromToRotation(swordSlashPrefab.swordSlash.transform.forward, transform.forward) * swordSlashPrefab.swordSlash.transform.rotation;
        }

        Vector3 desiredOffset = desiredRotation * swordSlashPrefab.offset;
        Vector3 spawnPosition = swordSlashPrefab.transform.position + desiredOffset;

        if (!swordSlashPrefab.followTransformTarget)
        {
            Instantiate(swordSlashPrefab.swordSlash, spawnPosition, desiredRotation);
        }
        else
        {
            Instantiate(swordSlashPrefab.swordSlash, swordSlashPrefab.transform);
        }


    }
}
