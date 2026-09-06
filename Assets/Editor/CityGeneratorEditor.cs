using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RandomCityGenerator))]
public class CityGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space(10);
        RandomCityGenerator generator = (RandomCityGenerator)target;

        if (GUILayout.Button("Generate City"))
        {
            generator.GenerateCity();
            EditorUtility.SetDirty(generator);
        }

        if (GUILayout.Button("Delete City"))
        {
            generator.ClearCity();
            EditorUtility.SetDirty(generator);
        }
    }
}
