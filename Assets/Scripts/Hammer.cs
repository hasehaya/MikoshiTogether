using UnityEngine;

public class Hammer : MonoBehaviour
{
    [Header("U‚èŽqÝ’è")]
    [SerializeField] private float pendulumLength = 1f; // U‚èŽq‚Ì˜r‚Ì’·‚³
    [SerializeField] private float gravity = 9.81f; // d—Í‚Ì‹­‚³
    [SerializeField] private float maxAngle = 70f; // Å‘åU‚èŠp“xi“xj
    
    private float currentAngle = 0f; // Œ»Ý‚ÌŠp“xi“xj
    private float angularVelocity = 0f; // Šp‘¬“xi“x/•bj
    
    void Start()
    {
        // U‚èŽq‰^“®‚ðŠJŽn‚·‚é‚½‚ßA‰ŠúŠp“x‚ðÝ’è
        currentAngle = maxAngle;
    }
    
    void Update()
    {
        // U‚èŽq‚Ì•¨—ŒvŽZ‚ðŽÀs
        UpdatePendulumPhysics();
        
        // Transform‚É‰ñ“]‚ð“K—p
        ApplyRotation();
    }
    
    private void UpdatePendulumPhysics()
    {
        // •¨—ŒvŽZ‚Ì‚½‚ßŠp“x‚ðƒ‰ƒWƒAƒ“‚É•ÏŠ·
        float angleInRadians = currentAngle * Mathf.Deg2Rad;
        
        // U‚èŽq‚Ì•¨—Ž®‚ðŽg—p‚µ‚ÄŠp‰Á‘¬“x‚ðŒvŽZ
        // Šp‰Á‘¬“x = -(d—Í / ’·‚³) * sin(Šp“x)
        float angularAcceleration = -(gravity / pendulumLength) * Mathf.Sin(angleInRadians);
        
        // “x–ˆ•b–ˆ•b‚É•ÏŠ·
        angularAcceleration *= Mathf.Rad2Deg;
        
        // Šp‘¬“x‚ðXV
        angularVelocity += angularAcceleration * Time.deltaTime;
        
        // Œ»Ý‚ÌŠp“x‚ðXV
        currentAngle += angularVelocity * Time.deltaTime;
        
        // Å‘åU‚èŠp“x‚ð’´‚¦‚È‚¢‚æ‚¤‚É§ŒÀ
        if (currentAngle > maxAngle)
        {
            currentAngle = maxAngle;
            angularVelocity = -Mathf.Abs(angularVelocity); // •ûŒü‚ð”½“]
        }
        else if (currentAngle < -maxAngle)
        {
            currentAngle = -maxAngle;
            angularVelocity = Mathf.Abs(angularVelocity); // •ûŒü‚ð”½“]
        }
    }
    
    private void ApplyRotation()
    {
        // ZŽ²Žü‚è‚Ì‰ñ“]‚ð“K—p
        transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }
    
    // ƒIƒvƒVƒ‡ƒ“: “Á’è‚ÌŠp“x‚ÅU‚èŽq‚ðŽè“®ŠJŽn‚·‚éƒƒ\ƒbƒh
    public void StartPendulum(float initialAngle = 70f)
    {
        currentAngle = Mathf.Clamp(initialAngle, -maxAngle, maxAngle);
        angularVelocity = 0f;
    }
    
    // ƒIƒvƒVƒ‡ƒ“: U‚èŽq‚ð’âŽ~‚·‚éƒƒ\ƒbƒh
    public void StopPendulum()
    {
        angularVelocity = 0f;
    }
}
