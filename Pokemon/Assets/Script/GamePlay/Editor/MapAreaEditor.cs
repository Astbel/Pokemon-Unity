using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapArea))]

public class MapAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //為了顯示機率而這邊機率是SerializeField地宣告這邊這使用查找並且賦直
        int totalChance = serializedObject.FindProperty("totalChance").intValue;
        //宣告為inspecter 的字體格式目前定義為粗體
        var style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        GUILayout.Label($"Total Chance ={totalChance}", style);
        if (totalChance != 100)
            EditorGUILayout.HelpBox("The total chance percentage is not 100", MessageType.Error);
    }
}
