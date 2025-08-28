using System.Collections;
using UnityEngine;
using TMPro;

public class TextTyper : MonoBehaviour
{
    public TMP_Text targetText;
    public float typeSpeed = 0.05f;
    private Coroutine typingCoroutine;

    public void ShowText(string text)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(text));
    }

    private IEnumerator TypeText(string text)
    {
        if (targetText == null) yield break;

        targetText.text = "";

        foreach (char c in text)
        {
            targetText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        typingCoroutine = null;
    }
}
