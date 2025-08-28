using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class BuildInfoPostProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        var data = new BuildReportData
        {
            projectName = Application.productName,
            buildTarget = report.summary.platform.ToString(),
            buildTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            totalSize = (report.summary.totalSize / (1024f * 1024f)).ToString("F2") + " MB",
            outputFiles = new List<OutputFileInfo>(),
            assets = new List<AssetInfo>(),
            scenes = new List<string>()
        };

        // ��ȡ���õĳ���
        data.scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToList();

        // ����ļ�
        foreach (var step in report.steps)
        {
            foreach (var msg in step.messages)
            {
                if (msg.type == LogType.Log && msg.content.Contains("output"))
                {
                    var outputPath = ExtractFilePathFromMessage(msg.content);
                    if (!string.IsNullOrEmpty(outputPath))
                    {
                        var fileInfo = new FileInfo(outputPath);
                        if (fileInfo.Exists)
                        {
                            data.outputFiles.Add(new OutputFileInfo
                            {
                                path = fileInfo.FullName,
                                size = (fileInfo.Length / (1024f * 1024f)).ToString("F2") + " MB"
                            });
                        }
                    }
                }
            }
        }

        // ��Դ�б�
        foreach (var packedAsset in report.packedAssets)
        {
            foreach (var content in packedAsset.contents)
            {
                if (!string.IsNullOrEmpty(content.sourceAssetPath))
                {
                    var fileInfo = new FileInfo(content.sourceAssetPath);
                    long sizeInBytes = fileInfo.Exists ? fileInfo.Length : 0;

                    data.assets.Add(new AssetInfo
                    {
                        path = content.sourceAssetPath,
                        type = content.type.ToString(),
                        size = (sizeInBytes / 1024f).ToString("F1") + " KB"
                    });
                }
            }
        }

        // ��� asset �б�Ϊ�գ��� AssetDatabase ��ȡ������Դ·��
        if (data.assets.Count == 0)
        {
            var allAssetGuids = AssetDatabase.FindAssets("", new[] { "Assets" });
            foreach (var guid in allAssetGuids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var fileInfo = new FileInfo(assetPath);
                long sizeInBytes = fileInfo.Exists ? fileInfo.Length : 0;

                data.assets.Add(new AssetInfo
                {
                    path = assetPath,
                    type = "Asset",
                    size = (sizeInBytes / 1024f).ToString("F1") + " KB"
                });
            }
        }

        // �������ݣ�д����ʱ�ļ���
        var json = JsonUtility.ToJson(data, true);
        var path = "BuildReports/LastBuildReport.json";
        Directory.CreateDirectory("BuildReports");
        File.WriteAllText(path, json);

        Debug.Log("Build report saved to: " + path);
    }

    private string ExtractFilePathFromMessage(string message)
    {
        var startIndex = message.IndexOf("output") + 7;
        var endIndex = message.LastIndexOf(" ");
        if (startIndex >= 0 && endIndex > startIndex)
        {
            return message.Substring(startIndex, endIndex - startIndex).Trim();
        }
        return string.Empty;
    }
}
