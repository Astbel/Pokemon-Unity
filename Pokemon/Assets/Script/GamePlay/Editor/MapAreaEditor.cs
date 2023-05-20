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
        int totalChanceInGrass = serializedObject.FindProperty("totalChance").intValue;
        int totalChanceInWater = serializedObject.FindProperty("totalChance_Water").intValue;
        //宣告為inspecter 的字體格式目前定義為粗體

        if (totalChanceInGrass != 100 && totalChanceInGrass != -1)
            EditorGUILayout.HelpBox($"The total chance percentage in grass is {totalChanceInGrass} and not 100", MessageType.Error);
        if (totalChanceInWater != 100 && totalChanceInWater != -1)
            EditorGUILayout.HelpBox($"The total chance percentage in water is {totalChanceInWater} and not 100", MessageType.Error);
    }
}
