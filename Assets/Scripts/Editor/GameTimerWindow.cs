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

    // 场景计时
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
            // 已经进入运行模式
            startTime = DateTime.Now;
            elapsedSeconds = 0;
            isRunning = true;

            // 初始化场景记录
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
            // 退出 Play 返回编辑器
            isRunning = false;
        }
        else if (state == PlayModeStateChange.ExitingEditMode)
        {
            // 刚点击了 Play，准备进入运行模式时，先弹出窗口
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

            Debug.Log($"[GameTimer] 场景 {currentSceneName} 本次耗时: {duration:F2} 秒，总计: {sceneTimes[currentSceneName]:F2} 秒");
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
        GUILayout.Label("游戏运行计时器", EditorStyles.boldLabel);

        if (isRunning)
        {
            TimeSpan time = TimeSpan.FromSeconds(elapsedSeconds);
            GUILayout.Label($"运行时间: {time:hh\\:mm\\:ss}", EditorStyles.largeLabel);

            if (EditorApplication.isPaused)
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

            // 当前场景实时更新
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
