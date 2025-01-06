using UnityEngine;

public class MaterialApplier : MonoBehaviour
{
    public Camera mainCamera;          // Reference to the main camera
    public Material selectedMaterial;  // Material to use as a base for new materials

    private GameObject selectedObject; // The currently selected object
    private bool isSnapshotMode = false; // Whether snapshot mode is active

    void Update()
    {
        // Handle object selection when snapshot mode is OFF
        if (!isSnapshotMode && Input.GetMouseButtonDown(0))
        {
            SelectObject();
        }
    }

    private void SelectObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            selectedObject = hit.collider.gameObject;
            Debug.Log($"Selected object: {selectedObject.name}");
        }
        else
        {
            Debug.LogWarning("No object hit by the ray.");
        }
    }

    public void ToggleSnapshotMode()
    {
        isSnapshotMode = !isSnapshotMode;

        if (isSnapshotMode)
        {
            Debug.Log("Snapshot Paint Mode Enabled");
        }
        else
        {
            Debug.Log("Snapshot Paint Mode Disabled");
        }
    }

    public void SnapshotPaint()
    {
        if (selectedObject != null)
        {
            Renderer objectRenderer = selectedObject.GetComponent<Renderer>();
            if (objectRenderer != null)
            {
                // Create a new material instance based on the selected material
                Material newMaterial = new Material(selectedMaterial);

                // Apply the new material to the object's renderer
                objectRenderer.material = newMaterial;

                Debug.Log($"Created new material for {selectedObject.name} and applied it.");
            }
            else
            {
                Debug.LogWarning("Selected object has no Renderer component.");
            }
        }
        else
        {
            Debug.LogWarning("No object selected to paint.");
        }
    }

    public void SetSelectedMaterial(Material material)
    {
        selectedMaterial = material;
        Debug.Log($"Selected material changed to: {material.name}");
    }
}
