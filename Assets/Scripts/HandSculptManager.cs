using UnityEngine;

public class HandSculptManager : MonoBehaviour
{
    [Header("Hand Tracking")]
    public Transform handTransform;  // e.g., 

    [Header("Primary Sculptor Reference")]
    public MeshSculptor sculptorPrimary;  // reference your first subdivided mesh

    [Header("Secondary Sculptor Reference")]
    public MeshSculptor sculptorSecondary; // reference your second mesh

    [Header("Sculpting Toggles")]
    public bool isSculptingPrimary = false;
    public bool isSculptingSecondary = false;

    private void Update()
    {
        if (handTransform == null) return;

        Vector3 handPos = handTransform.position;

        // Sculpt on the first mesh
        if (isSculptingPrimary && sculptorPrimary != null)
        {
            sculptorPrimary.SculptAtPoint(handPos);
        }

        // Sculpt on the second mesh
        if (isSculptingSecondary && sculptorSecondary != null)
        {
            sculptorSecondary.SculptAtPoint(handPos);
        }
    }

    // --- Buttons for the FIRST sculptable object ---
    public void StartSculptingPrimary()
    {
        isSculptingPrimary = true;
        Debug.Log("Sculpting on Primary Object Started");
    }

    public void StopSculptingPrimary()
    {
        isSculptingPrimary = false;
        Debug.Log("Sculpting on Primary Object Stopped");
    }

    // --- Buttons for the SECOND sculptable object ---
    public void StartSculptingSecondary()
    {
        isSculptingSecondary = true;
        Debug.Log("Sculpting on Secondary Object Started");
    }

    public void StopSculptingSecondary()
    {
        isSculptingSecondary = false;
        Debug.Log("Sculpting on Secondary Object Stopped");
    }

    // -------------------------------------------------
    // Brush Mode Changes:
    // We can apply the same brush mode to both sculptors 
    // or only to the one that’s relevant:
    // -------------------------------------------------

    public void SetPushMode()
    {
        if (sculptorPrimary != null) sculptorPrimary.brushMode = MeshSculptor.BrushMode.Push;
        if (sculptorSecondary != null) sculptorSecondary.brushMode = MeshSculptor.BrushMode.Push;
        Debug.Log("Brush Mode: PUSH");
    }

    public void SetPullMode()
    {
        if (sculptorPrimary != null) sculptorPrimary.brushMode = MeshSculptor.BrushMode.Pull;
        if (sculptorSecondary != null) sculptorSecondary.brushMode = MeshSculptor.BrushMode.Pull;
        Debug.Log("Brush Mode: PULL");
    }

    public void SetSmoothMode()
    {
        if (sculptorPrimary != null) sculptorPrimary.brushMode = MeshSculptor.BrushMode.Smooth;
        if (sculptorSecondary != null) sculptorSecondary.brushMode = MeshSculptor.BrushMode.Smooth;
        Debug.Log("Brush Mode: SMOOTH");
    }
}