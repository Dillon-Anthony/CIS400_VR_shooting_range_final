using UnityEngine;

public class PistolShoot : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float fireRate = 0.5f; // Time between shots

    [Header("Aiming Settings")]
    public Transform rightController; // Right hand controller
    public Vector3 positionOffset = new Vector3(0, -0.05f, 0.1f); // Adjust to position weapon in hand
    public Vector3 rotationOffset = new Vector3(0, 0, 0); // Adjust weapon angle
    public float smoothSpeed = 15f; // How smooth the weapon follows hand

    [Header("Optional Effects")]
    public AudioClip shootSound;
    public GameObject muzzleFlashEffect;

    private float nextFireTime = 0f;
    private AudioSource audioSource;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        // Store original position for reference
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Find right controller if not assigned
        if (rightController == null)
        {
            GameObject controllerObj = GameObject.Find("RightControllerAnchor");
            if (controllerObj == null)
            {
                controllerObj = GameObject.Find("RightHandAnchor");
            }
            if (controllerObj != null)
            {
                rightController = controllerObj.transform;
            }
            else
            {
                Debug.LogError("Could not find right controller!");
            }
        }

        // Set up audio source if shoot sound exists
        if (shootSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = shootSound;
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        // Update weapon position to follow controller
        FollowController();

        // Check if right trigger is pressed and enough time has passed
        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FollowController()
    {
        if (rightController == null) return;

        // Calculate target position with offset
        Vector3 targetPosition = rightController.position + rightController.TransformDirection(positionOffset);

        // Calculate target rotation with offset
        Quaternion targetRotation = rightController.rotation * Quaternion.Euler(rotationOffset);

        // DIRECTLY set position and rotation (no lerp = no jitter)
        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }

    void Shoot()
    {
        if (bulletPrefab == null || bulletSpawnPoint == null)
        {
            Debug.LogError("Bullet prefab or spawn point not assigned!");
            return;
        }

        // Spawn bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        Debug.Log("Pistol fired!");

        // Play sound effect (if assigned)
        if (audioSource != null && shootSound != null)
        {
            audioSource.Play();
        }

        // Show muzzle flash (if assigned)
        if (muzzleFlashEffect != null)
        {
            GameObject flash = Instantiate(muzzleFlashEffect, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            Destroy(flash, 0.1f);
        }

        // Haptic feedback
        OVRInput.SetControllerVibration(0.5f, 0.1f, OVRInput.Controller.RTouch);
    }
}