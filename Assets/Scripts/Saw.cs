using UnityEngine;

public class Saw : MonoBehaviour
{
    [Header("��]�ݒ�")]
    public float rotationSpeed = 90f; // X����]���x�i�x/�b�j
    
    [Header("�ړ��ݒ�")]
    public float moveSpeed = 2f; // �ړ����x
    public float amplitude = 3f; // Sin�J�[�u�̐U��
    public float frequency = 1f; // Sin�J�[�u�̎��g��
    
    private Vector3 startPosition;
    private float timeCounter;
    
    void Start()
    {
        // �J�n�ʒu���L�^
        startPosition = transform.position;
        timeCounter = 0f;
    }
    
    void Update()
    {
        // X���ŉ�]
        transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
        
        // ���ԃJ�E���^�[���X�V
        timeCounter += Time.deltaTime * frequency;
        
        // Z������Sin�J�[�u�ňړ�
        float zOffset = Mathf.Sin(timeCounter) * amplitude;
        Vector3 newPosition = startPosition + new Vector3(0, 0, zOffset);
        transform.position = newPosition;
    }
}
