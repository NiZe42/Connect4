using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(Board))]
public class BoardEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        Board boardGenerator = (Board)target;

        if (GUILayout.Button("Generate Board")) {
            boardGenerator.GenerateBoard();
        }

        if (GUILayout.Button("Delete Generated")) {
            boardGenerator.DeleteGenerated();
        }

        if (GUILayout.Button("Clear Children")) {
            boardGenerator.ClearChildren();
        }
    }
}
#endif