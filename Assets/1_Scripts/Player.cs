using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("점프 관련")]
    public float jumpForce = 3f;                                // 점프 힘
    public Vector2 boxSize = new Vector2(0.3f, 1f);             // 점프 체크 박스 크기
    public float boxPosition = 0.5f;                            // 점프 체크 박스 위치
    public LayerMask groundLayer;                               // 바닥 레이어

    [Header("이동 됨?")]
    public bool canMove = true;                                 // 이동 가능 여부

    [Header("공격 관련")]
    public List<GameObject> attacks = new List<GameObject>();   // 공격 리스트
    public bool isParried = false;                              // 패링 여부
    public bool isParryingCoolTime = false;                     // 패링 쿨타임

    [Header("게임 매니저")]
    public GameManager gameManager;                             // 게임 매니저

    [Header("상태 관련")]
    Vector2 moveInput;                                          // 이동 입력
    Rigidbody2D rb;                                             // 리지드바디


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
        // Scene 뷰에서 ground check 시각화
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(transform.position.x, transform.position.y - boxPosition), boxSize);
    }

    public void OnParrying(InputValue value)
    {
        if (value.isPressed) // 패링 버튼이 눌렸을 때
        {
            if (isParryingCoolTime) return; // 패링 쿨타임 중이면 패링 불가
            isParryingCoolTime = true;
            isParried = true; // 패링 여부 O
            Invoke(nameof(ParryingCoolTimeReset), 0.02f); // 패링 쿨타임 0.02초
        }
    }

    void ParryingCoolTimeReset()
    {
        isParryingCoolTime = false;
    }
}