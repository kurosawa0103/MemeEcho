#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using I2.Loc; // 添加 I2 本地化命名空间

public partial class GmWindow
{
    // ---------- 字段 ----------
    [TabGroup("页签", "杂项页签")] public float speed = 5f;
    private bool isSpeedUp = false;

    // 多语言相关字段
    [TabGroup("页签", "杂项页签"), LabelText("选择语言")]
    [ValueDropdown(nameof(GetAvailableLanguages))]
    public string selectedLanguage = "Chinese";

    // ---------- 功能方法 ----------
    public void SetGameSpeed(float value)
    {
        Time.timeScale = value;
        Debug.Log($"游戏速度已设置为 {value} 倍");
    }

    // 获取所有语言选项
    private List<string> GetAvailableLanguages()
    {
        if (LocalizationManager.Sources.Count > 0)
        {
            var languages = LocalizationManager.Sources[0].GetLanguages();
            List<string> names = new List<string>();
            foreach (var lang in languages)
            {
                names.Add(lang);
            }
            return names;
        }

        // 默认列表
        return new List<string> { "Chinese", "English", "Japanese" };
    }

    // 切换语言按钮
    [TabGroup("页签", "杂项页签"), Button("切换语言", ButtonSizes.Large)]
    private void SwitchLanguage()
    {
        if (!string.IsNullOrEmpty(selectedLanguage))
        {
            LocalizationManager.CurrentLanguage = selectedLanguage;
            Debug.Log($"语言切换为：{selectedLanguage}");
        }
        else
        {
            Debug.LogWarning("未选择语言");
        }
    }

    // ---------- UI ----------
    [TabGroup("页签", "杂项页签"), Button("加速游戏", ButtonSizes.Large)]
    private void SpeedUpGame()
    {
        if (isSpeedUp)
        {
            SetGameSpeed(1f); // 恢复正常速度
        }
        else
        {
            SetGameSpeed(speed); // 使用自定义速度
        }
        isSpeedUp = !isSpeedUp;
    }
}
#endif
