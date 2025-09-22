using UnityEngine;

public class Saw : MonoBehaviour
{
    [Header("回転設定")]
    public float rotationSpeed = 90f; // X軸回転速度（度/秒）
    
    [Header("移動設定")]
    public float moveSpeed = 2f; // 移動速度
    public float amplitude = 3f; // Sinカーブの振幅
    public float frequency = 1f; // Sinカーブの周波数
    
    private Vector3 startPosition;
    private float timeCounter;
    
    void Start()
    {
        // 開始位置を記録
        startPosition = transform.position;
        timeCounter = 0f;
    }
    
    void Update()
    {
        // X軸で回転
        transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
        
        // 時間カウンターを更新
        timeCounter += Time.deltaTime * frequency;
        
        // Z方向にSinカーブで移動
        float zOffset = Mathf.Sin(timeCounter) * amplitude;
        Vector3 newPosition = startPosition + new Vector3(0, 0, zOffset);
        transform.position = newPosition;
    }
}
