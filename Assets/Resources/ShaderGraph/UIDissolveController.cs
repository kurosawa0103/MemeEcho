using UnityEngine;

public class UIDissolveController : MonoBehaviour
{
    public Material dissolveMaterial;
    [Range(0f, 1f)]
    public float dissolveAmount = 0f;
    public float dissolveSpeed = 0.5f;
    private bool isDissolving = false;

    void Start()
    {
        dissolveAmount = 0f; // ȷ����ʼ��������ʾ
        dissolveMaterial.SetFloat("_Cutoff", dissolveAmount);
    }

    void Update()
    {
        if (isDissolving)
        {
            dissolveAmount += Time.deltaTime * dissolveSpeed;
            dissolveAmount = Mathf.Clamp01(dissolveAmount);
            dissolveMaterial.SetFloat("_Cutoff", dissolveAmount);

            if (dissolveAmount >= 1f)
                isDissolving = false;
        }
    }

    // �����ť�������ܽ�
    public void StartDissolve()
    {
        isDissolving = true;
    }
}