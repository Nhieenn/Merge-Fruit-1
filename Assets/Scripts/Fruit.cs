using UnityEngine;

public class Fruit : MonoBehaviour
{
    public int fruitLevel = 1;
    public GameObject nextLevelPrefab;
    private bool hasMerged = false;

    [Header("Effects")]
    public GameObject mergeEffectPrefab;

    [Header("Audio")]
    public AudioClip dropSound; // Tiếng cộc/bịch khi va chạm
    private AudioSource audioSource;

    [Header("UI Element")]
    public Sprite fruitIcon; // Ảnh 2D để hiển thị lên bảng Next Fruit

    public bool isDropped = false;
    void Awake()
    {
        // Tự động lấy component AudioSource khi quả xuất hiện
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // 1. XỬ LÝ ÂM THANH VA CHẠM (RƠI / ĐẬP VÀO NHAU)
        // Chỉ phát tiếng nếu lực va chạm đủ mạnh (> 1f) để tránh ồn ào khi quả lăn nhẹ
        // Thêm điều kiện audioSource.isActiveAndEnabled vào để check xem loa có đang bật không
        if (audioSource != null && audioSource.isActiveAndEnabled && dropSound != null && collision.relativeVelocity.magnitude > 1f)
        {
            float volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 5f) * 0.5f;
            audioSource.PlayOneShot(dropSound, volume);
        }

        // 2. XỬ LÝ GỘP QUẢ
        if (hasMerged) return;

        Fruit otherFruit = collision.gameObject.GetComponent<Fruit>();

        if (otherFruit != null && otherFruit.fruitLevel == this.fruitLevel && !otherFruit.hasMerged)
        {
            this.hasMerged = true;
            otherFruit.hasMerged = true;

            // Đưa hiệu ứng nổ ra ngoài để dù là gộp quả nhỏ hay quả to nhất thì vẫn nổ bùm
            if (mergeEffectPrefab != null)
            {
                Instantiate(mergeEffectPrefab, transform.position, Quaternion.identity);
            }

            // Kiểm tra xem có quả cấp tiếp theo không
            if (nextLevelPrefab != null)
            {
                // Tình huống gộp bình thường
                ScoreManager.instance.AddScore(fruitLevel * 10);

                int nextLevel = fruitLevel + 1;
                GameManager.instance.CheckAndUnlockLevel(nextLevel);

              
                // Lưu quả vừa tạo ra vào biến newFruit
                GameObject newFruit = Instantiate(nextLevelPrefab, transform.position, Quaternion.identity);

                // Gán isDropped cho quả mới tạo ra, tha cho cái Prefab gốc!
                newFruit.GetComponent<Fruit>().isDropped = true;
            }
            else
            {
                // TÌNH HUỐNG ĐẶC BIỆT: GỘP 2 QUẢ TO NHẤT (Level 11)
                // nextLevelPrefab đang bị bỏ trống (null)
                // Cộng một lượng điểm khổng lồ để thưởng cho người chơi!
                ScoreManager.instance.AddScore(1000);
                Debug.Log("Đã nổ 2 quả to nhất! Thưởng 1000 điểm!");

                // (Sau này bạn có thể tạo 1 prefab hiệu ứng nổ siêu to khổng lồ gắn vào đây)
            }

            // Dọn dẹp cả 2 quả
            Destroy(gameObject);
            Destroy(otherFruit.gameObject);
        }
    }
}