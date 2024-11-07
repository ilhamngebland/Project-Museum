using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Interact : MonoBehaviour
{
    public GameObject offset;               // Objek tempat objek lain akan bergerak saat di-inspect
    public Canvas _canva;                   // Canvas yang akan diaktifkan/nonaktifkan
    public GameObject tableObject;          // Objek yang akan diperiksa jaraknya dari player

    public float rotationSensitivity = 0.5f; // Sensitivitas rotasi saat inspect

    private GameObject targetObject;        // Referensi ke objek pemain
    private FPSController fpsController;    // Kontrol untuk menghentikan gerakan FPS saat inspeksi
    private Vector3 lastMousePosition;      // Posisi mouse terakhir untuk menghitung delta
    private Transform examinedObject;       // Objek yang sedang di-inspect
    private Rigidbody examinedRigidbody;    // Rigidbody dari objek yang sedang di-inspect

    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();
    
    public bool isExamining = false;        // Status apakah sedang dalam mode inspect atau tidak

    void Start()
    {
        _canva.enabled = false;

        // Inisialisasi targetObject dan fpsController
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
            // Simpan posisi, rotasi, dan referensi ke Rigidbody dari objek yang sedang di-inspect
            originalPositions[examinedObject] = examinedObject.position;
            originalRotations[examinedObject] = examinedObject.rotation;
            examinedRigidbody = examinedObject.GetComponent<Rigidbody>();

            // Nonaktifkan Rigidbody jika ada
            if (examinedRigidbody != null)
            {
                examinedRigidbody.isKinematic = true;
            }
        }

        lastMousePosition = Input.mousePosition;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        fpsController.canMove = false;  // Menghentikan gerakan pemain saat inspect
    }

    void StopExamination()
    {
        if (examinedObject != null)
        {
            // Kembalikan posisi dan rotasi objek
            examinedObject.position = originalPositions[examinedObject];
            examinedObject.rotation = originalRotations[examinedObject];

            // Aktifkan kembali Rigidbody jika ada
            if (examinedRigidbody != null)
            {
                examinedRigidbody.isKinematic = false;
                examinedRigidbody = null;  // Hapus referensi setelah selesai
            }

            examinedObject = null;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        fpsController.canMove = true;   // Mengizinkan kembali gerakan pemain
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
