using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Windows.Forms; // Add this namespace for SaveFileDialog

public class EnhancedOBJExporter : MonoBehaviour
{
    public void ExportSceneToOBJ()
    {
        string chosenPath = GetSavePath();
        if (string.IsNullOrEmpty(chosenPath))
        {
            Debug.Log("Export canceled by the user.");
            return;
        }

        // Ensure the file has a .obj extension
        if (!Path.GetExtension(chosenPath).Equals(".obj", System.StringComparison.OrdinalIgnoreCase))
        {
            chosenPath = Path.ChangeExtension(chosenPath, ".obj");
        }

        StringBuilder objBuilder = new StringBuilder();
        StringBuilder mtlBuilder = new StringBuilder();
        string objFilePath = chosenPath;
        string mtlFilePath = Path.ChangeExtension(chosenPath, ".mtl");

        objBuilder.AppendLine("# Exported OBJ File");
        mtlBuilder.AppendLine("# Exported MTL File");

        int vertexOffset = 0; // Tracks vertex index across objects

        foreach (GameObject obj in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (obj.activeInHierarchy)
            {
                vertexOffset += WriteGameObject(objBuilder, mtlBuilder, obj, vertexOffset);
            }
        }

        // Write OBJ and MTL files
        File.WriteAllText(objFilePath, objBuilder.ToString());
        File.WriteAllText(mtlFilePath, mtlBuilder.ToString());

        Debug.Log($"Scene exported to OBJ at {objFilePath}");
        Debug.Log($"Materials exported to MTL at {mtlFilePath}");
    }

    private string GetSavePath()
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Title = "Choose Save Location",
            Filter = "OBJ Files (*.obj)|*.obj|All Files (*.*)|*.*", // Set filter for .obj files
            FileName = "ExportedScene", // Default filename
            DefaultExt = "obj", // Default extension
            InitialDirectory = UnityEngine.Application.dataPath // Use explicit namespace
        };

        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            return saveFileDialog.FileName;
        }

        return null;
    }

    private int WriteGameObject(StringBuilder objBuilder, StringBuilder mtlBuilder, GameObject obj, int vertexOffset)
    {
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        Renderer renderer = obj.GetComponent<Renderer>();
        Mesh mesh = meshFilter != null ? meshFilter.sharedMesh : null;

        if (mesh != null)
        {
            objBuilder.AppendLine($"o {obj.name}");

            // Write vertices
            foreach (Vector3 vertex in mesh.vertices)
            {
                Vector3 worldVertex = obj.transform.TransformPoint(vertex);
                objBuilder.AppendLine($"v {worldVertex.x} {worldVertex.y} {worldVertex.z}");
            }

            // Write normals
            foreach (Vector3 normal in mesh.normals)
            {
                Vector3 worldNormal = obj.transform.TransformDirection(normal).normalized;
                objBuilder.AppendLine($"vn {worldNormal.x} {worldNormal.y} {worldNormal.z}");
            }

            // Write UVs
            foreach (Vector2 uv in mesh.uv)
            {
                objBuilder.AppendLine($"vt {uv.x} {uv.y}");
            }

            // Write material
            if (renderer != null && renderer.sharedMaterial != null)
            {
                string materialName = renderer.sharedMaterial.name;
                objBuilder.AppendLine($"usemtl {materialName}");
                WriteMaterial(mtlBuilder, renderer.sharedMaterial, materialName);
            }

            // Write faces
            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                int v1 = mesh.triangles[i] + 1 + vertexOffset;
                int v2 = mesh.triangles[i + 1] + 1 + vertexOffset;
                int v3 = mesh.triangles[i + 2] + 1 + vertexOffset;
                objBuilder.AppendLine($"f {v1}/{v1}/{v1} {v2}/{v2}/{v2} {v3}/{v3}/{v3}");
            }

            return mesh.vertexCount;
        }

        // Export child objects recursively
        int verticesWritten = 0;
        foreach (Transform child in obj.transform)
        {
            verticesWritten += WriteGameObject(objBuilder, mtlBuilder, child.gameObject, vertexOffset + verticesWritten);
        }

        return verticesWritten; // Ensure a value is returned in all cases
    }

    private void WriteMaterial(StringBuilder mtlBuilder, Material material, string materialName)
    {
        mtlBuilder.AppendLine($"newmtl {materialName}");

        if (material.HasProperty("_Color"))
        {
            Color color = material.color;
            mtlBuilder.AppendLine($"Kd {color.r} {color.g} {color.b}");
        }

        if (material.HasProperty("_MainTex"))
        {
            Texture texture = material.mainTexture;
            if (texture != null)
            {
                string texturePath = null;
#if UNITY_EDITOR
                texturePath = AssetDatabase.GetAssetPath(texture);
#endif
                if (!string.IsNullOrEmpty(texturePath))
                {
                    mtlBuilder.AppendLine($"map_Kd {Path.GetFileName(texturePath)}");
                }
            }
        }

        mtlBuilder.AppendLine(); // Separate materials with a blank line
    }
}