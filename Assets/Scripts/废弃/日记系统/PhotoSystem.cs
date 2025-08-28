using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;

[Serializable]
public class PhotoData
{
    public string photoName;      // dat文件路径
    public string filePath;      // dat文件路径
    public string description;   // 描述
    public Sprite photoImage;    // Sprite预览图
}

public class PhotoSystem : MonoBehaviour
{
    [Header("照片背包配置")]
    public GameObject[] slots; // 背包格子
    public List<PhotoData> photos = new List<PhotoData>(); // 改为 PhotoData 列表
    public Sprite emptySlotSprite;
    public int maxSlots = 8;

    public PhotoData currentPhotoData;
    public Image showImage;
    public TextMeshProUGUI showText;

    void Start()
    {
        LoadPhotos();     // 加载照片数据
        DisplayPhotos();  // 显示照片
    }

    // 加载 dat 文件并解析照片数据
    private void LoadPhotos()
    {
        photos.Clear();
        string folder = Path.Combine(Application.persistentDataPath, "EncryptedPhotos");
        if (!Directory.Exists(folder)) return;

        string[] files = Directory.GetFiles(folder, "*.dat");
        foreach (var file in files)
        {
            var data = LoadPhotoFromDat(file);
            if (data != null)
            {
                photos.Add(data);
            }
        }
    }

    // 将照片显示到 UI 上
    public void DisplayPhotos()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var slot = slots[i].transform.GetChild(0);
            var slotDec = slots[i].transform.GetChild(1);
            var image = slot.GetComponent<Image>();
            var slotScript = slot.GetComponent<ThisPhotoSlot>();

            if (i < photos.Count)
            {
                image.sprite = photos[i].photoImage;
                slotScript.thisPhotoData = photos[i];
                slotDec.GetComponent<TextMeshProUGUI>().text = photos[i].photoName;
            }
            else
            {
                image.sprite = emptySlotSprite;
                slotScript.thisPhotoData = null;
            }
        }
    }

    // 添加照片（用于新拍照）
    public void AddPhoto(string filePath)
    {
        if (photos.Exists(p => p.filePath == filePath))
        {
            Debug.Log("照片已存在于背包中，跳过添加：" + filePath);
            return;
        }

        if (photos.Count >= maxSlots)
        {
            Debug.Log("照片背包已满，无法添加新照片。");
            return;
        }

        var data = LoadPhotoFromDat(filePath);
        if (data != null)
        {
            photos.Add(data);
            DisplayPhotos();
            Debug.Log($"照片已添加：{Path.GetFileName(filePath)}");
        }
    }

    /// <summary>
    /// 清空加密照片的目录（EncryptedPhotos）
    /// </summary>
    public void ClearPhotoFolder()
    {
        string folder = Path.Combine(Application.persistentDataPath, "EncryptedPhotos");
        if (Directory.Exists(folder))
        {
            string[] files = Directory.GetFiles(folder, "*.dat");
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
                {
                    Debug.LogError($"删除照片失败：{file}，原因：{e.Message}");
                }
            }

            Debug.Log("加密照片目录已清空。");
        }
        else
        {
            Debug.Log("加密照片目录不存在。");
        }

        // 同时清空内存数据和UI
        photos.Clear();
        DisplayPhotos();
        Debug.Log("照片背包已清空！");
    }
    // 从 .dat 文件中读取 PhotoData
    private PhotoData LoadPhotoFromDat(string path)
    {
        if (!File.Exists(path)) return null;

        byte[] allData = File.ReadAllBytes(path);
        int offset = 0;

        // 读取描述
        int descLength = BitConverter.ToInt32(allData, offset);
        offset += 4;
        string desc = System.Text.Encoding.UTF8.GetString(allData, offset, descLength);
        offset += descLength;

        // 读取照片名
        int nameLength = BitConverter.ToInt32(allData, offset);
        offset += 4;
        string photoName = System.Text.Encoding.UTF8.GetString(allData, offset, nameLength);
        offset += nameLength;

        // 加载加密图像数据
        byte[] encrypted = new byte[allData.Length - offset];
        Buffer.BlockCopy(allData, offset, encrypted, 0, encrypted.Length);
        byte[] decrypted = EncryptImage(encrypted);

        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(decrypted);
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

        return new PhotoData
        {
            filePath = path,
            description = desc,
            photoName = photoName,
            photoImage = sprite,
        };
    }


    // 加解密图像数据（简单异或）
    private byte[] EncryptImage(byte[] data)
    {
        byte key = 0xAA;
        for (int i = 0; i < data.Length; i++)
        {
            data[i] ^= key;
        }
        return data;
    }

    public void UpdateCurrentPhoto()
    {
        showImage.sprite = currentPhotoData.photoImage;
        showText.text = currentPhotoData.description;
    }
}
