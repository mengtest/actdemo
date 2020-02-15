using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AnimatedVegetationSceneManager))]
public class AnimatedVegetationSceneManagerEditor : Editor 
{
    private AnimatedVegetationSceneManager sceneManager = null;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(sceneManager == null)
        {
            sceneManager = target as AnimatedVegetationSceneManager;
        }
        if(sceneManager != null)
        {
            EditorGUI.BeginChangeCheck();
            bool drawGizmos = EditorGUILayout.Toggle("Draw Gizmos", sceneManager.drawGizmos);
            if (EditorGUI.EndChangeCheck())
            {
                sceneManager.drawGizmos = drawGizmos;
                EditorUtility.SetDirty(sceneManager.gameObject);
            }
        }
    }
}
