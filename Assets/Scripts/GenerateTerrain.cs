using PathCreation;
using SplineMesh;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour
{
    public VertexPath path;
    public Mesh mesh;
    GameObject terrainObject;

    public float width;
    public float thickness;
    public bool flattenSurface;
    public float noiseAmount;
    public Material material;

    public float xAmpModAmount = 1.0f;
    public float xAmpModFrequency = 0.11f;
    public float yAmpModAmount = 20.0f;
    public float yAmpModFrequency = 1.0f;
    public float zAmpModAmount = 1.0f;
    public float zAmpModFrequency = 0.1f;

    private List<GameObject> segmentObjects;

    void Start()
    {
        GenerateMesh();
        ModulateSegments();
    }

    void GenerateMesh()
    {
        terrainObject = new GameObject("Terrain");
        terrainObject.transform.SetParent(gameObject.transform, false);
        terrainObject.tag = "green";

        Spline spline = gameObject.AddComponent<Spline>();

        spline.AddNode(new SplineNode(
            new Vector3(0, 0, 0), new Vector3(0, 0, 25)
        ));
        spline.AddNode(new SplineNode(
            new Vector3(10, 0, 100), new Vector3(90, 0, 100)
        ));
        spline.AddNode(new SplineNode(
            new Vector3(20, -9, 70), new Vector3(9, -9, 70)
        ));

        List<ExtrusionSegment.Vertex> shapeVertices = new List<ExtrusionSegment.Vertex>();
        shapeVertices.Add( 
            new ExtrusionSegment.Vertex(new Vector2(-10, 1), new Vector2(-1, 1), 0)
        );
        shapeVertices.Add( 
            new ExtrusionSegment.Vertex(new Vector2(-9, 0), new Vector2(0.5f, 1), 0)
        );
        shapeVertices.Add( 
            new ExtrusionSegment.Vertex(new Vector2(-4, 0), new Vector2(0.5f, 1), 0)
        );
        shapeVertices.Add( 
            new ExtrusionSegment.Vertex(new Vector2(0, -0.5f), new Vector2(0, 1), 0)
        );
        shapeVertices.Add( 
            new ExtrusionSegment.Vertex(new Vector2(4, 0), new Vector2(0.5f, 1), 0)
        );
        shapeVertices.Add( 
            new ExtrusionSegment.Vertex(new Vector2(9, 0), new Vector2(0.5f, 1), 0)
        );
        shapeVertices.Add(
            new ExtrusionSegment.Vertex(new Vector2(10, 1), new Vector2(1, 1), 0.25f)
        );
        shapeVertices.Add(
            new ExtrusionSegment.Vertex(new Vector2(10, -7), new Vector2(1, -1), 0.5f)
        );
        shapeVertices.Add(
            new ExtrusionSegment.Vertex(new Vector2(-10, -7), new Vector2(-1, -1), 0.75f)
        );

        int i = 0;
        float textureOffset = 0.0f;
        segmentObjects = new List<GameObject>();
        foreach (CubicBezierCurve curve in spline.GetCurves()) {
            GameObject go = UOUtility.Create("segment " + i++,
                terrainObject,
                typeof(MeshFilter),
                typeof(MeshRenderer),
                typeof(ExtrusionSegment),
                typeof(MeshCollider));
            go.GetComponent<MeshRenderer>().material = material;
            ExtrusionSegment seg = go.GetComponent<ExtrusionSegment>();
            seg.ShapeVertices = shapeVertices;
            seg.TextureScale = 1;
            seg.TextureOffset = textureOffset;
            seg.SampleSpacing = 5f;
            seg.SetInterval(curve);
            seg.Compute();

            textureOffset += curve.Length;

            segmentObjects.Add(go);
        }
    }

    void ModulateSegments()
    {
        foreach (GameObject segment in segmentObjects)
        {
            segment.tag = "green";

            // Remove the ExtrusionSegment to prevent vertex changes from being overwritten
            Destroy(segment.GetComponent<ExtrusionSegment>());

            MeshFilter meshFilter = segment.GetComponent<MeshFilter>();
            Mesh mesh = meshFilter.mesh;

            Vector3[] vertices = mesh.vertices;
            
            for (int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++) {
                Vector3 vertex = vertices[vertexIndex];
                vertex.x += xAmpModAmount * Mathf.Sin(vertex.x * xAmpModFrequency);
                vertex.y += yAmpModAmount * Mathf.Sin(vertex.x * yAmpModFrequency);
                vertex.z += zAmpModAmount * Mathf.Sin(vertex.z * zAmpModFrequency);
                vertices[vertexIndex] = vertex;
            }

            mesh.SetVertices(vertices);
            mesh.RecalculateNormals();

            // Update the mesh collider
            MeshCollider meshCollider = segment.GetComponent<MeshCollider>();
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }

    }

}
