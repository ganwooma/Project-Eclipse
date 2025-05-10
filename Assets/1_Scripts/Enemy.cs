using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour
{
    [Header("���� ��ü")]
    public GameObject attackWarningPrefab;      // ���� ��� ����Ʈ ������
    public GameObject attackEffectPrefab;       // ���� ����Ʈ ������
    public GameObject parryingEffectPrefab;     // �и� ����Ʈ ������
    public InputActionAsset action;             // �׼� ��ü
    public LayerMask playerLayer;               // �÷��̾� ���̾�
    public AudioClip[] audioClip;               // ȿ���� �迭
    public GameObject player;                   // �÷��̾� ��ü
    public GameManager gameManager;             // ���� �Ŵ��� ��ü
    
    [Header("���� ���� ����")]
    public float attackTime = 1f;               // ���� �ð�
    public float attackJudgeStartTime;          // ���� ���� �ð�
    public float attackJudgeTime;               // ���� ���� ���� �ð�
    public float parryingTime = 0.8f;           // �и� ���� �ð�
    public float attackDamage;                  // ���� �����

    [Header("������Ʈ ���� ����")]
    private Player playerScript;                // �÷��̾� ��ũ��Ʈ ��ü
    private Transform playerTransform;          // �÷��̾� Ʈ������ ��ü
    private Rigidbody2D playerRigidbody;        // �÷��̾� ������ٵ� ��ü
    private Rigidbody2D rb;                     // �� ������ٵ� ��ü
    private AudioSource audioSource;            // ����� �ҽ� ��ü

    [Header("�и� ���� ���� ����")]
    private bool isParried;                     // �и� ����
    private bool isParryingTime = false;        // �и� ���� ����

    [Header("�ν��Ͻ� ���� ����")]
    private GameObject attackInstance;          // ���� ����Ʈ �ν��Ͻ�
    private GameObject attackWarningInstance;   // ���� ��� ����Ʈ �ν��Ͻ�
    private GameObject parryingInstance;        // �и� ����Ʈ �ν��Ͻ�

    [Header("��Ÿ ���� ����")]
    private bool isPlayer;                      // �÷��̾� ���� ����
    private bool canAttack = true;              // ���� ���� ����
    private bool playerIsLeft;                  // �÷��̾ ���� �ִ��� ����

    /// <summary>
    /// ���� �ʱ�ȭ
    /// </summary>
    private void Awake()
    {
        playerScript = player.GetComponent<Player>(); // �÷��̾� ��ũ��Ʈ ��������
        playerTransform = player.GetComponent<Transform>(); // �÷��̾� Ʈ������ ��������
        playerRigidbody = player.GetComponent<Rigidbody2D>(); // �÷��̾� ������ٵ� ��������
        rb = GetComponent<Rigidbody2D>(); // �� ������ٵ� ��������
        audioSource = GetComponent<AudioSource>(); // ����� �ҽ� ��������
    }

    /// <summary>
    /// �����ϴ� ����
    /// </summary>
    private void Update()
    {
        if (Vector2.Distance(player.transform.position, transform.position) > 2f && canAttack) // �÷��̾���� �Ÿ� üũ
        {
            if (playerTransform.position.x > transform.position.x) // �÷��̾ ������ �����ʿ� ���� ��
            {
                transform.localScale = new Vector3(-1f, 1f, 1f); // ���� ������Ŵ
                rb.linearVelocityX = 1f; // ���� �÷��̾� ������ �̵�
                playerIsLeft = false; // �÷��̾ �����ʿ� ����
            }
            else if (playerTransform.position.x < transform.position.x) // �÷��̾ ������ ���ʿ� ���� ��
            {
                transform.localScale = new Vector3(1f, 1f, 1f); // ���� �������
                rb.linearVelocityX = -1f; // ���� �÷��̾� ������ �̵�
                playerIsLeft = true; // �÷��̾ ���ʿ� ����
            }
        }

        if (canAttack) // ������ �����ϴٸ�
        {
            if (Vector2.Distance(player.transform.position, transform.position) < 2f)
            {
                if (playerTransform.position.x > transform.position.x) // �÷��̾ ������ �����ʿ� ���� ��
                {
                    playerIsLeft = false; // �÷��̾ �����ʿ� ����
                }
                else if (playerTransform.position.x < transform.position.x) // �÷��̾ ������ ���ʿ� ���� ��
                {
                    playerIsLeft = true; // �÷��̾ ���ʿ� ����
                }

                isParried = false; // �и� ���� X
                canAttack = false; // �̹� ������ ���� ������ ���� �Ұ�

                audioSource.PlayOneShot(audioClip[0]); // ���� �Ҹ� ���
                attackWarningInstance = Instantiate(attackWarningPrefab, transform.position + Vector3.up * 1f, Quaternion.identity); // ���� ��� ����Ʈ ����

                Invoke(nameof(AttackStart), 0.2f);          // 0.2�� �Ŀ� ���� ����
                Invoke(nameof(RemoveAttackWarning), 0.5f);  // 0.5�� �Ŀ� ���� ��� ����Ʈ ����
            }
        }

        if (attackInstance != null)
        {
            if (playerIsLeft) // �÷��̾ ���ʿ� ���� ��
            {
                attackInstance.transform.localScale = new Vector3(1f, 1f, 1f); // ���� ����Ʈ�� �������
                attackInstance.transform.position = new Vector2(transform.position.x - 1f, transform.position.y); // ���� ����Ʈ ��ġ ����
            }
            else // �÷��̾ �����ʿ� ���� ��
            {
                attackInstance.transform.localScale = new Vector3(-1f, 1f, 1f); // ���� ����Ʈ�� ������Ŵ
                attackInstance.transform.position = new Vector2(transform.position.x + 1f, transform.position.y); // ���� ����Ʈ ��ġ ����
            }
        }
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    private void FixedUpdate()
    {
        rb.linearVelocityX *= 0.909f;
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    void AttackStart()
    {
        playerScript.isParried = false; // �÷��̾� �и� Ű �Է� �ʱ�ȭ
        attackInstance = Instantiate(attackEffectPrefab, transform.position + Vector3.left * 1f, Quaternion.identity); // ���� ����Ʈ ����
        if (playerIsLeft) // �÷��̾ ���ʿ� ���� ��
        {
            attackInstance.transform.localScale = new Vector3(1f, 1f, 1f); // �и� ����Ʈ�� �������
        }
        else // �÷��̾ �����ʿ� ���� ��
        {
            attackInstance.transform.localScale = new Vector3(-1f, 1f, 1f); // �и� ����Ʈ�� ����
        }
        playerScript.attacks.Add(gameObject); // �÷��̾��� ���� ����Ʈ�� �߰�
        StartCoroutine(ParryingCoroutine());  // �и� ó��
        Invoke(nameof(AttackReset), attackTime); // ���� �ð� �Ŀ� ���� �ʱ�ȭ
        Invoke(nameof(AttackCheck), attackJudgeStartTime); // ���� ���� ���� ������ �Ŀ� ���� ���� ����
        Invoke(nameof(EndParryingTime), parryingTime); // �и� ���� �ð� ���� �İ� �и� ���� �ð� ����
    }

    /// <summary>
    /// ���� ��� ����Ʈ ����
    /// </summary>
    void RemoveAttackWarning() 
    {
        if (attackWarningInstance != null)
        {
            Destroy(attackWarningInstance);
        }
    }

    /// <summary>
    /// ���� �ʱ�ȭ
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
    /// �и� �ð� ����
    /// </summary>
    void EndParryingTime()
    {
        if (!isParried)
        {
            playerScript.attacks.Remove(gameObject); // �÷��̾��� ���� ����Ʈ���� ����
        }
        isParryingTime = false; // �и� �ð� ����
    }

    /// <summary>
    /// �и��� ó��
    /// </summary>
    private IEnumerator ParryingCoroutine()
    {
        isParryingTime = true; // �и� �ð� ����
        while (isParryingTime) // �и� �ð��� ���� ������ ��ٸ�
        {
            if (playerScript.isParried && Vector2.Distance(player.transform.position, transform.position) < 2f && !isParried && !playerScript.isParryingCoolTime) // �и� ��û�� �޾Ұ�(�и� Ű �Է�), �÷��̾ ��ó�� ������, �и��� ���� �ʾҰ�, �и� ��Ÿ���� �ƴ϶��
            {
                if (playerScript.attacks.Count > 0)
                {
                    if (playerScript.attacks[0] == gameObject)
                    {
                        playerScript.isParried = false; // �и� Ű �Է� �ʱ�ȭ
                        playerScript.attacks.Remove(gameObject); // �÷��̾��� ���� ����Ʈ���� ����
                        isParryingTime = false; // �и� �ð� ����
                        isParried = true; // �и� ����

                        GetComponent<AudioSource>().PlayOneShot(audioClip[1]); // ���� �Ҹ� ���
                        parryingInstance = Instantiate(parryingEffectPrefab, attackInstance.transform.position, Quaternion.identity); // �и� ����Ʈ ����
                        player.transform.position = attackInstance.transform.position; // �÷��̾� ��ġ�� ���� ��ġ�� �̵�
                        playerScript.canMove = false; // �÷��̾� �̵� �Ұ��ϰ� �����
                        Destroy(attackInstance); // ���� ����Ʈ ����
                        Time.timeScale = 0.5f; // ���� �ӵ� ����
                        if (playerIsLeft) // �÷��̾ ���ʿ� ���� ��
                        {
                            playerRigidbody.linearVelocity = new Vector2(-3f, 0); // �÷��̾� �������� �и�
                        }
                        else // �÷��̾ �����ʿ� ���� ��
                        {
                            playerRigidbody.linearVelocity = new Vector2(3f, 0); // �÷��̾� ���������� �и�
                        }
                        Invoke(nameof(DesroyParryingEffect), 0.2f); // 0.2�� �Ŀ� �и� ����Ʈ ����
                        break; // �и� ���� �� ���� ����
                    }
                }
            }
            yield return null; // �� �����Ӹ��� ���
        }
    }

    /// <summary>
    /// �и� ����Ʈ �����ϰ� �÷��̾� �̵� �����ϰ� ����
    /// </summary>
    void DesroyParryingEffect()
    {
        Time.timeScale = 1f; // ���� �ӵ� �������
        Destroy(parryingInstance);
        playerScript.canMove = true; // �÷��̾� �̵� ����
    }

    /// <summary>
    /// �ڷ�ƾ�� Invoke�� �ȵż� ��� Invoke �޾Ƽ� �ڷ�ƾ ����
    /// </summary>
    void AttackCheck()
    {
        StartCoroutine(AttackCheckCoroutine());
    }

    /// <summary>
    /// �и� �ȹ޾��� �� ���� ����
    /// </summary>
    private IEnumerator AttackCheckCoroutine()
    {
        float timer = 0f;
        while (timer < attackTime - attackJudgeStartTime - attackJudgeTime) // ���� �ð� - ���� ���� �ð� - �и� �ð�
        {
            timer += Time.deltaTime;
            if (!isPlayer) { // �÷��̾ ���� �ȵ����� �÷��̾� ����
                if (playerIsLeft) // �÷��̾ ���ʿ� ���� ��
                {
                    isPlayer = Physics2D.OverlapBox(new Vector2(transform.position.x - 1f, transform.position.y), new Vector2(1, 1), 0f, playerLayer) != null;
                }
                else //�÷��̾ �����ʿ� ���� ��
                {
                    isPlayer = Physics2D.OverlapBox(new Vector2(transform.position.x + 1f, transform.position.y), new Vector2(1, 1), 0f, playerLayer) != null;
                }
            }
            yield return null; // ���� ������ ������ ���
        }
        if (isPlayer && !isParried) // �÷��̾ �и� ��������
        {
            gameManager.hp -= attackDamage; // �÷��̾� ü�� ����
            isPlayer = false; // �÷��̾� ���� �ʱ�ȭ
        }
    }

    /// <summary>
    /// Gizmos�� ����Ͽ� �� �信�� ������ Collider �ð�ȭ
    /// </summary>
    void OnDrawGizmos()
    {
        // Scene �信�� ground check �ð�ȭ
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(transform.position.x - 1f, transform.position.y), new Vector2(1, 1));
    }
}