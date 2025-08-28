using UnityEditor;
using UnityEngine;
using Fungus;
using Fungus.EditorUtils;

[CustomEditor(typeof(TextDialogAnimation))]
public class TextDialogAnimationEditor : CommandEditor
{
    SerializedProperty dialogTypeProp;
    SerializedProperty playerDialogPrefabProp;
    SerializedProperty customDialogPrefabProp;


    SerializedProperty bubblePosition;
    SerializedProperty requireClickToCloseProp;
    SerializedProperty clickTriggerProp;
    SerializedProperty dialogText;
    SerializedProperty ignoreClickLayers;

    SerializedProperty fadeInDuration;
    SerializedProperty fadeOutDuration;
    SerializedProperty scaleInDuration;
    SerializedProperty scaleOutDuration;
    SerializedProperty dialogOffset;
    SerializedProperty customFontSize;
    private void OnEnable()
    {
        dialogTypeProp = serializedObject.FindProperty(nameof(TextDialogAnimation.dialogType));
        playerDialogPrefabProp = serializedObject.FindProperty(nameof(TextDialogAnimation.playerDialogPrefab));
        customDialogPrefabProp = serializedObject.FindProperty(nameof(TextDialogAnimation.customDialogPrefab));

        bubblePosition = serializedObject.FindProperty(nameof(TextDialogAnimation.bubblePosition));
        requireClickToCloseProp = serializedObject.FindProperty(nameof(TextDialogAnimation.requireClickToClose));
        clickTriggerProp = serializedObject.FindProperty(nameof(TextDialogAnimation.clickTrigger));
        dialogText = serializedObject.FindProperty(nameof(TextDialogAnimation.dialogText));
        ignoreClickLayers= serializedObject.FindProperty(nameof(TextDialogAnimation.ignoreClickLayers));
        fadeInDuration = serializedObject.FindProperty(nameof(TextDialogAnimation.fadeInDuration));
        fadeOutDuration = serializedObject.FindProperty(nameof(TextDialogAnimation.fadeOutDuration));
        scaleInDuration = serializedObject.FindProperty(nameof(TextDialogAnimation.scaleInDuration));
        scaleOutDuration = serializedObject.FindProperty(nameof(TextDialogAnimation.scaleOutDuration));
        dialogOffset = serializedObject.FindProperty(nameof(TextDialogAnimation.dialogOffset));
        customFontSize = serializedObject.FindProperty(nameof(TextDialogAnimation.customFontSize));
    }

    public override void DrawCommandGUI()
    {
        serializedObject.Update();

        //对话类型
        EditorGUILayout.PropertyField(dialogTypeProp, new GUIContent("对话类型"));

        var dialogType = (TextDialogAnimation.DialogType)dialogTypeProp.enumValueIndex;

        if (dialogType == TextDialogAnimation.DialogType.PlayerDialog)
        {
            EditorGUILayout.PropertyField(playerDialogPrefabProp, new GUIContent("莉莉娅对话预制体"));
        }
        else if (dialogType == TextDialogAnimation.DialogType.CustomDialog)
        {
            EditorGUILayout.PropertyField(customDialogPrefabProp, new GUIContent("自定义对话预制体"));
            EditorGUILayout.PropertyField(bubblePosition, new GUIContent("文字点位"));
        }
        // 需要点击关闭
        EditorGUILayout.PropertyField(requireClickToCloseProp, new GUIContent("需要点击关闭"));

        if (requireClickToCloseProp.boolValue)
        {
            EditorGUILayout.PropertyField(clickTriggerProp, new GUIContent("点击触发器"));
            EditorGUILayout.PropertyField(ignoreClickLayers, new GUIContent("忽视点击层级"));
        }
        
        // 对话框
        EditorGUILayout.PropertyField(dialogText, new GUIContent("对话框"));

        //淡入淡出
        EditorGUILayout.PropertyField(fadeInDuration, new GUIContent("文字淡入"));
        EditorGUILayout.PropertyField(fadeOutDuration, new GUIContent("文字淡出"));
        EditorGUILayout.PropertyField(scaleInDuration, new GUIContent("对话框淡入"));
        EditorGUILayout.PropertyField(scaleOutDuration, new GUIContent("对话框淡出"));
        //对话框偏移
        EditorGUILayout.PropertyField(dialogOffset, new GUIContent("对话偏移"));
        //字体大小
        EditorGUILayout.PropertyField(customFontSize, new GUIContent("字体大小"));

        // 排除的属性名用 nameof，防止改名出错
        string[] excludedProps = new string[]
        {
            nameof(TextDialogAnimation.customFontSize),
            nameof(TextDialogAnimation.dialogType),
            nameof(TextDialogAnimation.playerDialogPrefab),
            nameof(TextDialogAnimation.customDialogPrefab),
            nameof(TextDialogAnimation.bubblePosition),
            nameof(TextDialogAnimation.requireClickToClose),
            nameof(TextDialogAnimation.clickTrigger),
            nameof(TextDialogAnimation.dialogText),
            nameof(TextDialogAnimation.ignoreClickLayers),
            nameof(TextDialogAnimation.fadeInDuration),
            nameof(TextDialogAnimation.fadeOutDuration),
            nameof(TextDialogAnimation.scaleInDuration),
            nameof(TextDialogAnimation.scaleOutDuration),
            nameof(TextDialogAnimation.dialogOffset),
        };

        DrawPropertiesExcluding(serializedObject, excludedProps);

        serializedObject.ApplyModifiedProperties();
    }
}
