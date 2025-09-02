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

    // 暂停处理
    private static bool isPaused;
    private static DateTime pauseStartTime;
    private static double pausedTotalSeconds;

    // 场景计时
    private static string currentSceneName;
    private static DateTime sceneEnterTime;
    private static Dictionary<string, double> sceneTimes = new Dictionary<string, double>();

    private static DateTime pauseTime; // 暂停时刻

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

            sceneEnterTime = DateTime.Now; // 重置当前场景计时
            Debug.Log($"[GameTimer] 场景 {currentSceneName} 本次耗时: {duration:F2} 秒，总计: {sceneTimes[currentSceneName]:F2} 秒");
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
                // 累计暂停时间
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
            // 暂停开始
            if (pauseTime == DateTime.MinValue)
                pauseTime = DateTime.Now;

            return; // 暂停期间不更新时间
        }
        else
        {
            // 恢复
            if (pauseTime != DateTime.MinValue)
            {
                TimeSpan pausedDuration = DateTime.Now - pauseTime;
                startTime = startTime.Add(pausedDuration);          // 调整游戏总计时
                sceneEnterTime = sceneEnterTime.Add(pausedDuration); // 调整当前场景计时
                pauseTime = DateTime.MinValue;
            }
        }

        elapsedSeconds = (DateTime.Now - startTime).TotalSeconds;
        Repaint();
    }

    private void OnGUI()
    {
        GUILayout.Label("游戏运行计时器", EditorStyles.boldLabel);

        if (isRunning)
        {
            TimeSpan time = TimeSpan.FromSeconds(elapsedSeconds);
            GUILayout.Label($"运行时间: {time:hh\\:mm\\:ss}", EditorStyles.largeLabel);

            if (isPaused)
            {
                GUILayout.Label(" 已暂停", EditorStyles.miniBoldLabel);
            }

            GUILayout.Space(10);
            GUILayout.Label("场景耗时统计：", EditorStyles.boldLabel);

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
                GUILayout.Label($"* 当前场景 {currentSceneName}: {t:hh\\:mm\\:ss}", EditorStyles.helpBox);
            }
        }
        else
        {
            GUILayout.Label("未在运行", EditorStyles.label);
        }
    }
}
