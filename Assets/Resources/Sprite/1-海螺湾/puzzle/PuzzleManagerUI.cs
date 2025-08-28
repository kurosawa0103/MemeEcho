using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;
    public Transform[] slots;
    [Header("�������")]
    public GameObject completionImage; // ��ɺ���ʾ��ͼƬ
    public float showDuration = 3f; // ͼƬ��ʾʱ��

    [Header("��������")]
    public int shuffleSteps = 50; // ���Ҳ���
    public bool shuffleOnStart = true; // �Ƿ�ʼʱ�Զ�����

    private PuzzlePiece[] puzzlePieces;
    private int[] correctOrder; // ��ȷ��ƴͼ˳��

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializePuzzle();
        if (shuffleOnStart)
        {
            StartCoroutine(ShufflePuzzleCoroutine());
        }
    }

    private void InitializePuzzle()
    {
        // ��ȡ����ƴͼ��
        puzzlePieces = FindObjectsOfType<PuzzlePiece>();
        correctOrder = new int[puzzlePieces.Length];

        // Ϊÿ��ƴͼ�������ȷ������
        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            puzzlePieces[i].correctSlotIndex = i;
            correctOrder[i] = i;
        }

        // ȷ�����ͼƬ��ʼʱ�����ص�
        if (completionImage != null)
        {
            completionImage.SetActive(false);
        }
    }

    // ����ƴͼ
    public void ShufflePuzzle()
    {
        StartCoroutine(ShufflePuzzleCoroutine());
    }

    private IEnumerator ShufflePuzzleCoroutine()
    {
        // ����ƴͼ���б�
        List<PuzzlePiece> pieceList = new List<PuzzlePiece>(puzzlePieces);

        // ���Ҷ��ȷ��������
        for (int step = 0; step < shuffleSteps; step++)
        {
            // ���ѡ��������ͬ��ƴͼ��
            int index1 = Random.Range(0, pieceList.Count);
            int index2 = Random.Range(0, pieceList.Count);

            if (index1 != index2)
            {
                // ��������ƴͼ���λ��
                Transform slot1 = pieceList[index1].currentSlot;
                Transform slot2 = pieceList[index2].currentSlot;

                pieceList[index1].MoveToSlot(slot2);
                pieceList[index2].MoveToSlot(slot1);
            }

            // ���С�ӳ��ô��ҹ��̿ɼ�����ѡ��
            if (step % 10 == 0)
            {
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    public Transform GetNearestSlot(Vector3 pos)
    {
        Transform nearest = null;
        float minDist = float.MaxValue;
        foreach (Transform slot in slots)
        {
            float dist = Vector3.Distance(pos, slot.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = slot;
            }
        }
        return nearest;
    }

    public PuzzlePiece GetPieceInSlot(Transform slot)
    {
        foreach (PuzzlePiece piece in puzzlePieces)
        {
            if (piece.currentSlot == slot)
                return piece;
        }
        return null;
    }

    // ���ƴͼ�Ƿ����
    public void CheckPuzzleCompletion()
    {
        bool isComplete = true;

        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            // ���ÿ��ƴͼ���Ƿ�����ȷλ��
            int currentSlotIndex = System.Array.IndexOf(slots, puzzlePieces[i].currentSlot);
            if (currentSlotIndex != puzzlePieces[i].correctSlotIndex)
            {
                isComplete = false;
                break;
            }
        }

        if (isComplete)
        {
            OnPuzzleCompleted();
        }
    }

    private void OnPuzzleCompleted()
    {
        Debug.Log("ƴͼ��ɣ�");

        if (completionImage != null)
        {
            StartCoroutine(ShowCompletionImage());
        }
    }

    private IEnumerator ShowCompletionImage()
    {
        completionImage.SetActive(true);

        // ������ӵ���Ч��
        if (completionImage.GetComponent<Image>() != null)
        {
            Image img = completionImage.GetComponent<Image>();
            Color color = img.color;
            color.a = 0f;
            img.color = color;

            // ����
            float timer = 0f;
            float fadeTime = 0.5f;
            while (timer < fadeTime)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(0f, 1f, timer / fadeTime);
                img.color = color;
                yield return null;
            }
        }

        // ��ʾָ��ʱ��
        yield return new WaitForSeconds(showDuration);

        // ������ֱ������
        if (completionImage.GetComponent<Image>() != null)
        {
            Image img = completionImage.GetComponent<Image>();
            Color color = img.color;

            // ����
            float timer = 0f;
            float fadeTime = 0.5f;
            while (timer < fadeTime)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(1f, 0f, timer / fadeTime);
                img.color = color;
                yield return null;
            }
        }

        completionImage.SetActive(false);
    }
}
