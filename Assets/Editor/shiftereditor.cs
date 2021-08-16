using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(worldinfoshifter))]
public class shiftereditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        worldinfoshifter myScript = (worldinfoshifter)target;
        if (GUILayout.Button("Build Object"))
        {
            myScript.ShiftValues();
        }
    }
}
