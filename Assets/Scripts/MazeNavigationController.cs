using UnityEngine;
using Fungus;

/// <summary>
/// 迷宫导航系统管理脚本
/// 控制三个迷宫场景的切换和玩家的导航逻辑
/// </summary>
public class MazeNavigationController : MonoBehaviour
{
    [Header("迷宫场景引用")]
    public GameObject maze1; // 迷宫1
    public GameObject maze2; // 迷宫1 (1) 
    public GameObject maze3; // 迷宫1 (2)
    public GameObject maze4; // 迷宫1 (3) - 如果有的话
    
    [Header("迷宫正确路径配置")]
    [Tooltip("每个迷宫的正确方向：0=上,1=下,2=左,3=右")]
    public int[] correctDirections = {2, 3, 0}; // 默认：左,右,上
    
    [Header("Flowchart引用")]
    public Flowchart flowchart;
    
    // 当前迷宫索引
    private int currentMazeIndex = 0;
    
    // 迷宫数组
    private GameObject[] mazes;
    
    void Start()
    {
        // 初始化迷宫数组
        mazes = new GameObject[] { maze1, maze2, maze3, maze4 };
        
        // 如果没有手动分配Flowchart，尝试从当前物体获取
        if (flowchart == null)
        {
            flowchart = GetComponent<Flowchart>();
        }
        
        // 开始游戏
        StartMazeGame();
    }
    
    /// <summary>
    /// 开始迷宫游戏
    /// </summary>
    public void StartMazeGame()
    {
        currentMazeIndex = 0;
        ActivateMaze(0);
        
        // 设置Flowchart变量
        if (flowchart != null)
        {
            flowchart.SetIntegerVariable("CurrentMaze", currentMazeIndex);
            flowchart.SetBooleanVariable("GameStarted", true);
        }
    }
    
    /// <summary>
    /// 激活指定的迷宫
    /// </summary>
    /// <param name="mazeIndex">迷宫索引</param>
    public void ActivateMaze(int mazeIndex)
    {
        // 关闭所有迷宫
        for (int i = 0; i < mazes.Length; i++)
        {
            if (mazes[i] != null)
            {
                mazes[i].SetActive(false);
            }
        }
        
        // 激活指定迷宫
        if (mazeIndex >= 0 && mazeIndex < mazes.Length && mazes[mazeIndex] != null)
        {
            mazes[mazeIndex].SetActive(true);
            currentMazeIndex = mazeIndex;
            
            Debug.Log($"激活迷宫 {mazeIndex + 1}");
            
            // 更新Flowchart变量
            if (flowchart != null)
            {
                flowchart.SetIntegerVariable("CurrentMaze", currentMazeIndex);
            }
        }
    }
    
    /// <summary>
    /// 处理方向按钮点击
    /// </summary>
    /// <param name="direction">方向：0=上,1=下,2=左,3=右</param>
    public void OnDirectionClicked(int direction)
    {
        if (currentMazeIndex >= correctDirections.Length)
        {
            Debug.Log("已完成所有迷宫！");
            OnAllMazesCompleted();
            return;
        }
        
        int correctDirection = correctDirections[currentMazeIndex];
        
        if (direction == correctDirection)
        {
            // 正确选择
            OnCorrectChoice();
        }
        else
        {
            // 错误选择
            OnWrongChoice();
        }
    }
    
    /// <summary>
    /// 正确选择的处理
    /// </summary>
    private void OnCorrectChoice()
    {
        Debug.Log("选择正确！");
        
        currentMazeIndex++;
        
        if (currentMazeIndex < mazes.Length && currentMazeIndex < correctDirections.Length)
        {
            // 进入下一个迷宫
            ActivateMaze(currentMazeIndex);
            
            // 触发Flowchart中的正确选择事件
            if (flowchart != null)
            {
                flowchart.SetBooleanVariable("CorrectChoice", true);
                flowchart.ExecuteBlock("OnCorrectChoice");
            }
        }
        else
        {
            // 完成所有迷宫
            OnAllMazesCompleted();
        }
    }
    
    /// <summary>
    /// 错误选择的处理
    /// </summary>
    private void OnWrongChoice()
    {
        Debug.Log("选择错误！返回第一个迷宫");
        
        // 返回第一个迷宫
        ActivateMaze(0);
        
        // 触发Flowchart中的错误选择事件
        if (flowchart != null)
        {
            flowchart.SetBooleanVariable("WrongChoice", true);
            flowchart.ExecuteBlock("OnWrongChoice");
        }
    }
    
    /// <summary>
    /// 完成所有迷宫
    /// </summary>
    private void OnAllMazesCompleted()
    {
        Debug.Log("恭喜！完成所有迷宫！");
        
        // 触发Flowchart中的完成事件
        if (flowchart != null)
        {
            flowchart.SetBooleanVariable("AllMazesCompleted", true);
            flowchart.ExecuteBlock("OnAllMazesCompleted");
        }
    }
    
    // 以下方法用于在Flowchart中直接调用
    public void ClickUp() => OnDirectionClicked(0);
    public void ClickDown() => OnDirectionClicked(1);
    public void ClickLeft() => OnDirectionClicked(2);
    public void ClickRight() => OnDirectionClicked(3);
    
    /// <summary>
    /// 重置游戏
    /// </summary>
    public void ResetGame()
    {
        StartMazeGame();
        
        if (flowchart != null)
        {
            flowchart.SetBooleanVariable("GameReset", true);
        }
    }
}