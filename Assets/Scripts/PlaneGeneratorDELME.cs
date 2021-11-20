using UnityEngine;

/*****************************************************************************
* Project: TerrainEditor
* File   : MeshGenerator.cs
* Date   : 11.11.2021
* Author : René Kraus (RK)
*
* These coded instructions, statements, and computer programs contain
* proprietary information of the author and are protected by Federal
* copyright law. They may not be disclosed to third parties or copied
* or duplicated in any form, in whole or in part, without the prior
* written consent of the author.
*
* History:
*	11.11.21	RK	Created
******************************************************************************/

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PlaneGenerator : MonoBehaviour
{
    private Mesh customMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private Vector3[] vertices;
    private int[] triangles;

    [SerializeField, Range(2, 255)]
    private int m_resolution = 64;

    [Header("Perlin Noise Propertys")]
    [SerializeField]
    private bool m_usePerlinNoise = false;
    [SerializeField, Range(0.0f, 1.0f)]
    private float m_xValue = 0.3f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float m_yValue = 0.25f;
    [SerializeField]
    private float m_offset = 3f;

    void Start()
    { 
        Generate();
    }

    public void Generate()
    {
        Initialize();
        CreateMesh();
        UpdateMesh();
    }


    private void Initialize()
    {
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();

        customMesh = new Mesh();
        meshFilter.sharedMesh = customMesh;
    }

    private void CreateMesh()
    {
        Vector3 startPos = new Vector3(1f, 0f, 1f) * -0.5f;

        vertices = new Vector3[m_resolution * m_resolution];
        triangles = new int[(m_resolution - 1) * (m_resolution - 1) * 6];


        int vert = 0;
        int tris = 0;
       
        for (int z = 0; z < m_resolution; z++)
        {
            for (int x = 0; x < m_resolution; x++)
            {
                Vector2 percent = new Vector2(x, z) / (m_resolution - 1);

                vert = z * m_resolution + x;

                vertices[vert] = startPos + Vector3.right * percent.x + Vector3.forward * percent.y;

                if (m_usePerlinNoise)
                {
                    float y = Mathf.PerlinNoise(x * m_xValue, z * m_yValue) * m_offset;
                    vertices[vert] = new Vector3(x, y, z);
                }
                else
                {
                    vertices[vert] = new Vector3(x, 0, z);
                }

                if (x < m_resolution - 1 && z < m_resolution - 1)
                {
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + m_resolution + 1;
                    triangles[tris + 2] = vert + 1;

                    triangles[tris + 3] = vert;
                    triangles[tris + 4] = vert + m_resolution;
                    triangles[tris + 5] = vert + m_resolution + 1;

                    //vert++;
                    tris += 6;
                }      
            }
        }
        Debug.Log($"Vertices: {vertices.Length} | Triangles: {triangles.Length}");

    }

    public void UpdateMesh()
    {
        customMesh.Clear();

        customMesh.vertices = vertices;
        customMesh.triangles = triangles;
        customMesh.RecalculateNormals();
        customMesh.RecalculateBounds();
        customMesh.Optimize();
    }

}
