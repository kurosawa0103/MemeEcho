using Fungus;
using UnityEngine;

[CommandInfo("Custom",
             "Set Sailing",
             "设置是否航行")]
public class SetSailing : Command
{
    [Tooltip("航行状态控制（启用或禁用）")]
    public bool isSailing;

    [Tooltip("Sailor 脚本引用")]
    private Sailor sailor;

    [Tooltip("涟漪效果父节点引用（如果有的话）")]
    public Transform rippleEffectParent; // 父节点，用于查找所有粒子系统

    public override void OnEnter()
    {
        // 自动寻找 Sailor 脚本（可选）
        if (sailor == null)
        {
            sailor = GameObject.FindObjectOfType<Sailor>();
        }

        if (sailor != null)
        {
            sailor.isSailing = isSailing; // 设置航行状态
        }
        else
        {
            Debug.LogWarning("未找到 Sailor 脚本！");
        }

        // 遍历父节点下的所有子物体，查找 ParticleSystem
        if (rippleEffectParent != null)
        {
            ParticleSystem[] particleSystems = rippleEffectParent.GetComponentsInChildren<ParticleSystem>();

            if (!isSailing)
            {
                // 停止航行时关闭所有粒子效果
                foreach (var particleSystem in particleSystems)
                {
                    particleSystem.Stop();
                }
            }
            else
            {
                // 启动航行时播放所有粒子效果
                foreach (var particleSystem in particleSystems)
                {
                    particleSystem.Play();
                }
            }
        }

        Continue();
    }

    public override string GetSummary()
    {
        return sailor != null
            ? $"设置 {sailor.name} 航行状态为：{(isSailing ? "启用" : "禁用")}"
            : "Sailor 未指定";
    }
}
