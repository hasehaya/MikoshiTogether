using UnityEngine;

public class Hammer : MonoBehaviour
{
    [Header("�U��q�ݒ�")]
    [SerializeField] private float pendulumLength = 1f; // �U��q�̘r�̒���
    [SerializeField] private float gravity = 9.81f; // �d�͂̋���
    [SerializeField] private float maxAngle = 70f; // �ő�U��p�x�i�x�j
    
    private float currentAngle = 0f; // ���݂̊p�x�i�x�j
    private float angularVelocity = 0f; // �p���x�i�x/�b�j
    
    void Start()
    {
        // �U��q�^�����J�n���邽�߁A�����p�x��ݒ�
        currentAngle = maxAngle;
    }
    
    void Update()
    {
        // �U��q�̕����v�Z�����s
        UpdatePendulumPhysics();
        
        // Transform�ɉ�]��K�p
        ApplyRotation();
    }
    
    private void UpdatePendulumPhysics()
    {
        // �����v�Z�̂��ߊp�x�����W�A���ɕϊ�
        float angleInRadians = currentAngle * Mathf.Deg2Rad;
        
        // �U��q�̕��������g�p���Ċp�����x���v�Z
        // �p�����x = -(�d�� / ����) * sin(�p�x)
        float angularAcceleration = -(gravity / pendulumLength) * Mathf.Sin(angleInRadians);
        
        // �x���b���b�ɕϊ�
        angularAcceleration *= Mathf.Rad2Deg;
        
        // �p���x���X�V
        angularVelocity += angularAcceleration * Time.deltaTime;
        
        // ���݂̊p�x���X�V
        currentAngle += angularVelocity * Time.deltaTime;
        
        // �ő�U��p�x�𒴂��Ȃ��悤�ɐ���
        if (currentAngle > maxAngle)
        {
            currentAngle = maxAngle;
            angularVelocity = -Mathf.Abs(angularVelocity); // �����𔽓]
        }
        else if (currentAngle < -maxAngle)
        {
            currentAngle = -maxAngle;
            angularVelocity = Mathf.Abs(angularVelocity); // �����𔽓]
        }
    }
    
    private void ApplyRotation()
    {
        // Z������̉�]��K�p
        transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }
    
    // �I�v�V����: ����̊p�x�ŐU��q���蓮�J�n���郁�\�b�h
    public void StartPendulum(float initialAngle = 70f)
    {
        currentAngle = Mathf.Clamp(initialAngle, -maxAngle, maxAngle);
        angularVelocity = 0f;
    }
    
    // �I�v�V����: �U��q���~���郁�\�b�h
    public void StopPendulum()
    {
        angularVelocity = 0f;
    }
}
