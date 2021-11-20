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
* --Changelog:
*	20.11.21	FM	created
******************************************************************************/

[ExecuteInEditMode]
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField]
    private int width, height;
    [SerializeField]
    private int levelOfDetail;
    [SerializeField]
    private Material planeMaterial;
    [SerializeField]
    private bool activatePerlinNoise;
    [SerializeField]
    [Range(0.1f, 0.5f)]
    private float perlinOffset = 0.35f;
    [SerializeField]
    [Range(1f, 5f)]
    private float perlinMultiplier;

    [SerializeField]
    ColorGenerator colorGenerator;

    private Color[] colormap;
    private Vector3[] vertices;
    private int[] indices;
    private float[,] allNoise;
    private float perlinNoise;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer rend;
    private Texture2D texture;

    public float[,] AllNoise { get => allNoise; set => allNoise = value; }

    private void Awake()
    {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;
        rend = GetComponent<MeshRenderer>();
        rend.material = planeMaterial;
    }
    // Start is called before the first frame update
    void Start()
    {
        AssignMesh();
        AssignColor();
        UpdateMesh();
    }

    private void Update()
    {
        AssignMesh();
        UpdateMesh();
    }

    public IEnumerator CreateMesh()
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

    private void AssignMesh()
    {
        //BSP: width = 2, height = 2 -> 4 vertices -> 2 Triangles -> 6 indices
        vertices = new Vector3[width * height];
        AllNoise = new float[width, height];
        int index = 0;
        int offset = 0;
        int vertex = 0;
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
        for (int j = 0; j < height - 1; j++)
        {
            //creating one row of quads
            for (int i = 0; i < width - 1; i++)
            {
                vertex = j * height + i;

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
        Debug.Log($"Vertices: {vertices.Length} | Triangles: {indices.Length}");
    }

    private float SetPerlinNoise(int _zPos, int _xPos)
    {
        return Mathf.PerlinNoise(_zPos * perlinOffset, _xPos * perlinOffset) * perlinMultiplier;
    }

    private void AssignColor()
    {
        colormap = colorGenerator.UpdateColor(width, height);
        texture = colorGenerator.UpdateTexture(colormap, width, height);
        rend.sharedMaterial.mainTexture = texture;
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = indices;

        mesh.RecalculateNormals();
    }

}
