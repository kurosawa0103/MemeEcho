using System.Collections;
using UnityEngine;
using Fungus;

[CommandInfo("��չ",
             "�ƶ�����λ",
             "�ƶ�����λ")]
public class MoveToPoint : Command
{
    public enum TargetType
    {
        Player,      // ����ƶ�
        CustomObject, // ָ�������ƶ�
    }

    public enum FacingDirection
    {
        Left,  // ����
        Right  // ����
    }

    [Tooltip("Ŀ������")]
    [SerializeField] private TargetType targetType = TargetType.Player;

    [Tooltip("Ŀ������")]
    [SerializeField] private GameObject customObject;

    [Tooltip("Ŀ��λ��")]
    [SerializeField] private Transform targetPosition;

    [Tooltip("�ƶ��ٶ�")]
    [Header("�ƶ��ٶ�")]
    [SerializeField] private float moveSpeed = 5f;

    [Tooltip("�Ƿ�ȴ��ƶ����")]
    [SerializeField] private bool waitForCompletion = true;

    [Tooltip("�ƶ���ɺ����õĳ���")]
    [SerializeField] private FacingDirection finalFacingDirection = FacingDirection.Right;

    [Tooltip("�Ƿ�ʹ�� XY �����ƶ�������ѡ��ֻ�� X �����ƶ���")]
    [SerializeField] private bool useXYMovement = false;

    [Tooltip("�Ƿ����� Z ���ƶ�")]
    [SerializeField] private bool useZMovement = false;

    [Tooltip("Ŀ���λ����ֵ����")]
    [SerializeField] private float arrivalThreshold = 0.1f;

    [Header("��������")]
    [Tooltip("��������������")]
    [SerializeField] private CharacterState.CharState idleAnim; 
    [Tooltip("�ƶ�����������")]
    [SerializeField] private CharacterState.CharState walkAnim;


    public string idleAnimationName;
    public string runAnimationName;

    private CharacterState characterState;

    public override void OnEnter()
    {

        GameObject movingObject = null;
        characterState = FindObjectOfType<CharacterState>();
        // ����Ŀ������ѡ��Ҫ�ƶ�������
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
            Debug.LogWarning($"{movingObject.name} ȱ�� Rigidbody ������޷����������ƶ�");
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

        // �����ƶ�����
        PlayAnimation(runAnimationName, movingObject);
        //characterState.currentAction = CharacterState.CharAction.ControlAction;//����״̬
        //characterState.ChangeState(walkAnim);
        if (useXYMovement)
        {
            while (Vector3.Distance(movingObject.transform.position, targetPosition.position) > arrivalThreshold)
            {
                Vector3 direction = (targetPosition.position - movingObject.transform.position);

                //  �������Z���ƶ�����Z���������
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


        rb.velocity = Vector3.zero; // ֹͣ�����ٶ�

        // �������ճ���
        SetFinalFacingDirection(movingObject.transform);

        // ���Ŵ�������
        PlayAnimation(idleAnimationName, movingObject);
        //characterState.ChangeState(idleAnim);

        Continue(); // ����ִ�к�������
    }
    

    private void AdjustScaleForDirection(Vector3 direction, Transform objectTransform)
    {
        // ��̬��������
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
        // ����ö���������ճ���
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
        // ���Ŷ���������� Animator �����
        Animator animator = movingObject.transform .GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(animationName);
        }
    }

    public override string GetSummary()
    {
        string targetName = targetPosition != null ? targetPosition.name : "��Ŀ���";
        string movingObjectName;

        switch (targetType)
        {
            case TargetType.Player:
                movingObjectName = "���";
                break;
            case TargetType.CustomObject:
                movingObjectName = customObject != null ? customObject.name : "��ָ������";
                break;
            default:
                movingObjectName = "δ֪Ŀ��";
                break;
        }

        return $"����: {movingObjectName}, ��Ŀ��: {targetName}, �ٶ� {moveSpeed} , �ȴ����: {waitForCompletion}";
    }

    public override Color GetButtonColor()
    {
        return new Color32(173, 247, 199, 255);
    }
}
