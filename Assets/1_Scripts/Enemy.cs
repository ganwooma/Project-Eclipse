using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour
{
    [Header("참조 객체")]
    public GameObject attackWarningPrefab;      // 공격 경고 이펙트 프리팹
    public GameObject attackEffectPrefab;       // 공격 이펙트 프리팹
    public GameObject parryingEffectPrefab;     // 패링 이펙트 프리팹
    public InputActionAsset action;             // 액션 객체
    public LayerMask playerLayer;               // 플레이어 레이어
    public AudioClip[] audioClip;               // 효과음 배열
    public GameObject player;                   // 플레이어 객체
    public GameManager gameManager;             // 게임 매니저 객체
    
    [Header("공격 관련 변수")]
    public float attackTime = 1f;               // 공격 시간
    public float attackJudgeStartTime;          // 공격 판정 시간
    public float attackJudgeTime;               // 공격 판정 시작 시간
    public float parryingTime = 0.8f;           // 패링 가능 시간
    public float attackDamage;                  // 공격 대미지

    [Header("컴포넌트 관련 변수")]
    private Player playerScript;                // 플레이어 스크립트 객체
    private Transform playerTransform;          // 플레이어 트랜스폼 객체
    private Rigidbody2D playerRigidbody;        // 플레이어 리지드바디 객체
    private Rigidbody2D rb;                     // 적 리지드바디 객체
    private AudioSource audioSource;            // 오디오 소스 객체

    [Header("패링 관련 상태 변수")]
    private bool isParried;                     // 패링 여부
    private bool isParryingTime = false;        // 패링 가능 여부

    [Header("인스턴스 저장 변수")]
    private GameObject attackInstance;          // 공격 이펙트 인스턴스
    private GameObject attackWarningInstance;   // 공격 경고 이펙트 인스턴스
    private GameObject parryingInstance;        // 패링 이펙트 인스턴스

    [Header("기타 상태 변수")]
    private bool isPlayer;                      // 플레이어 감지 여부
    private bool canAttack = true;              // 공격 가능 여부
    private bool playerIsLeft;                  // 플레이어가 왼쪽 있는지 여부

    /// <summary>
    /// 변수 초기화
    /// </summary>
    private void Awake()
    {
        playerScript = player.GetComponent<Player>(); // 플레이어 스크립트 가져오기
        playerTransform = player.GetComponent<Transform>(); // 플레이어 트랜스폼 가져오기
        playerRigidbody = player.GetComponent<Rigidbody2D>(); // 플레이어 리지드바디 가져오기
        rb = GetComponent<Rigidbody2D>(); // 적 리지드바디 가져오기
        audioSource = GetComponent<AudioSource>(); // 오디오 소스 가져오기
    }

    /// <summary>
    /// 공격하는 로직
    /// </summary>
    private void Update()
    {
        if (Vector2.Distance(player.transform.position, transform.position) > 2f && canAttack) // 플레이어와의 거리 체크
        {
            if (playerTransform.position.x > transform.position.x) // 플레이어가 적보다 오른쪽에 있을 때
            {
                transform.localScale = new Vector3(-1f, 1f, 1f); // 적을 반전시킴
                rb.linearVelocityX = 1f; // 적이 플레이어 쪽으로 이동
                playerIsLeft = false; // 플레이어가 오른쪽에 있음
            }
            else if (playerTransform.position.x < transform.position.x) // 플레이어가 적보다 왼쪽에 있을 때
            {
                transform.localScale = new Vector3(1f, 1f, 1f); // 적을 원래대로
                rb.linearVelocityX = -1f; // 적이 플레이어 쪽으로 이동
                playerIsLeft = true; // 플레이어가 왼쪽에 있음
            }
        }

        if (canAttack) // 공격이 가능하다면
        {
            if (Vector2.Distance(player.transform.position, transform.position) < 2f)
            {
                if (playerTransform.position.x > transform.position.x) // 플레이어가 적보다 오른쪽에 있을 때
                {
                    playerIsLeft = false; // 플레이어가 오른쪽에 있음
                }
                else if (playerTransform.position.x < transform.position.x) // 플레이어가 적보다 왼쪽에 있을 때
                {
                    playerIsLeft = true; // 플레이어가 왼쪽에 있음
                }

                isParried = false; // 패링 여부 X
                canAttack = false; // 이번 공격이 끝날 때까지 공격 불가

                audioSource.PlayOneShot(audioClip[0]); // 공격 소리 재생
                attackWarningInstance = Instantiate(attackWarningPrefab, transform.position + Vector3.up * 1f, Quaternion.identity); // 공격 경고 이펙트 생성

                Invoke(nameof(AttackStart), 0.2f);          // 0.2초 후에 공격 시작
                Invoke(nameof(RemoveAttackWarning), 0.5f);  // 0.5초 후에 공격 경고 이펙트 제거
            }
        }

        if (attackInstance != null)
        {
            if (playerIsLeft) // 플레이어가 왼쪽에 있을 때
            {
                attackInstance.transform.localScale = new Vector3(1f, 1f, 1f); // 공격 이펙트를 원래대로
                attackInstance.transform.position = new Vector2(transform.position.x - 1f, transform.position.y); // 공격 이펙트 위치 조정
            }
            else // 플레이어가 오른쪽에 있을 때
            {
                attackInstance.transform.localScale = new Vector3(-1f, 1f, 1f); // 공격 이펙트를 반전시킴
                attackInstance.transform.position = new Vector2(transform.position.x + 1f, transform.position.y); // 공격 이펙트 위치 조정
            }
        }
    }

    /// <summary>
    /// 마찰 적용
    /// </summary>
    private void FixedUpdate()
    {
        rb.linearVelocityX *= 0.909f;
    }

    /// <summary>
    /// 공격 시작
    /// </summary>
    void AttackStart()
    {
        playerScript.isParried = false; // 플레이어 패링 키 입력 초기화
        attackInstance = Instantiate(attackEffectPrefab, transform.position + Vector3.left * 1f, Quaternion.identity); // 공격 이펙트 생성
        if (playerIsLeft) // 플레이어가 왼쪽에 있을 때
        {
            attackInstance.transform.localScale = new Vector3(1f, 1f, 1f); // 패링 이펙트를 원래대로
        }
        else // 플레이어가 오른쪽에 있을 때
        {
            attackInstance.transform.localScale = new Vector3(-1f, 1f, 1f); // 패링 이펙트를 반전
        }
        playerScript.attacks.Add(gameObject); // 플레이어의 공격 리스트에 추가
        StartCoroutine(ParryingCoroutine());  // 패링 처리
        Invoke(nameof(AttackReset), attackTime); // 공격 시간 후에 공격 초기화
        Invoke(nameof(AttackCheck), attackJudgeStartTime); // 공격 판정 시작 딜레이 후에 공격 판정 시작
        Invoke(nameof(EndParryingTime), parryingTime); // 패링 판정 시간 끝난 후게 패링 판정 시간 종료
    }

    /// <summary>
    /// 공격 경고 이펙트 제거
    /// </summary>
    void RemoveAttackWarning() 
    {
        if (attackWarningInstance != null)
        {
            Destroy(attackWarningInstance);
        }
    }

    /// <summary>
    /// 공격 초기화
    /// </summary>
    void AttackReset()
    {
        if (attackInstance != null)
        {
            Destroy(attackInstance);
        }
        canAttack = true;
    }

    /// <summary>
    /// 패링 시간 종료
    /// </summary>
    void EndParryingTime()
    {
        if (!isParried)
        {
            playerScript.attacks.Remove(gameObject); // 플레이어의 공격 리스트에서 제거
        }
        isParryingTime = false; // 패링 시간 종료
    }

    /// <summary>
    /// 패링을 처리
    /// </summary>
    private IEnumerator ParryingCoroutine()
    {
        isParryingTime = true; // 패링 시간 시작
        while (isParryingTime) // 패링 시간이 끝날 때까지 기다림
        {
            if (playerScript.isParried && Vector2.Distance(player.transform.position, transform.position) < 2f && !isParried && !playerScript.isParryingCoolTime) // 패링 요청을 받았고(패링 키 입력), 플레이어가 근처에 있으며, 패링이 되지 않았고, 패링 쿨타임이 아니라면
            {
                if (playerScript.attacks.Count > 0)
                {
                    if (playerScript.attacks[0] == gameObject)
                    {
                        playerScript.isParried = false; // 패링 키 입력 초기화
                        playerScript.attacks.Remove(gameObject); // 플레이어의 공격 리스트에서 제거
                        isParryingTime = false; // 패링 시간 종료
                        isParried = true; // 패링 됐음

                        GetComponent<AudioSource>().PlayOneShot(audioClip[1]); // 공격 소리 재생
                        parryingInstance = Instantiate(parryingEffectPrefab, attackInstance.transform.position, Quaternion.identity); // 패링 이펙트 생성
                        player.transform.position = attackInstance.transform.position; // 플레이어 위치를 공격 위치로 이동
                        playerScript.canMove = false; // 플레이어 이동 불가하게 만들기
                        Destroy(attackInstance); // 공격 이펙트 제거
                        Time.timeScale = 0.5f; // 게임 속도 감소
                        if (playerIsLeft) // 플레이어가 왼쪽에 있을 때
                        {
                            playerRigidbody.linearVelocity = new Vector2(-3f, 0); // 플레이어 왼쪽으로 밀림
                        }
                        else // 플레이어가 오른쪽에 있을 때
                        {
                            playerRigidbody.linearVelocity = new Vector2(3f, 0); // 플레이어 오른쪽으로 밀림
                        }
                        Invoke(nameof(DesroyParryingEffect), 0.2f); // 0.2초 후에 패링 이펙트 제거
                        break; // 패링 성공 시 루프 종료
                    }
                }
            }
            yield return null; // 매 프레임마다 대기
        }
    }

    /// <summary>
    /// 패링 이펙트 제거하고 플레이어 이동 가능하게 만듬
    /// </summary>
    void DesroyParryingEffect()
    {
        Time.timeScale = 1f; // 게임 속도 원래대로
        Destroy(parryingInstance);
        playerScript.canMove = true; // 플레이어 이동 가능
    }

    /// <summary>
    /// 코루틴은 Invoke가 안돼서 대신 Invoke 받아서 코루틴 실행
    /// </summary>
    void AttackCheck()
    {
        StartCoroutine(AttackCheckCoroutine());
    }

    /// <summary>
    /// 패링 안받았을 때 공격 판정
    /// </summary>
    private IEnumerator AttackCheckCoroutine()
    {
        float timer = 0f;
        while (timer < attackTime - attackJudgeStartTime - attackJudgeTime) // 공격 시간 - 공격 판정 시간 - 패링 시간
        {
            timer += Time.deltaTime;
            if (!isPlayer) { // 플레이어가 감지 안됐으면 플레이어 감지
                if (playerIsLeft) // 플레이어가 왼쪽에 있을 때
                {
                    isPlayer = Physics2D.OverlapBox(new Vector2(transform.position.x - 1f, transform.position.y), new Vector2(1, 1), 0f, playerLayer) != null;
                }
                else //플레이어가 오른쪽에 있을 때
                {
                    isPlayer = Physics2D.OverlapBox(new Vector2(transform.position.x + 1f, transform.position.y), new Vector2(1, 1), 0f, playerLayer) != null;
                }
            }
            yield return null; // 공격 판정이 끝나면 대기
        }
        if (isPlayer && !isParried) // 플레이어가 패링 안했으면
        {
            gameManager.hp -= attackDamage; // 플레이어 체력 감소
            isPlayer = false; // 플레이어 감지 초기화
        }
    }

    /// <summary>
    /// Gizmos를 사용하여 씬 뷰에서 생성된 Collider 시각화
    /// </summary>
    void OnDrawGizmos()
    {
        // Scene 뷰에서 ground check 시각화
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(transform.position.x - 1f, transform.position.y), new Vector2(1, 1));
    }
}