using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] private LayerMask groundLayerMask = -1; // �n�ʃ��C���[
    private float moveSpeed = 1f;       // �ړ����x
    private float rotationSpeed = 120f; // ��]���x
    private float jumpForce = 3000f;       // �W�����v��
    private float groundCheckDistance = 2f; // �n�ʃ`�F�b�N����
    private float gravityMultiplier = 2f; // �d�͂̔{��

    Rigidbody rb;
    Animator animator; // Animator�R���|�[�l���g

    private bool isGrounded = true; // ���݂̒n�ʂƂ̐ڒn���
    private Vector3 moveDirection; // ���݂̈ړ�����
    private float currentSpeed; // ���݂̈ړ����x

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // �������Z�ł̉�]�𐧌��iY����]�̂݋��j
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        // �W�����v����
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // �A�j���[�V�����̍X�V
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
        // ���͎擾
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // �J�����̌����ɍ��킹���ړ�����
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical);

        // ���͂�����ꍇ�̂ݏ������s��
        if (inputDirection.sqrMagnitude > 0.01f)
        {
            inputDirection = inputDirection.normalized;

            Quaternion cameraRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
            moveDirection = cameraRotation * inputDirection;

            // �ړ����x�̌v�Z�iRigidbody�x�[�X�j
            Vector3 targetVelocity = moveDirection * moveSpeed;

            // ���݂̐������x��ێ����Ȃ��琅���ړ���K�p
            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

            // ���݂̑��x���L�^�iAnimation�p�j
            currentSpeed = inputDirection.magnitude * moveSpeed;
        }
        else
        {
            // ���͂��Ȃ��ꍇ�A���������̑��x���X���[�Y�Ɍ���
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
        // �ړ�����������ꍇ�̂݉�]�������s��
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            // �^�[�Q�b�g�̉�]����
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            // �X���[�Y�ɉ�]�iRigidbody�x�[�X�j
            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newRotation);
        }

        // �p���x�����Z�b�g���ĕs�v�ȉ�]��h��
        rb.angularVelocity = Vector3.zero;
    }

    void Jump()
    {
        // �W�����v�͂�K�p�iRigidbody�x�[�X�j
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false; // �W�����v���͑����ɒn�ʂ��痣�ꂽ�Ƃ݂Ȃ�
    }

    void GroundCheck()
    {
        // �v���C���[�̒��S���牺�����Ƀ��C�L���X�g
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;
        
        bool wasGrounded = isGrounded;

        // ���C�L���X�g�Œn�ʂ��`�F�b�N
        if (Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance, groundLayerMask))
        {
            // �������x���������̏ꍇ�̂ݒ��n����
            if (rb.linearVelocity.y <= 0.1f)
            {
                isGrounded = true;
            }
        }
        else
        {
            isGrounded = false;
        }

        // �f�o�b�O�p�F�n�ʃ`�F�b�N�̃��C������
        Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }

    void ApplyGravity()
    {
        // �J�X�^���d�͂�K�p�iRigidbody�x�[�X�j
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        // �ړ����x��Animator�ɓn���i0-1�͈̔͂ɐ��K���j
        float normalizedSpeed = currentSpeed / moveSpeed;
        animator.SetFloat("Speed", normalizedSpeed);

        // �n�ʐڐG��Ԃ�Animator�ɓn��
        animator.SetBool("IsGrounded", isGrounded);

        // �W�����v�g���K�[�i�K�v�ɉ����āj
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            animator.SetTrigger("Jump");
        }
    }

    public void WarpTo(Vector3 position)
    {
        // �e���|�[�g�@�\�iRigidbody�x�[�X�j
        rb.position = position;
        rb.linearVelocity = Vector3.zero; // ���x�����Z�b�g
        rb.angularVelocity = Vector3.zero; // �p���x�����Z�b�g
    }

    // �f�o�b�O���\���p
    private void OnDrawGizmos()
    {
        // �n�ʃ`�F�b�N�̉���
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * groundCheckDistance);
    }
}
