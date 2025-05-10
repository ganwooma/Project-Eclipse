using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("���� ����")]
    public float jumpForce = 3f;                                // ���� ��
    public Vector2 boxSize = new Vector2(0.3f, 1f);             // ���� üũ �ڽ� ũ��
    public float boxPosition = 0.5f;                            // ���� üũ �ڽ� ��ġ
    public LayerMask groundLayer;                               // �ٴ� ���̾�

    [Header("�̵� ��?")]
    public bool canMove = true;                                 // �̵� ���� ����

    [Header("���� ����")]
    public List<GameObject> attacks = new List<GameObject>();   // ���� ����Ʈ
    public bool isParried = false;                              // �и� ����
    public bool isParryingCoolTime = false;                     // �и� ��Ÿ��

    [Header("���� �Ŵ���")]
    public GameManager gameManager;                             // ���� �Ŵ���

    [Header("���� ����")]
    Vector2 moveInput;                                          // �̵� �Է�
    Rigidbody2D rb;                                             // ������ٵ�


    private bool isGround;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void Update()
    {
        isGround = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - boxPosition), boxSize, 0f, groundLayer);

        if (moveInput.x != 0 && canMove) {
            rb.linearVelocityX = moveInput.x * gameManager.speed;
        }
    }

    public void OnJump(InputValue value)
    {
        if (isGround && value.isPressed && canMove)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocityX *= 0.909f;
    }

    void OnDrawGizmos()
    {
        // Scene �信�� ground check �ð�ȭ
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(transform.position.x, transform.position.y - boxPosition), boxSize);
    }

    public void OnParrying(InputValue value)
    {
        if (value.isPressed) // �и� ��ư�� ������ ��
        {
            if (isParryingCoolTime) return; // �и� ��Ÿ�� ���̸� �и� �Ұ�
            isParryingCoolTime = true;
            isParried = true; // �и� ���� O
            Invoke(nameof(ParryingCoolTimeReset), 0.02f); // �и� ��Ÿ�� 0.02��
        }
    }

    void ParryingCoolTimeReset()
    {
        isParryingCoolTime = false;
    }
}