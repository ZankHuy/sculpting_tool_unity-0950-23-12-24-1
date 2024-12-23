using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SimpleOBJLoader
{
    public static GameObject Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"File not found: {filePath}");
            return null;
        }

        var objData = File.ReadAllLines(filePath);
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uvs = new List<Vector2>();

        GameObject obj = new GameObject(Path.GetFileNameWithoutExtension(filePath));
        Mesh mesh = new Mesh();

        foreach (var line in objData)
        {
            string[] split = line.Split(' ');

            if (split[0] == "v") // Vertex position
            {
                vertices.Add(new Vector3(
                    float.Parse(split[1]),
                    float.Parse(split[2]),
                    float.Parse(split[3])
                ));
            }
            else if (split[0] == "vt") // Vertex texture (UV)
            {
                uvs.Add(new Vector2(
                    float.Parse(split[1]),
                    float.Parse(split[2])
                ));
            }
            else if (split[0] == "f") // Face
            {
                for (int i = 1; i < split.Length; i++)
                {
                    var faceData = split[i].Split('/');
                    int vertexIndex = int.Parse(faceData[0]) - 1;
                    triangles.Add(vertexIndex);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        if (uvs.Count > 0)
        {
            mesh.uv = uvs.ToArray();
        }

        mesh.RecalculateNormals();

        MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));

        MeshCollider collider = obj.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;

        return obj;
    }
}
