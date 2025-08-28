using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;

[Serializable]
public class PhotoData
{
    public string photoName;      // dat�ļ�·��
    public string filePath;      // dat�ļ�·��
    public string description;   // ����
    public Sprite photoImage;    // SpriteԤ��ͼ
}

public class PhotoSystem : MonoBehaviour
{
    [Header("��Ƭ��������")]
    public GameObject[] slots; // ��������
    public List<PhotoData> photos = new List<PhotoData>(); // ��Ϊ PhotoData �б�
    public Sprite emptySlotSprite;
    public int maxSlots = 8;

    public PhotoData currentPhotoData;
    public Image showImage;
    public TextMeshProUGUI showText;

    void Start()
    {
        LoadPhotos();     // ������Ƭ����
        DisplayPhotos();  // ��ʾ��Ƭ
    }

    // ���� dat �ļ���������Ƭ����
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

    // ����Ƭ��ʾ�� UI ��
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

    // �����Ƭ�����������գ�
    public void AddPhoto(string filePath)
    {
        if (photos.Exists(p => p.filePath == filePath))
        {
            Debug.Log("��Ƭ�Ѵ����ڱ����У�������ӣ�" + filePath);
            return;
        }

        if (photos.Count >= maxSlots)
        {
            Debug.Log("��Ƭ�����������޷��������Ƭ��");
            return;
        }

        var data = LoadPhotoFromDat(filePath);
        if (data != null)
        {
            photos.Add(data);
            DisplayPhotos();
            Debug.Log($"��Ƭ����ӣ�{Path.GetFileName(filePath)}");
        }
    }

    /// <summary>
    /// ��ռ�����Ƭ��Ŀ¼��EncryptedPhotos��
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
                    Debug.LogError($"ɾ����Ƭʧ�ܣ�{file}��ԭ��{e.Message}");
                }
            }

            Debug.Log("������ƬĿ¼����ա�");
        }
        else
        {
            Debug.Log("������ƬĿ¼�����ڡ�");
        }

        // ͬʱ����ڴ����ݺ�UI
        photos.Clear();
        DisplayPhotos();
        Debug.Log("��Ƭ��������գ�");
    }
    // �� .dat �ļ��ж�ȡ PhotoData
    private PhotoData LoadPhotoFromDat(string path)
    {
        if (!File.Exists(path)) return null;

        byte[] allData = File.ReadAllBytes(path);
        int offset = 0;

        // ��ȡ����
        int descLength = BitConverter.ToInt32(allData, offset);
        offset += 4;
        string desc = System.Text.Encoding.UTF8.GetString(allData, offset, descLength);
        offset += descLength;

        // ��ȡ��Ƭ��
        int nameLength = BitConverter.ToInt32(allData, offset);
        offset += 4;
        string photoName = System.Text.Encoding.UTF8.GetString(allData, offset, nameLength);
        offset += nameLength;

        // ���ؼ���ͼ������
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


    // �ӽ���ͼ�����ݣ������
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
