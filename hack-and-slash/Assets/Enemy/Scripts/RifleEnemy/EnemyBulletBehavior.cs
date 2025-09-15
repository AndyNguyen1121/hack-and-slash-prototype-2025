using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletBehavior : MonoBehaviour
{
    [Header("Initialization")]
    public float damage;
    public float speed;
    public LayerMask whatIsPlayer;
    public LayerMask whatIsEnemy;
    private LayerMask activeCollisionLayerMask;
    private Rigidbody rb;

    [Header("Spherecast")]
    public float sphereCastRadius;
    //public float sphereCastDistance;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        activeCollisionLayerMask = whatIsPlayer;
        rb.velocity = transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {
        

        HandleCollisionDetection();
    }

    void HandleCollisionDetection()
    {
        RaycastHit hit;
        float castDistance = speed * Time.fixedDeltaTime;

        if (Physics.SphereCast(transform.position, sphereCastRadius, transform.forward, 
            out hit, (rb.velocity.magnitude * Time.fixedDeltaTime), activeCollisionLayerMask))
        {
            IDamageable damageScript;
            if (!hit.collider.TryGetComponent<IDamageable>(out damageScript))
                return;

            if (damageScript.TakeDamage(damage, hit.collider.ClosestPointOnBounds(transform.position), gameObject))
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.laserHit, transform.position);
                Destroy(gameObject);
            }
            else
            {
                activeCollisionLayerMask = whatIsEnemy;
                rb.velocity = -transform.forward * speed;
            }
            


        }

        Debug.DrawRay(transform.position, transform.forward * (rb.velocity.magnitude * Time.fixedDeltaTime), Color.red);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, sphereCastRadius);
    }


}
