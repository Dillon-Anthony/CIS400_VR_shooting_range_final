using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToHub : MonoBehaviour
{
    public Material normalMaterial;
    public Material highlightMaterial;

    private Renderer buttonRenderer;

    void Start()
    {
        buttonRenderer = GetComponent<Renderer>();
        Debug.Log("ReturnToHub button initialized!");
    }

    void Update()
    {
        // Find right controller
        Transform rightController = GameObject.Find("RightControllerAnchor").transform;

        if (rightController == null)
        {
            rightController = GameObject.Find("RightHandAnchor").transform;
        }

        if (rightController != null)
        {
            // DRAW THE RAYCAST - Always visible for debugging
            Debug.DrawRay(rightController.position, rightController.forward * 10f, Color.red);

            // Check if right trigger is pressed
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                Debug.Log("Right trigger pressed!");

                RaycastHit hit;
                if (Physics.Raycast(rightController.position, rightController.forward, out hit, 10f))
                {
                    Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

                    // Draw line to what we hit
                    Debug.DrawLine(rightController.position, hit.point, Color.green, 2f);

                    if (hit.collider.gameObject == gameObject)
                    {
                        Debug.Log("HIT THE RETURN BUTTON!");
                        FlashButton();
                        ReturnToHubScene();
                    }
                }
                else
                {
                    Debug.Log("Raycast didn't hit anything");
                }
            }
        }
        else
        {
            Debug.LogError("Right controller not found!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered trigger: " + other.name);
        if (other.name.Contains("Controller") && highlightMaterial != null)
        {
            buttonRenderer.material = highlightMaterial;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Controller") && normalMaterial != null)
        {
            buttonRenderer.material = normalMaterial;
        }
    }

    void FlashButton()
    {
        // Flash grey when pressed
        if (buttonRenderer != null)
        {
            buttonRenderer.material.color = Color.grey;
            Debug.Log("Button flashed grey!");
        }
    }

    void ReturnToHubScene()
    {
        Debug.Log("ReturnToHubScene() called!");
        Debug.Log("Attempting to load scene: HubScene");

        // Check if scene exists in build settings
        if (Application.CanStreamedLevelBeLoaded("HubWorld"))
        {
            Debug.Log("HubScene found in build settings, loading...");
            SceneManager.LoadScene("HubWorld");
        }
        else
        {
            Debug.LogError("HubScene NOT found in build settings! Check File -> Build Settings");
        }
    }
}