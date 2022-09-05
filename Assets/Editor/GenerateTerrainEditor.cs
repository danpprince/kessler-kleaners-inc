using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateTerrain))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GenerateTerrain terrainGenerator = (GenerateTerrain)target;

        if (GUILayout.Button("Generate terrain"))
        {
            terrainGenerator.GenerateMesh();
            terrainGenerator.ModulateSegments();
        }
    }
}
