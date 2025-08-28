using UnityEngine;
using UnityEngine.UI;

public class ButtonImageSpawner : MonoBehaviour
{
    public GameObject imagePrefab;  // Ԥ�Ƶ�ͼƬ
    private GameObject currentImage;  // ��ǰʵ������ͼƬ

    // ��ť����¼�����
    public void OnButtonClick(Button clickedButton)
    {
        // ������ǰʵ������ͼƬ
        if (currentImage != null)
        {
            Destroy(currentImage);
        }

        // ��ȡ��ť��λ�ã�������谴ť��ê���ͼƬ��ê��һ�£�
        Vector3 buttonPosition = clickedButton.transform.localPosition;

        // ʵ�����µ�ͼƬ
        currentImage = Instantiate(imagePrefab, buttonPosition, Quaternion.identity);

        // ��Ҳ����ͨ���޸�ͼƬ��λ�û�������������Ӧ����
        // ���磬ȷ��ͼƬ��UI�㼶����ʾ
        currentImage.transform.SetParent(clickedButton.transform.parent, false);
    }
}
