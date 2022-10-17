using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    public float cellSize = 1;
    public Vector3 gridOffset;
    public int gridSize = 10;
    public float borderSize = .1f;


#if UNITY_EDITOR

    [ContextMenu("Generate Mesh")]
    void GenerateMesh()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        MakeProceduralGrid();
        UpdateMesh();
    }


#endif


    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }
    void Start()
    {
        MakeProceduralGrid();
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void MakeProceduralGrid()
    {
        vertices = new Vector3[gridSize * gridSize * 4];
        triangles = new int[gridSize * gridSize * 6];

        int v = 0;
        int t = 0;
        bool r = true;

        float vertexOffset = cellSize * .5f;

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Vector3 cellOffset = new Vector3(i*cellSize, 0, j*cellSize);

                if (r)
                {
                    vertices[v    ] = new Vector3(-(vertexOffset + (vertexOffset * 2 - borderSize)), 0, -borderSize) + cellOffset + gridOffset;
                    vertices[v + 1] = new Vector3(-(vertexOffset + (vertexOffset * 2 - borderSize)), 0,  borderSize) + cellOffset + gridOffset;
                    vertices[v + 2] = new Vector3((vertexOffset + (vertexOffset * 2 - borderSize)), 0, -borderSize) + cellOffset + gridOffset;
                    vertices[v + 3] = new Vector3((vertexOffset + (vertexOffset * 2 - borderSize)), 0,  borderSize) + cellOffset + gridOffset;
                }
                else
                {
                    vertices[v    ] = new Vector3(-borderSize, 0, -(vertexOffset+(vertexOffset * 2 - borderSize))) + cellOffset + gridOffset;
                    vertices[v + 1] = new Vector3(-borderSize, 0, (vertexOffset + (vertexOffset * 2 - borderSize))) + cellOffset + gridOffset;
                    vertices[v + 2] = new Vector3( borderSize, 0, -(vertexOffset + (vertexOffset * 2 - borderSize))) + cellOffset + gridOffset;
                    vertices[v + 3] = new Vector3( borderSize, 0, (vertexOffset + (vertexOffset * 2 - borderSize))) + cellOffset + gridOffset;
                }
                

                triangles[t    ] = v;
                triangles[t + 1] = v + 1;
                triangles[t + 2] = v + 2;
                triangles[t + 3] = v + 2;
                triangles[t + 4] = v + 1;
                triangles[t + 5] = v + 3;

                v += 4;
                t += 6;
                r = !r;
            }
            r = !r;
        }
    }
}
