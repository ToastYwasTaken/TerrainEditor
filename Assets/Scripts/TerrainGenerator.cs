using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField]
    private int width, height;
    [SerializeField]
    private int levelOfDetail;
    [SerializeField]
    private Material planeMaterial;

    private Vector3[] vertices;
    private int[] indices;
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer rend;

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
        //AssignMesh();
        StartCoroutine(CreateMesh());
        UpdateMesh();
    }
    private void Update()
    {
        UpdateMesh();
    }

    public IEnumerator CreateMesh()
    {
        vertices = new Vector3[(width + 1) * (height + 1)];
        int index = 0;
        int offset = 0;
        int vertex = 0;
        indices = new int[(width+1) * (height+1) * 6];
        //9
        for (int i = 0; i < height+1; i++)
        {
            for (int j = 0; j < width+1; j++)
            {
                vertices[index] = new Vector3(j, 0, i);
                index++;
            }
        }

        //creating plane mesh
        for (int j = 0; j < height+1; j++)
        {
            //creating one row of quads
            for (int i = 0; i < width+1; i++)
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
        vertices = new Vector3[(width + 1) * (height + 1)];
        int index = 0;
        int quadOffset = 0;
        int vertex = 0;
        indices = new int[width * height * 6];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                vertices[index] = new Vector3(j, 0, i);
                index++;
            }
        }

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                indices[quadOffset + 0] = vertex + 0; //0
                indices[quadOffset + 1] = vertex + 1 + width; //2
                indices[quadOffset + 2] = vertex + 1; //1
                indices[quadOffset + 3] = vertex + 0; //0
                indices[quadOffset + 4] = vertex + 0 + width; //1
                indices[quadOffset + 5] = vertex + 1 + width; //2
                vertex++;
                quadOffset += 6;
            }
            vertex++;
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = indices;

        mesh.RecalculateNormals();
    }

}
