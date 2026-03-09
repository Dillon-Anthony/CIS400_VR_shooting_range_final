using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [HideInInspector]
    public TargetSpawner spawner;

    [HideInInspector]
    public float lifetime = 3f;

    public AudioClip hitSound;
    public GameObject hitEffect; // Particle effect on hit (optional)

    private bool isHit = false;
    private float spawnTime;

    void Start()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        // Despawn after lifetime expires
        if (Time.time - spawnTime >= lifetime && !isHit)
        {
            Miss();
        }
    }

    public void Hit()
    {
        if (isHit) return; // Prevent double hits

        isHit = true;

        // Visual/audio feedback
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        // Flash color
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.green;
        }

        // Notify spawner
        if (spawner != null)
        {
            spawner.OnTargetDestroyed(true);
        }

        // Destroy after brief delay
        Destroy(gameObject, 0.2f);
    }

    void Miss()
    {
        if (isHit) return;

        isHit = true;

        // Notify spawner (no points)
        if (spawner != null)
        {
            spawner.OnTargetDestroyed(false);
        }

        Destroy(gameObject);
    }

    // This will be called by projectiles/raycasts
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Hit();
        }
    }
}