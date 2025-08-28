using UnityEngine;
using Fungus;

[CommandInfo("角色系统", "激活记录开关", "设置 CharacterBehavior 中 needRecord 与拍照点开关")]
public class SetTrackObject : Command
{
    [Tooltip("要设置的角色对象（拥有 CharacterBehavior）")]
    [SerializeField] protected GameObject targetCharacter;

    public bool setRecordActive;
    public bool setPhotoActive;

    public override void OnEnter()
    {
        if (targetCharacter == null)
        {
            Debug.LogWarning("未指定角色对象！");
            Continue();
            return;
        }

        CharacterObject behavior = targetCharacter.GetComponent<CharacterObject>();
        if (behavior == null)
        {
            Debug.LogWarning($"目标对象 {targetCharacter.name} 没有 CharacterBehavior 组件！");
            Continue();
            return;
        }

        behavior.needRecord = setRecordActive;
        Debug.Log($"设置 {targetCharacter.name} 的 needRecord = {setRecordActive}");

        behavior.needPhoto = setPhotoActive;
        Debug.Log($"设置 {targetCharacter.name} 的拍照点 = {setPhotoActive}");

        Continue();
    }

    public override string GetSummary()
    {
        if (targetCharacter == null)
            return "未设置角色";

        string recordState = setRecordActive ? "记录功能：开启" : "记录功能：关闭";
        string photoState = setPhotoActive ? "拍照点：开启" : "拍照点：关闭";
        return $"{targetCharacter.name} → {recordState}，{photoState}";
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255); // 淡粉色
    }
}
