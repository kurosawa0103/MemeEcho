using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;
    public Transform[] slots;
    [Header("完成设置")]
    public GameObject completionImage; // 完成后显示的图片
    public float showDuration = 3f; // 图片显示时长

    [Header("打乱设置")]
    public int shuffleSteps = 50; // 打乱步数
    public bool shuffleOnStart = true; // 是否开始时自动打乱

    private PuzzlePiece[] puzzlePieces;
    private int[] correctOrder; // 正确的拼图顺序

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
        // 获取所有拼图块
        puzzlePieces = FindObjectsOfType<PuzzlePiece>();
        correctOrder = new int[puzzlePieces.Length];

        // 为每个拼图块分配正确的索引
        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            puzzlePieces[i].correctSlotIndex = i;
            correctOrder[i] = i;
        }

        // 确保完成图片开始时是隐藏的
        if (completionImage != null)
        {
            completionImage.SetActive(false);
        }
    }

    // 打乱拼图
    public void ShufflePuzzle()
    {
        StartCoroutine(ShufflePuzzleCoroutine());
    }

    private IEnumerator ShufflePuzzleCoroutine()
    {
        // 创建拼图块列表
        List<PuzzlePiece> pieceList = new List<PuzzlePiece>(puzzlePieces);

        // 打乱多次确保充分随机
        for (int step = 0; step < shuffleSteps; step++)
        {
            // 随机选择两个不同的拼图块
            int index1 = Random.Range(0, pieceList.Count);
            int index2 = Random.Range(0, pieceList.Count);

            if (index1 != index2)
            {
                // 交换两个拼图块的位置
                Transform slot1 = pieceList[index1].currentSlot;
                Transform slot2 = pieceList[index2].currentSlot;

                pieceList[index1].MoveToSlot(slot2);
                pieceList[index2].MoveToSlot(slot1);
            }

            // 添加小延迟让打乱过程可见（可选）
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

    // 检查拼图是否完成
    public void CheckPuzzleCompletion()
    {
        bool isComplete = true;

        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            // 检查每个拼图块是否在正确位置
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
        Debug.Log("拼图完成！");

        if (completionImage != null)
        {
            StartCoroutine(ShowCompletionImage());
        }
    }

    private IEnumerator ShowCompletionImage()
    {
        completionImage.SetActive(true);

        // 可以添加淡入效果
        if (completionImage.GetComponent<Image>() != null)
        {
            Image img = completionImage.GetComponent<Image>();
            Color color = img.color;
            color.a = 0f;
            img.color = color;

            // 淡入
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

        // 显示指定时长
        yield return new WaitForSeconds(showDuration);

        // 淡出或直接隐藏
        if (completionImage.GetComponent<Image>() != null)
        {
            Image img = completionImage.GetComponent<Image>();
            Color color = img.color;

            // 淡出
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
