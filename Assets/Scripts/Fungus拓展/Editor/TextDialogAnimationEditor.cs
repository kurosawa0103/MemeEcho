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

        //�Ի�����
        EditorGUILayout.PropertyField(dialogTypeProp, new GUIContent("�Ի�����"));

        var dialogType = (TextDialogAnimation.DialogType)dialogTypeProp.enumValueIndex;

        if (dialogType == TextDialogAnimation.DialogType.PlayerDialog)
        {
            EditorGUILayout.PropertyField(playerDialogPrefabProp, new GUIContent("����櫶Ի�Ԥ����"));
        }
        else if (dialogType == TextDialogAnimation.DialogType.CustomDialog)
        {
            EditorGUILayout.PropertyField(customDialogPrefabProp, new GUIContent("�Զ���Ի�Ԥ����"));
            EditorGUILayout.PropertyField(bubblePosition, new GUIContent("���ֵ�λ"));
        }
        // ��Ҫ����ر�
        EditorGUILayout.PropertyField(requireClickToCloseProp, new GUIContent("��Ҫ����ر�"));

        if (requireClickToCloseProp.boolValue)
        {
            EditorGUILayout.PropertyField(clickTriggerProp, new GUIContent("���������"));
            EditorGUILayout.PropertyField(ignoreClickLayers, new GUIContent("���ӵ���㼶"));
        }
        
        // �Ի���
        EditorGUILayout.PropertyField(dialogText, new GUIContent("�Ի���"));

        //���뵭��
        EditorGUILayout.PropertyField(fadeInDuration, new GUIContent("���ֵ���"));
        EditorGUILayout.PropertyField(fadeOutDuration, new GUIContent("���ֵ���"));
        EditorGUILayout.PropertyField(scaleInDuration, new GUIContent("�Ի�����"));
        EditorGUILayout.PropertyField(scaleOutDuration, new GUIContent("�Ի��򵭳�"));
        //�Ի���ƫ��
        EditorGUILayout.PropertyField(dialogOffset, new GUIContent("�Ի�ƫ��"));
        //�����С
        EditorGUILayout.PropertyField(customFontSize, new GUIContent("�����С"));

        // �ų����������� nameof����ֹ��������
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
