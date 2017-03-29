using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager gm = (GameManager)target;

        if (GUILayout.Button("Create Grid"))
        {
            gm.createGrid();
            //hasGrid = true;
        }
        if (GUILayout.Button("Remove Grid"))
        {
            gm.removeGrid();
            //hasGrid = false;
        }
    }
}
