using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Interact : MonoBehaviour
{
    public GameObject offset;              
    public Canvas _canva;                   
    public GameObject tableObject;          

    public float rotationSensitivity = 0.5f; 

    private GameObject targetObject;        
    private FPSController fpsController;    
    private Vector3 lastMousePosition;      
    private Transform examinedObject;       
    private Rigidbody examinedRigidbody;    

    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();
    
    public bool isExamining = false;        

    void Start()
    {
        _canva.enabled = false;
        targetObject = GameObject.Find("PlayerCapsule");

        if (targetObject != null)
        {
            fpsController = targetObject.GetComponent<FPSController>();
        }
        else
        {
            Debug.LogError("targetObject (PlayerCapsule) tidak ditemukan di scene.");
        }

        if (tableObject == null)
        {
            Debug.LogError("tableObject belum dihubungkan di Inspector.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Object"))
            {
                ToggleExamination(hit.transform);
            }
        }

        if (targetObject != null && tableObject != null && CheckUserClose())
        {
            if (isExamining)
            {
                _canva.enabled = false;
                Examine();
            }
            else
            {
                _canva.enabled = true;
                NonExamine();
            }
        }
        else
        {
            _canva.enabled = false;
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

            
            if (examinedRigidbody != null)
            {
                examinedRigidbody.isKinematic = true;
            }
        }

        lastMousePosition = Input.mousePosition;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        fpsController.canMove = false; 
    }

    void StopExamination()
    {
        if (examinedObject != null)
        {
            
            examinedObject.position = originalPositions[examinedObject];
            examinedObject.rotation = originalRotations[examinedObject];

          
            if (examinedRigidbody != null)
            {
                examinedRigidbody.isKinematic = false;
                examinedRigidbody = null;  
            }

            examinedObject = null;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        fpsController.canMove = true;   
    }

    void Examine()
    {
        if (examinedObject != null)
        {
            examinedObject.position = Vector3.Lerp(examinedObject.position, offset.transform.position, 0.2f);

            Vector3 deltaMouse = Input.mousePosition - lastMousePosition;
            examinedObject.Rotate(deltaMouse.x * rotationSensitivity * Vector3.up, Space.World);
            examinedObject.Rotate(deltaMouse.y * rotationSensitivity * Vector3.left, Space.World);
            lastMousePosition = Input.mousePosition;
        }
    }

    void NonExamine()
    {
        if (examinedObject != null)
        {
            if (originalPositions.ContainsKey(examinedObject))
            {
                examinedObject.position = Vector3.Lerp(examinedObject.position, originalPositions[examinedObject], 0.2f);
            }
            if (originalRotations.ContainsKey(examinedObject))
            {
                examinedObject.rotation = Quaternion.Slerp(examinedObject.rotation, originalRotations[examinedObject], 0.2f);
            }
        }
    }

    private bool CheckUserClose()
    {
        return Vector3.Distance(targetObject.transform.position, tableObject.transform.position) < 2;
    }
}
