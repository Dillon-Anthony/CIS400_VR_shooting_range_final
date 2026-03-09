using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponTeleporter : MonoBehaviour
{
    [Header("Which scene to load")]
    public string sceneToLoad = "PistolRange";

    [Header("Visual Feedback")]
    public Material highlightMaterial;
    public Color highlightColor = Color.yellow;

    private Renderer weaponRenderer;
    private Material originalMaterial;
    private Color originalColor;
    private bool canGrab = true;

    void Start()
    {
        // Get the weapon's renderer for visual feedback
        weaponRenderer = GetComponentInChildren<Renderer>();
        if (weaponRenderer != null)
        {
            originalMaterial = weaponRenderer.material;
            originalColor = weaponRenderer.material.color;
        }

        Debug.Log(gameObject.name + " teleporter initialized. Will load: " + sceneToLoad);
    }

    void Update()
    {
        // Check if right trigger is pressed
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch) && canGrab)
        {
            // Raycast from right controller
            Transform rightController = GameObject.Find("RightControllerAnchor").transform;

            if (rightController == null)
            {
                rightController = GameObject.Find("RightHandAnchor").transform;
            }

            if (rightController != null)
            {
                RaycastHit hit;
                if (Physics.Raycast(rightController.position, rightController.forward, out hit, 10f))
                {
                    // Check if we hit this weapon
                    if (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform))
                    {
                        Debug.Log("Grabbed " + gameObject.name + "! Teleporting to " + sceneToLoad);
                        TeleportToRange();
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Visual feedback when controller is near
        if (other.name.Contains("Controller") || other.name.Contains("Hand"))
        {
            HighlightWeapon();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Controller") || other.name.Contains("Hand"))
        {
            UnhighlightWeapon();
        }
    }

    void HighlightWeapon()
    {
        if (weaponRenderer != null)
        {
            if (highlightMaterial != null)
            {
                weaponRenderer.material = highlightMaterial;
            }
            else
            {
                weaponRenderer.material.color = highlightColor;
            }
        }
    }

    void UnhighlightWeapon()
    {
        if (weaponRenderer != null)
        {
            if (originalMaterial != null)
            {
                weaponRenderer.material = originalMaterial;
            }
            else
            {
                weaponRenderer.material.color = originalColor;
            }
        }
    }

    void TeleportToRange()
    {
        canGrab = false; // Prevent multiple triggers

        // Flash the weapon
        if (weaponRenderer != null)
        {
            weaponRenderer.material.color = Color.green;
        }

        // Load the scene
        SceneManager.LoadScene(sceneToLoad);
    }
}