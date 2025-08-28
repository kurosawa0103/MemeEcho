using System.Collections;
using UnityEngine;
using Fungus;

[CommandInfo("拓展",
             "移动到点位",
             "移动到点位")]
public class MoveToPoint : Command
{
    public enum TargetType
    {
        Player,      // 玩家移动
        CustomObject, // 指定物体移动
    }

    public enum FacingDirection
    {
        Left,  // 朝左
        Right  // 朝右
    }

    [Tooltip("目标类型")]
    [SerializeField] private TargetType targetType = TargetType.Player;

    [Tooltip("目标物体")]
    [SerializeField] private GameObject customObject;

    [Tooltip("目标位置")]
    [SerializeField] private Transform targetPosition;

    [Tooltip("移动速度")]
    [Header("移动速度")]
    [SerializeField] private float moveSpeed = 5f;

    [Tooltip("是否等待移动完成")]
    [SerializeField] private bool waitForCompletion = true;

    [Tooltip("移动完成后设置的朝向")]
    [SerializeField] private FacingDirection finalFacingDirection = FacingDirection.Right;

    [Tooltip("是否使用 XY 方向移动（不勾选则只在 X 轴上移动）")]
    [SerializeField] private bool useXYMovement = false;

    [Tooltip("是否启用 Z 轴移动")]
    [SerializeField] private bool useZMovement = false;

    [Tooltip("目标点位的阈值距离")]
    [SerializeField] private float arrivalThreshold = 0.1f;

    [Header("动画设置")]
    [Tooltip("待机动画的名称")]
    [SerializeField] private CharacterState.CharState idleAnim; 
    [Tooltip("移动动画的名称")]
    [SerializeField] private CharacterState.CharState walkAnim;


    public string idleAnimationName;
    public string runAnimationName;

    private CharacterState characterState;

    public override void OnEnter()
    {

        GameObject movingObject = null;
        characterState = FindObjectOfType<CharacterState>();
        // 根据目标类型选择要移动的物体
        switch (targetType)
        {
            case TargetType.Player:
                movingObject = GameObject.FindGameObjectWithTag("Player");
                break;

            case TargetType.CustomObject:
                movingObject = customObject;
                break;
        }

        Rigidbody rb = movingObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning($"{movingObject.name} 缺少 Rigidbody 组件，无法进行物理移动");
            Continue();
            return;
        }

        MoveWithPhysic(rb, movingObject);
    }

    private void MoveWithPhysic(Rigidbody rb, GameObject movingObject)
    {
        if (waitForCompletion)
        {
            StartCoroutine(MoveToTargetWithSystem(rb, movingObject));
        }
        else
        {
            StartCoroutine(MoveToTargetWithSystem(rb, movingObject));
            Continue();
        }

    }

    private IEnumerator MoveToTargetWithSystem(Rigidbody rb, GameObject movingObject)
    {

        // 播放移动动画
        PlayAnimation(runAnimationName, movingObject);
        //characterState.currentAction = CharacterState.CharAction.ControlAction;//控制状态
        //characterState.ChangeState(walkAnim);
        if (useXYMovement)
        {
            while (Vector3.Distance(movingObject.transform.position, targetPosition.position) > arrivalThreshold)
            {
                Vector3 direction = (targetPosition.position - movingObject.transform.position);

                //  如果启用Z轴移动则保留Z，否则忽略
                direction.z = useZMovement ? direction.z : 0f;

                direction = direction.normalized;

                rb.velocity = direction * moveSpeed;
                AdjustScaleForDirection(direction, movingObject.transform);
                yield return null;
            }
        }
        else
        {
            while (Mathf.Abs(movingObject.transform.position.x - targetPosition.position.x) > arrivalThreshold)
            {
                Vector3 direction = new Vector3(
                    targetPosition.position.x - movingObject.transform.position.x,
                    0f,
                    useZMovement ? targetPosition.position.z - movingObject.transform.position.z : 0f
                ).normalized;

                rb.velocity = direction * moveSpeed;
                AdjustScaleForDirection(direction, movingObject.transform);
                yield return null;
            }
        }


        rb.velocity = Vector3.zero; // 停止刚体速度

        // 设置最终朝向
        SetFinalFacingDirection(movingObject.transform);

        // 播放待机动画
        PlayAnimation(idleAnimationName, movingObject);
        //characterState.ChangeState(idleAnim);

        Continue(); // 继续执行后续流程
    }
    

    private void AdjustScaleForDirection(Vector3 direction, Transform objectTransform)
    {
        // 动态调整朝向
        if (direction.x > 0)
        {
            objectTransform.localScale = new Vector3(Mathf.Abs(objectTransform.localScale.x), objectTransform.localScale.y, objectTransform.localScale.z);
        }
        else if (direction.x < 0)
        {
            objectTransform.localScale = new Vector3(-Mathf.Abs(objectTransform.localScale.x), objectTransform.localScale.y, objectTransform.localScale.z);
        }
    }

    private void SetFinalFacingDirection(Transform objectTransform)
    {
        // 根据枚举设置最终朝向
        if (finalFacingDirection == FacingDirection.Left)
        {
            objectTransform.localScale = new Vector3(-Mathf.Abs(objectTransform.localScale.x), objectTransform.localScale.y, objectTransform.localScale.z);
        }
        else if (finalFacingDirection == FacingDirection.Right)
        {
            objectTransform.localScale = new Vector3(Mathf.Abs(objectTransform.localScale.x), objectTransform.localScale.y, objectTransform.localScale.z);
        }
    }

    private void PlayAnimation(string animationName, GameObject movingObject)
    {
        // 播放动画（如果有 Animator 组件）
        Animator animator = movingObject.transform .GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(animationName);
        }
    }

    public override string GetSummary()
    {
        string targetName = targetPosition != null ? targetPosition.name : "无目标点";
        string movingObjectName;

        switch (targetType)
        {
            case TargetType.Player:
                movingObjectName = "玩家";
                break;
            case TargetType.CustomObject:
                movingObjectName = customObject != null ? customObject.name : "无指定物体";
                break;
            default:
                movingObjectName = "未知目标";
                break;
        }

        return $"物体: {movingObjectName}, 到目标: {targetName}, 速度 {moveSpeed} , 等待完成: {waitForCompletion}";
    }

    public override Color GetButtonColor()
    {
        return new Color32(173, 247, 199, 255);
    }
}
