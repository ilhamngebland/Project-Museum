using UnityEngine;

public class ObjectInspect : MonoBehaviour
{
    public Transform inspectOffset; // Tempat objek akan dipindahkan saat di-inspect
    private Transform originalParent; // Menyimpan parent asli objek
    private GameObject currentObject; // Objek yang sedang di-inspect
    private bool isInspecting = false;
    private Vector3 lastMousePosition; // Menyimpan posisi terakhir mouse untuk rotasi

    void Update()
    {
        if (!isInspecting)
        {
            // Raycast untuk mendeteksi objek
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.CompareTag("Object"))
                {
                    currentObject = hit.transform.gameObject;

                    // Tampilkan UI petunjuk interaksi
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        StartInspecting();
                    }
                }
            }
        }
        else
        {
            // Keluar dari mode inspect
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopInspecting();
            }

            // Rotasi objek menggunakan input mouse saat tombol mouse ditekan
            if (Input.GetMouseButton(0)) // Tombol kiri mouse ditekan
            {
                Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
                float rotationSpeed = 100f;
                currentObject.transform.Rotate(Vector3.up, -mouseDelta.x * rotationSpeed * Time.deltaTime, Space.World);
                currentObject.transform.Rotate(Vector3.right, mouseDelta.y * rotationSpeed * Time.deltaTime, Space.World);
            }

            // Update posisi terakhir mouse
            lastMousePosition = Input.mousePosition;
        }
    }

    private void StartInspecting()
    {
        if (currentObject != null)
        {
            originalParent = currentObject.transform.parent;
            currentObject.transform.SetParent(inspectOffset);
            currentObject.transform.localPosition = Vector3.zero;
            currentObject.transform.localRotation = Quaternion.identity;
            isInspecting = true;
            lastMousePosition = Input.mousePosition; // Inisialisasi posisi mouse
        }
    }

    private void StopInspecting()
    {
        if (currentObject != null)
        {
            currentObject.transform.SetParent(originalParent);
            currentObject.transform.localPosition = Vector3.zero; // Mengembalikan ke posisi awal
            currentObject.transform.localRotation = Quaternion.identity; // Reset rotasi
            isInspecting = false;
            currentObject = null;
        }
    }
}
