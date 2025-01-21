using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Interact : MonoBehaviour
{
    public GameObject offset;                // Position where the object moves during inspection
    public Canvas _canva;                    // Canvas UI to display during inspection
    public float inspectDistance = 3.0f;     // Distance required to enable object inspection
    public float rotationSensitivity = 0.5f; // Sensitivity for object rotation

    private GameObject targetObject;         // Player object
    private FPSController fpsController;     // Script controlling player movement
    private Camera mainCamera;               // Main camera reference
    private Vector3 lastMousePosition;       // Last mouse position for calculating rotation
    private Transform examinedObject;        // Currently inspected object
    private Rigidbody examinedRigidbody;     // Rigidbody of the examined object
    private Quaternion originalPlayerRotation; // Store original PlayerCapsule rotation
    private Quaternion originalCameraRotation; // Store original Camera rotation

    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();

    public bool isExamining = false;         // Indicates if the player is examining an object

    void Start()
    {
        _canva.enabled = false; // Disable canvas at start
        targetObject = GameObject.Find("PlayerCapsule");
        mainCamera = Camera.main;

        if (targetObject != null)
        {
            fpsController = targetObject.GetComponent<FPSController>();
            originalPlayerRotation = targetObject.transform.rotation; // Store the initial rotation
        }
        else
        {
            Debug.LogError("targetObject (PlayerCapsule) not found in the scene.");
        }

        if (mainCamera != null)
        {
            originalCameraRotation = mainCamera.transform.rotation; // Store the initial camera rotation
        }
        else
        {
            Debug.LogError("Main Camera not found in the scene.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Object"))
            {
                if (Vector3.Distance(targetObject.transform.position, hit.transform.position) <= inspectDistance)
                {
                    ToggleExamination(hit.transform);
                }
                else
                {
                    Debug.Log("Object is too far to inspect.");
                }
            }
        }

        if (isExamining)
        {
            _canva.enabled = false;
            Examine();
        }
        else
        {
            _canva.enabled = true;
        }
    }

    public void ToggleExamination(Transform newExaminedObject)
    {
        isExamining = !isExamining;

        if (isExamining)
        {
            StartExamination(newExaminedObject);
        }
        else
        {
            StopExamination();
        }
    }

    void StartExamination(Transform newExaminedObject)
    {
        examinedObject = newExaminedObject;
        if (examinedObject != null)
        {
            originalPositions[examinedObject] = examinedObject.position;
            originalRotations[examinedObject] = examinedObject.rotation;
            examinedRigidbody = examinedObject.GetComponent<Rigidbody>();

            // Disable Rigidbody physics
            if (examinedRigidbody != null)
            {
                examinedRigidbody.isKinematic = true;
            }
        }

        // Rotate PlayerCapsule to Y = 270
        originalPlayerRotation = targetObject.transform.rotation; // Store the original rotation
        targetObject.transform.rotation = Quaternion.Euler(0, 270, 0);

        // Set Camera X Rotation to 0
        if (mainCamera != null)
        {
            originalCameraRotation = mainCamera.transform.rotation; // Store the original camera rotation
            Vector3 cameraEuler = mainCamera.transform.eulerAngles;
            mainCamera.transform.rotation = Quaternion.Euler(0, cameraEuler.y, cameraEuler.z);
        }

        lastMousePosition = Input.mousePosition;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        fpsController.canMove = false; // Disable player movement
    }

    void StopExamination()
    {
        if (examinedObject != null)
        {
            // Restore object to its original position and rotation
            examinedObject.position = originalPositions[examinedObject];
            examinedObject.rotation = originalRotations[examinedObject];

            // Re-enable Rigidbody physics
            if (examinedRigidbody != null)
            {
                examinedRigidbody.isKinematic = false;
                examinedRigidbody = null;
            }

            examinedObject = null;
        }

        // Restore PlayerCapsule rotation
        targetObject.transform.rotation = originalPlayerRotation;

        // Restore Camera rotation
        if (mainCamera != null)
        {
            mainCamera.transform.rotation = originalCameraRotation;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        fpsController.canMove = true; // Re-enable player movement
    }

    void Examine()
    {
        if (examinedObject != null)
        {
            // Move the object toward the offset position
            examinedObject.position = Vector3.Lerp(examinedObject.position, offset.transform.position, 0.2f);

            // Rotate the object based on mouse movement
            Vector3 deltaMouse = Input.mousePosition - lastMousePosition;
            examinedObject.Rotate(deltaMouse.x * rotationSensitivity * Vector3.up, Space.World);
            examinedObject.Rotate(deltaMouse.y * rotationSensitivity * Vector3.left, Space.World);
            lastMousePosition = Input.mousePosition;
        }
    }
}
