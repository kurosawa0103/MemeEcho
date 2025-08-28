using UnityEngine;
using Fungus;

[CommandInfo("��ɫϵͳ", "�����¼����", "���� CharacterBehavior �� needRecord �����յ㿪��")]
public class SetTrackObject : Command
{
    [Tooltip("Ҫ���õĽ�ɫ����ӵ�� CharacterBehavior��")]
    [SerializeField] protected GameObject targetCharacter;

    public bool setRecordActive;
    public bool setPhotoActive;

    public override void OnEnter()
    {
        if (targetCharacter == null)
        {
            Debug.LogWarning("δָ����ɫ����");
            Continue();
            return;
        }

        CharacterObject behavior = targetCharacter.GetComponent<CharacterObject>();
        if (behavior == null)
        {
            Debug.LogWarning($"Ŀ����� {targetCharacter.name} û�� CharacterBehavior �����");
            Continue();
            return;
        }

        behavior.needRecord = setRecordActive;
        Debug.Log($"���� {targetCharacter.name} �� needRecord = {setRecordActive}");

        behavior.needPhoto = setPhotoActive;
        Debug.Log($"���� {targetCharacter.name} �����յ� = {setPhotoActive}");

        Continue();
    }

    public override string GetSummary()
    {
        if (targetCharacter == null)
            return "δ���ý�ɫ";

        string recordState = setRecordActive ? "��¼���ܣ�����" : "��¼���ܣ��ر�";
        string photoState = setPhotoActive ? "���յ㣺����" : "���յ㣺�ر�";
        return $"{targetCharacter.name} �� {recordState}��{photoState}";
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255); // ����ɫ
    }
}
