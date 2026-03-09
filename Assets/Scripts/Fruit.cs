using UnityEngine;

public class Fruit : MonoBehaviour
{
    public int fruitLevel = 1;
    public GameObject nextLevelPrefab;
    private bool hasMerged = false;
    [Header("Effects")]
    public GameObject mergeEffectPrefab;
    void OnCollisionEnter(Collision collision)
    {
        if (hasMerged) return;

        Fruit otherFruit = collision.gameObject.GetComponent<Fruit>();

        if (otherFruit != null && otherFruit.fruitLevel == this.fruitLevel && !otherFruit.hasMerged)
        {
            this.hasMerged = true;
            ScoreManager.instance.AddScore(fruitLevel * 10);

            // Tính cấp độ tiếp theo
            int nextLevel = fruitLevel + 1;

            // Báo cho GameManager biết để mở khóa
            GameManager.instance.CheckAndUnlockLevel(nextLevel);

            otherFruit.hasMerged = true;

            if (nextLevelPrefab != null)
            {
                Instantiate(nextLevelPrefab, transform.position, Quaternion.identity);
                // Tạo hiệu ứng nổ ngay tại vị trí gộp quả
                if (mergeEffectPrefab != null)
                {
                    Instantiate(mergeEffectPrefab, transform.position, Quaternion.identity);
                }
            }

            Destroy(gameObject);
            Destroy(otherFruit.gameObject);
        }
    }
}