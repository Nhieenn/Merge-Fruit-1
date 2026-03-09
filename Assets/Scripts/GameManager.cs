using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game State")]
    public int highestUnlockedLevel = 1;

    // Giới hạn cấp độ quả rớt xuống (thường game merge chỉ cho rớt tối đa quả cấp 4 hoặc 5 để tránh game quá dễ)
    public int maxSpawnableLevelLimit = 4;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    public void CheckAndUnlockLevel(int newLevel)
    {
        if (newLevel > highestUnlockedLevel)
        {
            highestUnlockedLevel = newLevel;
            Debug.Log("Đã mở khóa quả cấp độ: " + highestUnlockedLevel);
        }
    }
}