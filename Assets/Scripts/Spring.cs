using UnityEngine;
using System.Collections;

public class Spring : MonoBehaviour
{
    [Header("Spring Settings")]
    [SerializeField] private float springForce = 10f;
    [SerializeField] private float compressedScaleY = 0.05f; // ���k���ꂽ������Ԃ̃X�P�[��
    
    [Header("Animation Settings")]
    [SerializeField] private float extendScale = 1.5f;    // �L�����̃X�P�[��
    [SerializeField] private float extendDuration = 0.2f;   // �L������
    [SerializeField] private float returnDuration = 0.3f;   // ���ɖ߂鎞��
    [SerializeField] private float rechargeDelay = 0.5f;    // �ă`���[�W�܂ł̑ҋ@����
    
    private Vector3 compressedScale;
    private Vector3 originalScale;
    private bool isReady = true; // �o�l���g�p�\���ǂ���
    private bool isAnimating = false;
    
    private void Start()
    {
        // ������Ԃ����k���ꂽ��Ԃɐݒ�
        originalScale = transform.localScale;
        compressedScale = new Vector3(originalScale.x, compressedScaleY, originalScale.z);
        transform.localScale = compressedScale;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // �o�l������������ԂłȂ��ꍇ�͔������Ȃ�
        if (!isReady || isAnimating) return;
        
        // �Փ˂����I�u�W�F�N�g��Rigidbody���擾
        Rigidbody targetRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        
        if (targetRigidbody != null)
        {
            // ���˔�΂��֐������s
            LaunchObject(targetRigidbody);
        }
    }
    
    /// <summary>
    /// �I�u�W�F�N�g�𒵂˔�΂��A�o�l�A�j���[�V���������s����
    /// </summary>
    /// <param name="targetRigidbody">���˔�΂��Ώۂ�Rigidbody</param>
    private void LaunchObject(Rigidbody targetRigidbody)
    {
        // �������AddForce��K�p
        targetRigidbody.AddForce(Vector3.up * springForce, ForceMode.Impulse);
        
        // �o�l�A�j���[�V�������J�n
        StartCoroutine(SpringReleaseAnimation());
    }
    
    /// <summary>
    /// �o�l�̗͂��������A�j���[�V����
    /// </summary>
    private IEnumerator SpringReleaseAnimation()
    {
        isAnimating = true;
        isReady = false;
        
        // 1. ����t�F�[�Y�i�o�l�������悭�L�т�j
        Vector3 extendedScale = new Vector3(originalScale.x, extendScale, originalScale.z);
        yield return StartCoroutine(ScaleTo(extendedScale, extendDuration));
        
        // 2. ���ɖ߂�t�F�[�Y�i�ʏ��Ԃɖ߂�j
        yield return StartCoroutine(ScaleTo(originalScale, returnDuration));
        
        // 3. �ă`���[�W�ҋ@����
        yield return new WaitForSeconds(rechargeDelay);
        
        // 4. ���k��Ԃɖ߂�i�ă`���[�W�j
        yield return StartCoroutine(ScaleTo(compressedScale, returnDuration));
        
        isAnimating = false;
        isReady = true; // �o�l���Ăюg�p�\�ɂȂ�
    }
    
    /// <summary>
    /// �w�肵���X�P�[���܂Ŋ��炩�ɕω�������
    /// </summary>
    /// <param name="targetScale">�ڕW�X�P�[��</param>
    /// <param name="duration">�ω�����</param>
    private IEnumerator ScaleTo(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            
            // �C�[�W���O���ʂ�K�p�i���炩�ȕω��j
            t = Mathf.SmoothStep(0f, 1f, t);
            
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        
        // �ŏI�I�Ȓl���m���ɐݒ�
        transform.localScale = targetScale;
    }
    
    /// <summary>
    /// �o�l���蓮�Ń��Z�b�g����i�f�o�b�O�p�j
    /// </summary>
    [ContextMenu("Reset Spring")]
    private void ResetSpring()
    {
        if (!isAnimating)
        {
            StopAllCoroutines();
            transform.localScale = compressedScale;
            isReady = true;
            isAnimating = false;
        }
    }
}
