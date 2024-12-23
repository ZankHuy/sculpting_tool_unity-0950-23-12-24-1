using UnityEngine;
using System.IO;
using SFB; // Namespace for StandaloneFileBrowser

public class FileImporter : MonoBehaviour
{
    public SculptingTool sculptingTool;

    public void Import3DFile()
    {
        // Open the file browser dialog
        var paths = StandaloneFileBrowser.OpenFilePanel("Open 3D File", "", "obj", false);
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            string filePath = paths[0];
            Debug.Log($"Selected file: {filePath}");

            // Load the OBJ file into the scene
            GameObject importedObject = ImportOBJFile(filePath);
            if (importedObject != null)
            {
                Debug.Log("File imported successfully. Preparing for sculpting...");
                PrepareForSculpting(importedObject);
            }
            else
            {
                Debug.LogError("Failed to import 3D file.");
            }
        }
        else
        {
            Debug.Log("File selection canceled or no file selected.");
        }
    }

    private GameObject ImportOBJFile(string filePath)
    {
        return SimpleOBJLoader.Load(filePath);
    }

    private void PrepareForSculpting(GameObject obj)
    {
        // Ensure the object has necessary components
        if (obj.GetComponent<MeshFilter>() == null)
        {
            Debug.LogError("Imported object lacks a MeshFilter.");
            return;
        }

        if (obj.GetComponent<MeshRenderer>() == null)
        {
            obj.AddComponent<MeshRenderer>();
        }

        if (obj.GetComponent<MeshCollider>() == null)
        {
            obj.AddComponent<MeshCollider>();
        }

        // Set the object as the current target in SculptingTool
        sculptingTool.ToggleSculptMode(true);
        sculptingTool.InitializeNewTarget(obj);
        Debug.Log($"Object {obj.name} prepared for sculpting.");
    }
}
