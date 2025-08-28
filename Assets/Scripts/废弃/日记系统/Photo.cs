using UnityEngine;
using Sirenix.OdinInspector; // 引入Odin Inspector命名空间

[CreateAssetMenu(fileName = "PhotoDataConfig", menuName = "创建/PhotoDataConfig", order = 1)]
public class Photo : ScriptableObject
{
    [Header("照片数据列表")]
    public Sprite  photoImage; 
    public string photoAddress;
    public string photoDesc;
}
