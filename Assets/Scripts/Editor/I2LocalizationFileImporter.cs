using UnityEngine;
using UnityEditor;
using System.IO;
using I2.Loc;

public static class I2LocalizationImporter
{
    [MenuItem("����/һ����������� CSV")]
    public static void ImportCSVToI2Languages()
    {
        string relativeCsvPath = "Assets/Resources/Language/Language.csv";
        string fullPath = Path.Combine(Application.dataPath, "../" + relativeCsvPath);

        if (!File.Exists(fullPath))
        {
            Debug.LogError(" �Ҳ��� CSV �ļ���" + fullPath);
            return;
        }

        try
        {
            string csvContent = File.ReadAllText(fullPath);

            // ���� I2Languages��LanguageSourceAsset��
            var sourceAsset = Resources.Load<LanguageSourceAsset>("I2Languages");
            if (sourceAsset == null)
            {
                Debug.LogError(" �޷��ҵ� Resources/I2Languages.asset������ӦΪ LanguageSourceAsset��");
                return;
            }

            if (sourceAsset.mSource == null)
            {
                Debug.LogError(" sourceAsset.mSource Ϊ�գ��޷����� CSV");
                return;
            }

            // ֱ���滻����
            sourceAsset.mSource.Import_CSV("", csvContent, eSpreadsheetUpdateMode.Replace);
            EditorUtility.SetDirty(sourceAsset);
            AssetDatabase.SaveAssets();

            Debug.Log("�ɹ����� CSV �ļ����ݵ� Resources/I2Languages��");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("�������: " + ex.Message);
        }
    }
}
