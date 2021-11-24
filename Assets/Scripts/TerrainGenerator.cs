using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*****************************************************************************
* Project: TerrainEditor
* File   : TerrainGenerator.cs
* Date   : 20.11.2021
* Author : Franz Mörike (FM)
*
* These coded instructions, statements, and computer programs contain
* proprietary information of the author and are protected by Federal
* copyright law. They may not be disclosed to third parties or copied
* or duplicated in any form, in whole or in part, without the prior
* written consent of the author.
*
* ChangeLog
* ----------------------------
*	20.11.21	created
*	23.11.21    added comments
*	24.11.21    Bug: only width = height supported (seems to be problem with colormap)
******************************************************************************/

[ExecuteInEditMode]
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField]
    private int width, height;
    [SerializeField]
    private int levelOfDetail;
    [SerializeField]
    private Material meshMaterial;
    [SerializeField]
    private bool activatePerlinNoise;
    [SerializeField]
    [Range(0.1f, 0.5f)]
    private float perlinOffset = 0.35f;
    [SerializeField]
    [Range(1f, 5f)]
    private float perlinMultiplier;
    [SerializeField]
    private GameObject planeForTextureDebug;
    [Tooltip("Click here to apply changes when high height / width")]
    [SerializeField]
    private bool updateMesh;
    [SerializeField]
    ColorGenerator colorGenerator;

    private Color[] colormap;
    private Vector3[] vertices;
    private int[] indices;
    private float[,] allNoise;
    private Vector2[] uvMap;

    private float perlinNoise;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer rend;
    private Texture2D texture;

    public float[,] AllNoise { get => allNoise; set => allNoise = value; }

    private void Awake()
    {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter  >();
        rend = GetComponent<MeshRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        AssignMesh();
        AssignColor();
        DrawDebugPlaneTexture();
        UpdateMesh();
    }

    private void Update()
    {
        if (width*height < 500)
        {
            AssignMesh();
            AssignColor();
            UpdateMesh();
        }else if (updateMesh)
        {
            AssignMesh();
            AssignColor();
            UpdateMesh();
            updateMesh = false;
        }
    }

    public IEnumerator CreateMeshProcedural()
    {
        vertices = new Vector3[(width + 1) * (height + 1)]; //9
        int index = 0;
        int offset = 0;
        int vertex = 0;
        indices = new int[(width + 1) * (height + 1) * 6];  //54
        //Calculate vertices
        for (int i = 0; i < height + 1; i++)
        {
            for (int j = 0; j < width + 1; j++)
            {
                vertices[index] = new Vector3(j, 0, i);
                index++;
            }
        }
        //creating plane mesh
        for (int j = 0; j < height + 1; j++)
        {
            //creating one row of quads
            for (int i = 0; i < width; i++)
            {
                //creating quads
                //creating first triangle
                indices[offset + 0] = vertex + 0;
                indices[offset + 1] = vertex + 1 + width;
                indices[offset + 2] = vertex + 1;
                //creating second triangle
                indices[offset + 3] = vertex + 0;
                indices[offset + 4] = vertex + 0 + width;
                indices[offset + 5] = vertex + 1 + width;
                vertex++;
                offset += 6;
                yield return new WaitForSeconds(0.1f);
            }
        }
        Debug.Log($"Vertices: {vertices.Length} | Triangles: {indices.Length}");
    }

    /// <summary>
    /// Creates a new mesh by assigning vertices, creating triangles and then a mesh
    /// Also uses noise for y coordinates
    /// </summary>
    private void AssignMesh()
    {

        //BSP: width = 2, height = 2 -> 4 vertices -> 2 Triangles -> 6 indices
        vertices = new Vector3[width * height];
        AllNoise = new float[width, height];
        int index = 0;
        int offset = 0;
        int vertex = 0;
        Quaternion rotationOffset = Quaternion.Euler(0, 0, 0);
        indices = new int[(width - 1) * (height - 1) * 6];
        //Calculate vertices
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (activatePerlinNoise)
                {
                    perlinNoise = SetPerlinNoise(j, i);
                    AllNoise[j, i] = perlinNoise;
                }
                vertices[index] = new Vector3(j, perlinNoise, i);
                index++;
            }
        }
        //creating plane mesh
        for (int i = 0; i < height - 1; i++)
        {
            //creating one row of quads
            for (int j = 0; j < width - 1; j++)
            {
                vertex = i * height + j;

                //creating quads
                //creating first triangle
                indices[offset + 0] = vertex + 0;
                indices[offset + 1] = vertex + 1 + width;
                indices[offset + 2] = vertex + 1;
                //creating second triangle
                indices[offset + 3] = vertex + 0;
                indices[offset + 4] = vertex + 0 + width;
                indices[offset + 5] = vertex + 1 + width;
                //increment to fill indices accordingly
                offset += 6;
            }
        }
        //Assign uvmap
        int uvIndex = 0;
        uvMap = new Vector2[vertices.Length];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                uvMap[uvIndex] = rotationOffset * (new Vector2(i / (float)width, j / (float)height));
                uvIndex++;
            }
        }

        Debug.Log($"Vertices: {vertices.Length} | Triangles: {indices.Length}");
    }

    /// <summary>
    /// Sets the perlin noise at a desired position
    /// </summary>
    /// <param name="_zPos">zPos where perlin is supposed to be applied</param>
    /// <param name="_xPos">xPos where perlin is supposed to be applied</param>
    /// <returns></returns>
    private float SetPerlinNoise(int _zPos, int _xPos)
    {
        return Mathf.PerlinNoise(_zPos * perlinOffset, _xPos * perlinOffset) * perlinMultiplier;
    }

    private void AssignColor()
    {
        colormap = colorGenerator.UpdateColor(width, height);
        texture = colorGenerator.UpdateTexture(colormap, width, height);
    }

    private void DrawDebugPlaneTexture()
    {
        Renderer planeRenderer = planeForTextureDebug.GetComponent<MeshRenderer>();
        planeRenderer.sharedMaterial.mainTexture = texture;
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        
        mesh.uv = uvMap;

        meshFilter.sharedMesh = mesh;
        rend.material = meshMaterial;
        rend.sharedMaterial.mainTexture = texture;
        mesh.RecalculateNormals();
    }

}
