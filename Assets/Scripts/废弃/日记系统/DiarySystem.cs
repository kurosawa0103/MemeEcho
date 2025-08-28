using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class DiarySystem : MonoBehaviour
{
    private string diaryFilePath; // 日记文件路径
    private TimeSystem timeSystem;

    public TextMeshProUGUI diaryText;
    public TextMeshProUGUI dayText;
    public RectTransform textTransform;
    public EventManager eventManager;
    private IEventHandler eventHandler = new EventToJson();

    public int currentDay; // 当前查看的日期
    private int totalDays;  // 总共有多少天的日记

    public GameObject previousPageButton; // 上一页按钮
    public GameObject nextPageButton; // 下一页按钮

    public List<int> diaryDays = new List<int>();  // 用于存储日期的列表

    // 设置路径，可以根据玩家存档来生成不同的路径
    private void Awake()
    {
        timeSystem = GameObject.FindGameObjectWithTag("Mgr").GetComponent<TimeSystem>();
        diaryFilePath = Application.persistentDataPath + "/Diary.txt"; // 持久化存储路径
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(textTransform);
        }
    }
    // 触发事件时记录日记
    public void WriteDiary()
    {
        //string currentDayEntry = $"Day {timeSystem.dayCount}";  // 当前是第几天
        //string diaryEntry = $"日期：{currentDayEntry}@@newline@@";  // 使用自定义换行符
        //string diaryContent = diaryEntry;

        //Debug.Log($"[WriteDiary] 开始记录日记，当前天数：{timeSystem.dayCount}");

        // 1. 获取当前事件
        EventData currentEvent = eventManager.currentEvent;

        if (currentEvent == null)
        {
            Debug.LogWarning("[WriteDiary] 当前没有事件发生，无法记录日记！");
            return; // 避免写入空内容
        }

        //Debug.Log($"[WriteDiary] 当前事件名称：{currentEvent.eventName}, 事件类型：{currentEvent.eventType}, 可用时间段：{string.Join(", ", currentEvent.availableTimes)}");

       

        bool hasWrittenEntry = false;

       
        if (!hasWrittenEntry)
        {
            Debug.LogWarning($"[WriteDiary] 没有满足条件的日记内容，跳过写入。");
            return;
        }

        // 3. 如果文件不存在，直接写入新日记
        if (!File.Exists(diaryFilePath))
        {
            Debug.Log($"[WriteDiary] 日记文件不存在，创建新文件并写入内容：{diaryFilePath}");
            File.Create(diaryFilePath).Close();
            //File.WriteAllText(diaryFilePath, diaryContent);
            //Debug.Log($"[WriteDiary] 新日记已写入成功：\n{diaryContent}");
            return; // 直接返回，不需要执行后续合并逻辑
        }

        // 4. 读取现有日记条目，合并内容
        string[] allDiaryEntries = File.ReadAllLines(diaryFilePath);
        bool dayExists = false;
        List<string> updatedDiaryEntries = new List<string>();

        foreach (var entry in allDiaryEntries)
        {
            if (entry.Contains($"日期：{"1"}"))
            {
                Debug.Log($"[WriteDiary] 找到当天日记，追加内容");
                updatedDiaryEntries.Add(entry + $"@@newline@@{"1"}");
                dayExists = true;
            }
            else
            {
                updatedDiaryEntries.Add(entry);
            }
        }

        if (!dayExists)
        {
            Debug.Log($"[WriteDiary] 当天日记不存在，新增记录");
            updatedDiaryEntries.Add("1");
        }

        // 5. 写入合并后的日记
        File.WriteAllLines(diaryFilePath, updatedDiaryEntries);
        Debug.Log($"[WriteDiary] 日记已写入成功：\n{"1"}");
    }


    public void ReadDiaryByDay(int day)
    {
        if (File.Exists(diaryFilePath))
        {
            string[] allDiaryEntries = File.ReadAllLines(diaryFilePath);

            // 清空 diaryDays 列表
            diaryDays.Clear();

            // 遍历所有的日记条目，提取日期数字并添加到 diaryDays 列表
            foreach (var entry in allDiaryEntries)
            {
                // 使用正则表达式提取 "Day X" 后面的数字
                var matches = System.Text.RegularExpressions.Regex.Matches(entry, @"日期：Day (\d+)");

                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    // 提取并转换为整数
                    int extractedDay = int.Parse(match.Groups[1].Value);

                    // 如果该日期没有添加到列表中，就添加
                    if (!diaryDays.Contains(extractedDay))
                    {
                        diaryDays.Add(extractedDay);
                    }
                }
            }

            // 排序并更新总天数
            diaryDays.Sort();
            totalDays = diaryDays.Count;
            // 确保 currentDay 在 diaryDays 列表中
            if (diaryDays.Count == 0)
            {
                Debug.LogWarning("没有找到任何日记条目！");
                return;
            }
            // 确保 currentDay 在 diaryDays 列表中
            currentDay = diaryDays.Contains(day) ? day : diaryDays.Last();
            UpdatePageButtons(); // 更新按钮的状态

            // 找到指定日期的日记条目
            var entriesForDay = allDiaryEntries.Where(entry => entry.Contains($"Day {currentDay}")).ToArray();

            if (entriesForDay.Length > 0)
            {
                string diaryContent = string.Join("\n", entriesForDay
                    .Select(entry => entry.Replace($"日期：Day {currentDay}@@newline@@", "")) // 去掉日期行
                    .Select(entry => entry.Replace("@@newline@@", "\n"))); // 替换换行符标识符

                dayText.text = $"第 {currentDay} 天";
                diaryText.text = diaryContent;
                // 强制重建布局
                LayoutRebuilder.ForceRebuildLayoutImmediate(textTransform);

                Debug.Log($"读取到第 {currentDay} 天的日记：\n{diaryContent}");
            }
            else
            {
                dayText.text = $"第 {currentDay} 天";
                diaryText.text = "没有找到该天的日记！";
                Debug.Log($"没有找到第 {currentDay} 天的日记！");
            }
        }
        else
        {
            dayText.text = "日记文件不存在！";
            diaryText.text = "没有找到日记文件！";
            Debug.Log("没有找到日记文件！");
        }
    }


    public void ClearDiary(bool deleteFile = false)
    {
        if (deleteFile)
        {
            // 删除日记文件
            if (File.Exists(diaryFilePath))
            {
                File.Delete(diaryFilePath); // 删除文件
                dayText.text = "";
                diaryText.text = "";
                Debug.Log("日记文件已删除！");
            }
            else
            {
                Debug.Log("日记文件不存在！");
            }
        }
        else
        {
            // 清空日记文件内容
            if (File.Exists(diaryFilePath))
            {
                // 打开文件并清空内容
                using (StreamWriter writer = new StreamWriter(diaryFilePath, false))
                {
                    writer.Write(""); // 清空文件
                }
                Debug.Log("日记内容已清空！");
            }
            else
            {
                Debug.Log("日记文件不存在！");
            }
        }
    }
    // 检查日记解锁条件


    // 切换到上一页（前一天的日记）
    public void OnPreviousPageClicked()
    {
        int currentIndex = diaryDays.IndexOf(currentDay); // 获取当前日记的索引
        if (currentIndex > 0)
        {
            currentDay = diaryDays[currentIndex - 1]; // 获取前一天的日记
            ReadDiaryByDay(currentDay);
        }
    }

    public void OnNextPageClicked()
    {
        int currentIndex = diaryDays.IndexOf(currentDay); // 获取当前日记的索引
        if (currentIndex < diaryDays.Count - 1)
        {
            currentDay = diaryDays[currentIndex + 1]; // 获取下一天的日记
            ReadDiaryByDay(currentDay);
        }
    }


    // 更新上一页和下一页按钮的状态
    private void UpdatePageButtons()
    {
        int currentIndex = diaryDays.IndexOf(currentDay);

        // 如果是第一页，则禁用上一页按钮
        if (currentIndex <= 0)
        {
            previousPageButton.SetActive(false); // 禁用上一页按钮
        }
        else
        {
            previousPageButton.SetActive(true); // 启用上一页按钮
        }

        // 如果是最后一天，则禁用下一页按钮
        if (currentIndex >= diaryDays.Count - 1)
        {
            nextPageButton.SetActive(false); // 禁用下一页按钮
        }
        else
        {
            nextPageButton.SetActive(true); // 启用下一页按钮
        }
    }

}
