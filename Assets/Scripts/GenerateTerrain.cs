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

    void Start()
    {
        GenerateMesh();
    }

    void CreateMeshObject()
    {

        MeshRenderer meshRenderer = terrainObject.AddComponent<MeshRenderer> ();
        MeshFilter meshFilter = terrainObject.AddComponent<MeshFilter> ();
        if (mesh == null) {
            mesh = new Mesh();
            mesh.name = "Terrain mesh";
        }
        meshFilter.sharedMesh = mesh;
    }

    void CreatePath()
    {
        List<Vector3> points = new List<Vector3>(){
            new Vector3(0, 0, 0),
            new Vector3(30, 0, 100),
            new Vector3(-30, 0, 200)
        };

        path = new VertexPath(new BezierPath(points), gameObject.transform);

        print("Num points in path: " + path.NumPoints);
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
            new ExtrusionSegment.Vertex(new Vector2(0, -0.5f), new Vector2(0, 1), 0)
        );
        shapeVertices.Add( 
            new ExtrusionSegment.Vertex(new Vector2(9, 0), new Vector2(0.5f, 1), 0)
        );
        shapeVertices.Add(
            new ExtrusionSegment.Vertex(new Vector2(10, 1), new Vector2(1, 1), 0.25f)
        );
        shapeVertices.Add(
            new ExtrusionSegment.Vertex(new Vector2(10, -1), new Vector2(1, -1), 0.5f)
        );
        shapeVertices.Add(
            new ExtrusionSegment.Vertex(new Vector2(-10, -1), new Vector2(-1, -1), 0.75f)
        );

        int i = 0;
        float textureOffset = 0.0f;
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

            textureOffset += curve.Length;
        }
    }


}
