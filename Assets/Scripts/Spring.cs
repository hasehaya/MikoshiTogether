using UnityEngine;
using System.Collections;

public class Spring : MonoBehaviour
{
    [Header("Spring Settings")]
    [SerializeField] private float springForce = 10f;
    [SerializeField] private float compressedScaleY = 0.05f; // 圧縮された初期状態のスケール
    
    [Header("Animation Settings")]
    [SerializeField] private float extendScale = 1.5f;    // 伸長時のスケール
    [SerializeField] private float extendDuration = 0.2f;   // 伸長時間
    [SerializeField] private float returnDuration = 0.3f;   // 元に戻る時間
    [SerializeField] private float rechargeDelay = 0.5f;    // 再チャージまでの待機時間
    
    private Vector3 compressedScale;
    private Vector3 originalScale;
    private bool isReady = true; // バネが使用可能かどうか
    private bool isAnimating = false;
    
    private void Start()
    {
        // 初期状態を圧縮された状態に設定
        originalScale = transform.localScale;
        compressedScale = new Vector3(originalScale.x, compressedScaleY, originalScale.z);
        transform.localScale = compressedScale;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // バネが準備完了状態でない場合は反応しない
        if (!isReady || isAnimating) return;
        
        // 衝突したオブジェクトのRigidbodyを取得
        Rigidbody targetRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        
        if (targetRigidbody != null)
        {
            // 跳ね飛ばす関数を実行
            LaunchObject(targetRigidbody);
        }
    }
    
    /// <summary>
    /// オブジェクトを跳ね飛ばし、バネアニメーションを実行する
    /// </summary>
    /// <param name="targetRigidbody">跳ね飛ばす対象のRigidbody</param>
    private void LaunchObject(Rigidbody targetRigidbody)
    {
        // 上方向にAddForceを適用
        targetRigidbody.AddForce(Vector3.up * springForce, ForceMode.Impulse);
        
        // バネアニメーションを開始
        StartCoroutine(SpringReleaseAnimation());
    }
    
    /// <summary>
    /// バネの力を解放するアニメーション
    /// </summary>
    private IEnumerator SpringReleaseAnimation()
    {
        isAnimating = true;
        isReady = false;
        
        // 1. 解放フェーズ（バネが勢いよく伸びる）
        Vector3 extendedScale = new Vector3(originalScale.x, extendScale, originalScale.z);
        yield return StartCoroutine(ScaleTo(extendedScale, extendDuration));
        
        // 2. 元に戻るフェーズ（通常状態に戻る）
        yield return StartCoroutine(ScaleTo(originalScale, returnDuration));
        
        // 3. 再チャージ待機時間
        yield return new WaitForSeconds(rechargeDelay);
        
        // 4. 圧縮状態に戻る（再チャージ）
        yield return StartCoroutine(ScaleTo(compressedScale, returnDuration));
        
        isAnimating = false;
        isReady = true; // バネが再び使用可能になる
    }
    
    /// <summary>
    /// 指定したスケールまで滑らかに変化させる
    /// </summary>
    /// <param name="targetScale">目標スケール</param>
    /// <param name="duration">変化時間</param>
    private IEnumerator ScaleTo(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            
            // イージング効果を適用（滑らかな変化）
            t = Mathf.SmoothStep(0f, 1f, t);
            
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        
        // 最終的な値を確実に設定
        transform.localScale = targetScale;
    }
    
    /// <summary>
    /// バネを手動でリセットする（デバッグ用）
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
