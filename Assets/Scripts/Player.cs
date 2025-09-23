using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] private LayerMask groundLayerMask = -1; // 地面レイヤー
    private float moveSpeed = 1f;       // 移動速度
    private float rotationSpeed = 120f; // 回転速度
    private float jumpForce = 3000f;       // ジャンプ力
    private float groundCheckDistance = 2f; // 地面チェック距離
    private float gravityMultiplier = 2f; // 重力の倍率

    Rigidbody rb;
    Animator animator; // Animatorコンポーネント

    private bool isGrounded = true; // 現在の地面との接地状態
    private Vector3 moveDirection; // 現在の移動方向
    private float currentSpeed; // 現在の移動速度

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // 物理演算での回転を制限（Y軸回転のみ許可）
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        // ジャンプ入力
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // アニメーションの更新
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        Move();
        Rotate();
        GroundCheck();
        ApplyGravity();
    }

    void Move()
    {
        // 入力取得
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // カメラの向きに合わせた移動方向
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical);

        // 入力がある場合のみ処理を行う
        if (inputDirection.sqrMagnitude > 0.01f)
        {
            inputDirection = inputDirection.normalized;

            Quaternion cameraRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
            moveDirection = cameraRotation * inputDirection;

            // 移動速度の計算（Rigidbodyベース）
            Vector3 targetVelocity = moveDirection * moveSpeed;

            // 現在の垂直速度を保持しながら水平移動を適用
            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

            // 現在の速度を記録（Animation用）
            currentSpeed = inputDirection.magnitude * moveSpeed;
        }
        else
        {
            // 入力がない場合、水平方向の速度をスムーズに減速
            Vector3 currentVelocity = rb.linearVelocity;
            rb.linearVelocity = new Vector3(
                Mathf.MoveTowards(currentVelocity.x, 0, moveSpeed * 2 * Time.fixedDeltaTime),
                currentVelocity.y,
                Mathf.MoveTowards(currentVelocity.z, 0, moveSpeed * 2 * Time.fixedDeltaTime)
            );

            currentSpeed = 0f;
            moveDirection = Vector3.zero;
        }
    }

    void Rotate()
    {
        // 移動方向がある場合のみ回転処理を行う
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            // ターゲットの回転方向
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            // スムーズに回転（Rigidbodyベース）
            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newRotation);
        }

        // 角速度をリセットして不要な回転を防ぐ
        rb.angularVelocity = Vector3.zero;
    }

    void Jump()
    {
        // ジャンプ力を適用（Rigidbodyベース）
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false; // ジャンプ時は即座に地面から離れたとみなす
    }

    void GroundCheck()
    {
        // プレイヤーの中心から下向きにレイキャスト
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;
        
        bool wasGrounded = isGrounded;

        // レイキャストで地面をチェック
        if (Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance, groundLayerMask))
        {
            // 垂直速度が下向きの場合のみ着地判定
            if (rb.linearVelocity.y <= 0.1f)
            {
                isGrounded = true;
            }
        }
        else
        {
            isGrounded = false;
        }

        // デバッグ用：地面チェックのレイを可視化
        Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }

    void ApplyGravity()
    {
        // カスタム重力を適用（Rigidbodyベース）
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        // 移動速度をAnimatorに渡す（0-1の範囲に正規化）
        float normalizedSpeed = currentSpeed / moveSpeed;
        animator.SetFloat("Speed", normalizedSpeed);

        // 地面接触状態をAnimatorに渡す
        animator.SetBool("IsGrounded", isGrounded);

        // ジャンプトリガー（必要に応じて）
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            animator.SetTrigger("Jump");
        }
    }

    public void WarpTo(Vector3 position)
    {
        // テレポート機能（Rigidbodyベース）
        rb.position = position;
        rb.linearVelocity = Vector3.zero; // 速度をリセット
        rb.angularVelocity = Vector3.zero; // 角速度もリセット
    }

    // デバッグ情報表示用
    private void OnDrawGizmos()
    {
        // 地面チェックの可視化
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * groundCheckDistance);
    }
}
