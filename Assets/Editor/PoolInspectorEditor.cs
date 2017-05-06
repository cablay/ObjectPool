using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PoolInspector))]
public class PoolInspectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PoolInspector piTarget = (PoolInspector)target;

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Original", piTarget.Original, typeof(GameObject), true);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.LabelField("Active Objects", piTarget.ActiveObjects.ToString());
        EditorGUILayout.LabelField("Inactive Objects", piTarget.InactiveObjects.ToString());

        piTarget.HasMaxSize = EditorGUILayout.Toggle("Has Max Size", piTarget.HasMaxSize);

        if(piTarget.HasMaxSize)
        {
            EditorGUI.indentLevel++;
            piTarget.MaxSize = EditorGUILayout.DelayedIntField("Max Size", piTarget.MaxSize);
            Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
            EditorGUI.ProgressBar(rect, (piTarget.InactiveObjects + piTarget.ActiveObjects) / (float)piTarget.MaxSize, "Total Usage");
            EditorGUI.indentLevel--;
        }
        

        piTarget.DestroyOnLoad = EditorGUILayout.Toggle("Destroy On Load", piTarget.DestroyOnLoad);
    }

    public override bool RequiresConstantRepaint()
    {
        return true;
    }
}
