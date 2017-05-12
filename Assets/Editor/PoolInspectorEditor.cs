using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PoolInspector))]
public class PoolInspectorEditor : Editor
{
    GameObject original;
    bool showMaxSize;
    int prewarmAmount;
    

    void OnEnable()
    {
        PoolInspector poolInspectorTarget = (PoolInspector)target;
        original = poolInspectorTarget.original;
        prewarmAmount = 1;
    }

    int InactiveObjects
    {
        get { return ObjectPool.GetProperty(original, ObjectPool.PoolProperties.InactiveObjects) ?? 0; }
    }

    int ActiveObjects
    {
        get { return ObjectPool.GetProperty(original, ObjectPool.PoolProperties.ActiveObjects) ?? 0; }
    }

    bool DestroyOnLoad
    {
        get { return ObjectPool.GetProperty(original, ObjectPool.PoolProperties.DestroyOnLoad) != 0; }
        set
        {
            if (value != (ObjectPool.GetProperty(original, ObjectPool.PoolProperties.DestroyOnLoad) != 0))
            {
                if (value) { ObjectPool.DoDestroyOnLoad(original); }
                else { ObjectPool.DontDestroyOnLoad(original); }
            }
        }
    }

    int MaxSize
    {
        get { return ObjectPool.GetProperty(original, ObjectPool.PoolProperties.MaxSize) ?? 0; }
        set
        {
            if (value > 0 && value != (ObjectPool.GetProperty(original, ObjectPool.PoolProperties.MaxSize) ?? 0))
            {
                ObjectPool.LimitPoolSize(original, value);
            }
        }
    }

    void DespawnPool()
    {
        ObjectPool.DespawnPool(original);
    }

    void DestroyPool()
    {
        ObjectPool.DestroyPool(original);
    }

    void Prewarm(int numObjs)
    {
        ObjectPool.PreWarm(original, numObjs);
    }

    public override void OnInspectorGUI()
    {

        //Pool Original Object Field - Unselectable
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Original", original, typeof(GameObject), true);
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(5f);

        //Do/Dont DestroyOnLoad
        DestroyOnLoad = EditorGUILayout.Toggle("Destroy On Load", DestroyOnLoad);

        GUILayout.Space(5f);

        //Active and Inactive Objects Count
        GUILayout.BeginVertical("box", GUILayout.MaxWidth(300f));
        EditorGUILayout.LabelField("Active Objects", ActiveObjects.ToString());
        EditorGUILayout.LabelField("Inactive Objects", InactiveObjects.ToString());
        GUILayout.EndVertical();

        GUILayout.Space(5f);

        //Max Size and Usage Foldout
        showMaxSize = EditorGUILayout.Foldout(showMaxSize, "Size and Usage");
        if(showMaxSize)
        {
            GUILayout.BeginVertical("box");
            MaxSize = EditorGUILayout.DelayedIntField("Max Size", MaxSize, GUILayout.MinWidth(120f));
            Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
            EditorGUI.ProgressBar(rect, (InactiveObjects + ActiveObjects) / (float)MaxSize, "Total Usage");
            GUILayout.EndVertical();
        }

        GUILayout.Space(5f);

        //Prewarm Control
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Prewarm", GUILayout.MaxWidth(150f)))
        {
            Prewarm(prewarmAmount);
        }
        prewarmAmount = EditorGUILayout.IntField(prewarmAmount, GUILayout.MaxWidth(Screen.width / 6f), GUILayout.MinWidth(100f));
        GUILayout.EndHorizontal();

        GUILayout.Space(20f);

        //Despawn Pool Button
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Despawn Pool", GUILayout.MaxWidth(150f)))
        {
            DespawnPool();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(20f);

        //Destroy Pool Button
        GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
        if (GUILayout.Button("Destroy Pool"))
        {
            DestroyPool();
        }
        GUI.backgroundColor = Color.white;
    }

    public override bool RequiresConstantRepaint()
    {
        return true;
    }
}
