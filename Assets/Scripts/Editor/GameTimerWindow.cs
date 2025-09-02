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

    // ��ͣ����
    private static bool isPaused;
    private static DateTime pauseStartTime;
    private static double pausedTotalSeconds;

    // ������ʱ
    private static string currentSceneName;
    private static DateTime sceneEnterTime;
    private static Dictionary<string, double> sceneTimes = new Dictionary<string, double>();

    private static DateTime pauseTime; // ��ͣʱ��

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
        EditorApplication.pauseStateChanged += OnEditorPauseChanged;
    }

    private void OnDisable()
    {
        EditorApplication.update -= UpdateTimer;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        SceneManager.activeSceneChanged -= OnSceneChanged;
        EditorApplication.pauseStateChanged -= OnEditorPauseChanged;
    }

    private void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            startTime = DateTime.Now;
            elapsedSeconds = 0;
            isRunning = true;
            isPaused = false;
            pausedTotalSeconds = 0;

            currentSceneName = SceneManager.GetActiveScene().name;
            sceneEnterTime = DateTime.Now;
            sceneTimes.Clear();
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            SaveCurrentSceneTime();
            isRunning = false;
        }
        else if (state == PlayModeStateChange.ExitingEditMode)
        {
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

            sceneEnterTime = DateTime.Now; // ���õ�ǰ������ʱ
            Debug.Log($"[GameTimer] ���� {currentSceneName} ���κ�ʱ: {duration:F2} �룬�ܼ�: {sceneTimes[currentSceneName]:F2} ��");
        }
    }

    private void OnEditorPauseChanged(PauseState state)
    {
        if (state == PauseState.Paused)
        {
            isPaused = true;
            pauseStartTime = DateTime.Now;
        }
        else if (state == PauseState.Unpaused)
        {
            if (isPaused)
            {
                // �ۼ���ͣʱ��
                pausedTotalSeconds += (DateTime.Now - pauseStartTime).TotalSeconds;
                isPaused = false;
            }
        }
    }

    private void UpdateTimer()
    {
        if (!isRunning) return;

        if (EditorApplication.isPaused)
        {
            // ��ͣ��ʼ
            if (pauseTime == DateTime.MinValue)
                pauseTime = DateTime.Now;

            return; // ��ͣ�ڼ䲻����ʱ��
        }
        else
        {
            // �ָ�
            if (pauseTime != DateTime.MinValue)
            {
                TimeSpan pausedDuration = DateTime.Now - pauseTime;
                startTime = startTime.Add(pausedDuration);          // ������Ϸ�ܼ�ʱ
                sceneEnterTime = sceneEnterTime.Add(pausedDuration); // ������ǰ������ʱ
                pauseTime = DateTime.MinValue;
            }
        }

        elapsedSeconds = (DateTime.Now - startTime).TotalSeconds;
        Repaint();
    }

    private void OnGUI()
    {
        GUILayout.Label("��Ϸ���м�ʱ��", EditorStyles.boldLabel);

        if (isRunning)
        {
            TimeSpan time = TimeSpan.FromSeconds(elapsedSeconds);
            GUILayout.Label($"����ʱ��: {time:hh\\:mm\\:ss}", EditorStyles.largeLabel);

            if (isPaused)
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
