using UnityEngine;
using Sirenix.OdinInspector; // ����Odin Inspector�����ռ�

[CreateAssetMenu(fileName = "PhotoDataConfig", menuName = "����/PhotoDataConfig", order = 1)]
public class Photo : ScriptableObject
{
    [Header("��Ƭ�����б�")]
    public Sprite  photoImage; 
    public string photoAddress;
    public string photoDesc;
}
