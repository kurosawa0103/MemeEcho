using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class BehaviorTreeManager : MonoBehaviour
{
    public BehaviorTree currentBehavior;           // 当前正在运行的行为树
    public ExternalBehaviorTree normalBehaviorTree;   // 要切换到的新行为树
    public ExternalBehaviorTree hungryBehaviorTree;   // 要切换到的新行为树

    private void Start()
    {
        currentBehavior = GetComponent<BehaviorTree>();

    }

    private void Update()
    {
        // 在 Update 中可以通过某些条件来切换行为树
        if (Input.GetKeyDown(KeyCode.T))   // 示例：按下 T 键时切换行为树
        {
            ReplaceBehaviorTree(hungryBehaviorTree);
        }
    }

    // 替换行为树的方法
    public void ReplaceBehaviorTree(ExternalBehaviorTree newTree)
    {
        if (currentBehavior != null)
        {
            currentBehavior.DisableBehavior();  // 停止当前行为树
            currentBehavior.ExternalBehavior = newTree; // 设置新的行为树
            currentBehavior.EnableBehavior();   // 启用新行为树
            Debug.Log("行为树替换为"+ newTree.name);
        }
    }
}
