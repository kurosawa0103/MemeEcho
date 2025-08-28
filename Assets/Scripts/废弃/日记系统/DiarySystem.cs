using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class DiarySystem : MonoBehaviour
{
    private string diaryFilePath; // �ռ��ļ�·��
    private TimeSystem timeSystem;

    public TextMeshProUGUI diaryText;
    public TextMeshProUGUI dayText;
    public RectTransform textTransform;
    public EventManager eventManager;
    private IEventHandler eventHandler = new EventToJson();

    public int currentDay; // ��ǰ�鿴������
    private int totalDays;  // �ܹ��ж�������ռ�

    public GameObject previousPageButton; // ��һҳ��ť
    public GameObject nextPageButton; // ��һҳ��ť

    public List<int> diaryDays = new List<int>();  // ���ڴ洢���ڵ��б�

    // ����·�������Ը�����Ҵ浵�����ɲ�ͬ��·��
    private void Awake()
    {
        timeSystem = GameObject.FindGameObjectWithTag("Mgr").GetComponent<TimeSystem>();
        diaryFilePath = Application.persistentDataPath + "/Diary.txt"; // �־û��洢·��
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(textTransform);
        }
    }
    // �����¼�ʱ��¼�ռ�
    public void WriteDiary()
    {
        //string currentDayEntry = $"Day {timeSystem.dayCount}";  // ��ǰ�ǵڼ���
        //string diaryEntry = $"���ڣ�{currentDayEntry}@@newline@@";  // ʹ���Զ��廻�з�
        //string diaryContent = diaryEntry;

        //Debug.Log($"[WriteDiary] ��ʼ��¼�ռǣ���ǰ������{timeSystem.dayCount}");

        // 1. ��ȡ��ǰ�¼�
        EventData currentEvent = eventManager.currentEvent;

        if (currentEvent == null)
        {
            Debug.LogWarning("[WriteDiary] ��ǰû���¼��������޷���¼�ռǣ�");
            return; // ����д�������
        }

        //Debug.Log($"[WriteDiary] ��ǰ�¼����ƣ�{currentEvent.eventName}, �¼����ͣ�{currentEvent.eventType}, ����ʱ��Σ�{string.Join(", ", currentEvent.availableTimes)}");

       

        bool hasWrittenEntry = false;

       
        if (!hasWrittenEntry)
        {
            Debug.LogWarning($"[WriteDiary] û�������������ռ����ݣ�����д�롣");
            return;
        }

        // 3. ����ļ������ڣ�ֱ��д�����ռ�
        if (!File.Exists(diaryFilePath))
        {
            Debug.Log($"[WriteDiary] �ռ��ļ������ڣ��������ļ���д�����ݣ�{diaryFilePath}");
            File.Create(diaryFilePath).Close();
            //File.WriteAllText(diaryFilePath, diaryContent);
            //Debug.Log($"[WriteDiary] ���ռ���д��ɹ���\n{diaryContent}");
            return; // ֱ�ӷ��أ�����Ҫִ�к����ϲ��߼�
        }

        // 4. ��ȡ�����ռ���Ŀ���ϲ�����
        string[] allDiaryEntries = File.ReadAllLines(diaryFilePath);
        bool dayExists = false;
        List<string> updatedDiaryEntries = new List<string>();

        foreach (var entry in allDiaryEntries)
        {
            if (entry.Contains($"���ڣ�{"1"}"))
            {
                Debug.Log($"[WriteDiary] �ҵ������ռǣ�׷������");
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
            Debug.Log($"[WriteDiary] �����ռǲ����ڣ�������¼");
            updatedDiaryEntries.Add("1");
        }

        // 5. д��ϲ�����ռ�
        File.WriteAllLines(diaryFilePath, updatedDiaryEntries);
        Debug.Log($"[WriteDiary] �ռ���д��ɹ���\n{"1"}");
    }


    public void ReadDiaryByDay(int day)
    {
        if (File.Exists(diaryFilePath))
        {
            string[] allDiaryEntries = File.ReadAllLines(diaryFilePath);

            // ��� diaryDays �б�
            diaryDays.Clear();

            // �������е��ռ���Ŀ����ȡ�������ֲ���ӵ� diaryDays �б�
            foreach (var entry in allDiaryEntries)
            {
                // ʹ��������ʽ��ȡ "Day X" ���������
                var matches = System.Text.RegularExpressions.Regex.Matches(entry, @"���ڣ�Day (\d+)");

                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    // ��ȡ��ת��Ϊ����
                    int extractedDay = int.Parse(match.Groups[1].Value);

                    // ���������û����ӵ��б��У������
                    if (!diaryDays.Contains(extractedDay))
                    {
                        diaryDays.Add(extractedDay);
                    }
                }
            }

            // ���򲢸���������
            diaryDays.Sort();
            totalDays = diaryDays.Count;
            // ȷ�� currentDay �� diaryDays �б���
            if (diaryDays.Count == 0)
            {
                Debug.LogWarning("û���ҵ��κ��ռ���Ŀ��");
                return;
            }
            // ȷ�� currentDay �� diaryDays �б���
            currentDay = diaryDays.Contains(day) ? day : diaryDays.Last();
            UpdatePageButtons(); // ���°�ť��״̬

            // �ҵ�ָ�����ڵ��ռ���Ŀ
            var entriesForDay = allDiaryEntries.Where(entry => entry.Contains($"Day {currentDay}")).ToArray();

            if (entriesForDay.Length > 0)
            {
                string diaryContent = string.Join("\n", entriesForDay
                    .Select(entry => entry.Replace($"���ڣ�Day {currentDay}@@newline@@", "")) // ȥ��������
                    .Select(entry => entry.Replace("@@newline@@", "\n"))); // �滻���з���ʶ��

                dayText.text = $"�� {currentDay} ��";
                diaryText.text = diaryContent;
                // ǿ���ؽ�����
                LayoutRebuilder.ForceRebuildLayoutImmediate(textTransform);

                Debug.Log($"��ȡ���� {currentDay} ����ռǣ�\n{diaryContent}");
            }
            else
            {
                dayText.text = $"�� {currentDay} ��";
                diaryText.text = "û���ҵ�������ռǣ�";
                Debug.Log($"û���ҵ��� {currentDay} ����ռǣ�");
            }
        }
        else
        {
            dayText.text = "�ռ��ļ������ڣ�";
            diaryText.text = "û���ҵ��ռ��ļ���";
            Debug.Log("û���ҵ��ռ��ļ���");
        }
    }


    public void ClearDiary(bool deleteFile = false)
    {
        if (deleteFile)
        {
            // ɾ���ռ��ļ�
            if (File.Exists(diaryFilePath))
            {
                File.Delete(diaryFilePath); // ɾ���ļ�
                dayText.text = "";
                diaryText.text = "";
                Debug.Log("�ռ��ļ���ɾ����");
            }
            else
            {
                Debug.Log("�ռ��ļ������ڣ�");
            }
        }
        else
        {
            // ����ռ��ļ�����
            if (File.Exists(diaryFilePath))
            {
                // ���ļ����������
                using (StreamWriter writer = new StreamWriter(diaryFilePath, false))
                {
                    writer.Write(""); // ����ļ�
                }
                Debug.Log("�ռ���������գ�");
            }
            else
            {
                Debug.Log("�ռ��ļ������ڣ�");
            }
        }
    }
    // ����ռǽ�������


    // �л�����һҳ��ǰһ����ռǣ�
    public void OnPreviousPageClicked()
    {
        int currentIndex = diaryDays.IndexOf(currentDay); // ��ȡ��ǰ�ռǵ�����
        if (currentIndex > 0)
        {
            currentDay = diaryDays[currentIndex - 1]; // ��ȡǰһ����ռ�
            ReadDiaryByDay(currentDay);
        }
    }

    public void OnNextPageClicked()
    {
        int currentIndex = diaryDays.IndexOf(currentDay); // ��ȡ��ǰ�ռǵ�����
        if (currentIndex < diaryDays.Count - 1)
        {
            currentDay = diaryDays[currentIndex + 1]; // ��ȡ��һ����ռ�
            ReadDiaryByDay(currentDay);
        }
    }


    // ������һҳ����һҳ��ť��״̬
    private void UpdatePageButtons()
    {
        int currentIndex = diaryDays.IndexOf(currentDay);

        // ����ǵ�һҳ���������һҳ��ť
        if (currentIndex <= 0)
        {
            previousPageButton.SetActive(false); // ������һҳ��ť
        }
        else
        {
            previousPageButton.SetActive(true); // ������һҳ��ť
        }

        // ��������һ�죬�������һҳ��ť
        if (currentIndex >= diaryDays.Count - 1)
        {
            nextPageButton.SetActive(false); // ������һҳ��ť
        }
        else
        {
            nextPageButton.SetActive(true); // ������һҳ��ť
        }
    }

}
