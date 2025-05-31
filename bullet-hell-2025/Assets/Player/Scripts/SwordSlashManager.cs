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
    public ParticleSystem swordSlashTrail;
    public ParticleSystem swordTipTrail;
    public List<SwordSlash> SwordSlashList = new();
    private float startingLifetime;

    private void Start()
    {
        var trails = swordSlashTrail.trails;
        startingLifetime = trails.lifetime.constant; 
    }
    public void ActivateSwordSlash(int i)
    {
       /* SwordSlash swordSlashPrefab = SwordSlashList[i];

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
        }*/


    }

    public void ActivateSwordSlashParticle()
    {
        swordSlashTrail.Play();
        var trails = swordSlashTrail.trails;
        trails.enabled = true;
        trails.lifetime = startingLifetime;

        swordTipTrail.Play();
        trails = swordTipTrail.trails;
        trails.enabled = true;
        trails.lifetime = startingLifetime;

    }

    public void DisableSwordSlashParticle()
    {
        swordSlashTrail.Stop();
        var trails = swordSlashTrail.trails;
        trails.enabled = false;

        swordTipTrail.Stop();
        trails = swordTipTrail.trails;
        trails.enabled = false;

    }
}
