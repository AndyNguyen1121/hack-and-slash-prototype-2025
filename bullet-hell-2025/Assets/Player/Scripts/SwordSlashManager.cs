using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSlashManager : MonoBehaviour
{
    [System.Serializable]
    public class SwordSlash
    {
        public GameObject swordSlash;
        public Transform transform;
        public Vector3 offset;
    }

    public List<SwordSlash> SwordSlashList = new();
    public void ActivateSwordSlash(int i)
    {
        SwordSlash swordSlashPrefab = SwordSlashList[i];
        Quaternion desiredRotation = Quaternion.FromToRotation(swordSlashPrefab.swordSlash.transform.forward, transform.forward) * swordSlashPrefab.swordSlash.transform.rotation;
        Vector3 rotatedOffset = desiredRotation * swordSlashPrefab.offset;
        Vector3 spawnPosition = swordSlashPrefab.transform.position + rotatedOffset;
        Instantiate(swordSlashPrefab.swordSlash, spawnPosition, desiredRotation);
    }
}
