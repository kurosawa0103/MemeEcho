using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class GameTimerWindow : EditorWindow
{
    private static DateTime startTime;
    private static bool isRunning;
    private static double elapsedSeconds;

    // ������ʱ
    private static string currentSceneName;
    private static DateTime sceneEnterTime;
    private static Dictionary<string, double> sceneTimes = new Dictionary<string, double>();

    [MenuItem("Tools/Game Timer")]
    public static void ShowWindow()
    {
        GetWindow<GameTimerWindow>("Game Timer");
    }

    private void OnEnable()
    {
        EditorApplication.update += UpdateTimer;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDisable()
    {
        EditorApplication.update -= UpdateTimer;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // �Ѿ���������ģʽ
            startTime = DateTime.Now;
            elapsedSeconds = 0;
            isRunning = true;

            // ��ʼ��������¼
            currentSceneName = SceneManager.GetActiveScene().name;
            sceneEnterTime = DateTime.Now;
            sceneTimes.Clear();
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            SaveCurrentSceneTime();
            isRunning = false;
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            // �˳� Play ���ر༭��
            isRunning = false;
        }
        else if (state == PlayModeStateChange.ExitingEditMode)
        {
            // �յ���� Play��׼����������ģʽʱ���ȵ�������
            ShowWindow();
        }
    }


    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        if (isRunning)
        {
            SaveCurrentSceneTime();
            currentSceneName = newScene.name;
            sceneEnterTime = DateTime.Now;
        }
    }

    private void SaveCurrentSceneTime()
    {
        if (!string.IsNullOrEmpty(currentSceneName))
        {
            double duration = (DateTime.Now - sceneEnterTime).TotalSeconds;

            if (sceneTimes.ContainsKey(currentSceneName))
                sceneTimes[currentSceneName] += duration;
            else
                sceneTimes[currentSceneName] = duration;

            Debug.Log($"[GameTimer] ���� {currentSceneName} ���κ�ʱ: {duration:F2} �룬�ܼ�: {sceneTimes[currentSceneName]:F2} ��");
        }
    }

    private void UpdateTimer()
    {
        if (isRunning && !EditorApplication.isPaused)
        {
            elapsedSeconds = (DateTime.Now - startTime).TotalSeconds;
            Repaint();
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("��Ϸ���м�ʱ��", EditorStyles.boldLabel);

        if (isRunning)
        {
            TimeSpan time = TimeSpan.FromSeconds(elapsedSeconds);
            GUILayout.Label($"����ʱ��: {time:hh\\:mm\\:ss}", EditorStyles.largeLabel);

            if (EditorApplication.isPaused)
            {
                GUILayout.Label(" ����ͣ", EditorStyles.miniBoldLabel);
            }

            GUILayout.Space(10);
            GUILayout.Label("������ʱͳ�ƣ�", EditorStyles.boldLabel);

            foreach (var kvp in sceneTimes)
            {
                TimeSpan t = TimeSpan.FromSeconds(kvp.Value);
                GUILayout.Label($"- {kvp.Key}: {t:hh\\:mm\\:ss}");
            }

            // ��ǰ����ʵʱ����
            if (!string.IsNullOrEmpty(currentSceneName))
            {
                double currentSceneElapsed = (DateTime.Now - sceneEnterTime).TotalSeconds;
                double total = currentSceneElapsed + (sceneTimes.ContainsKey(currentSceneName) ? sceneTimes[currentSceneName] : 0);
                TimeSpan t = TimeSpan.FromSeconds(total);
                GUILayout.Label($"* ��ǰ���� {currentSceneName}: {t:hh\\:mm\\:ss}", EditorStyles.helpBox);
            }
        }
        else
        {
            GUILayout.Label("δ������", EditorStyles.label);
        }
    }
}
