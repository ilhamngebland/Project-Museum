using UnityEngine;
using UnityEngine.UI;

public class ObjectInspector : MonoBehaviour
{
    public Camera playerCamera;
    public Transform inspectOffset; // Posisi offset untuk objek yang di-inspect
    public float raycastDistance = 3f;
    public LayerMask inspectableLayer;
    public Text inspectUI;

    private GameObject currentObject = null;
    private bool isInspecting = false;

    void Update()
    {
        if (isInspecting)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                EndInspect();
            }
            return;
        }

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, inspectableLayer))
        {
            if (hit.collider.CompareTag("Object"))
            {
                currentObject = hit.collider.gameObject;
                inspectUI.text = "Press E to Inspect";

                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartInspect(currentObject);
                }
            }
        }
        else
        {
            inspectUI.text = "";
            currentObject = null;
        }
    }

    void StartInspect(GameObject obj)
    {
        isInspecting = true;
        inspectUI.text = "Press E to Exit";
        obj.transform.SetParent(inspectOffset);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    void EndInspect()
    {
        isInspecting = false;
        inspectUI.text = "";

        if (currentObject != null)
        {
            currentObject.transform.SetParent(null);
            Rigidbody rb = currentObject.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;

            currentObject = null;
        }
    }
}
