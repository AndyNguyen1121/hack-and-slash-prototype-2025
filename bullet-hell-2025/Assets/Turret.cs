using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject bullet;
    private void Start()
    {
        InvokeRepeating("Shoot", 0f, 1f);
    }

    void Shoot()
    {
        Instantiate(bullet, transform.position, Quaternion.LookRotation(transform.forward));
    }
}
