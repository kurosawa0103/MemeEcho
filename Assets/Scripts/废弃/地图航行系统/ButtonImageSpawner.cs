using UnityEngine;
using UnityEngine.UI;

public class ButtonImageSpawner : MonoBehaviour
{
    public GameObject imagePrefab;  // 预制的图片
    private GameObject currentImage;  // 当前实例化的图片

    // 按钮点击事件方法
    public void OnButtonClick(Button clickedButton)
    {
        // 销毁先前实例化的图片
        if (currentImage != null)
        {
            Destroy(currentImage);
        }

        // 获取按钮的位置（这里假设按钮的锚点和图片的锚点一致）
        Vector3 buttonPosition = clickedButton.transform.localPosition;

        // 实例化新的图片
        currentImage = Instantiate(imagePrefab, buttonPosition, Quaternion.identity);

        // 你也可以通过修改图片的位置或其他属性来适应需求
        // 例如，确保图片在UI层级内显示
        currentImage.transform.SetParent(clickedButton.transform.parent, false);
    }
}
