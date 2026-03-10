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
        // ĐÃ THÊM: Kiểm tra UIManager.isSoundOn trước khi cho phép phát tiếng!
        if (UIManager.isSoundOn && audioSource != null && audioSource.isActiveAndEnabled && dropSound != null && collision.relativeVelocity.magnitude > 1f)
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
                // Bắt lấy cái hiệu ứng vừa được sinh ra gán vào biến 'effect'
                GameObject effect = Instantiate(mergeEffectPrefab, transform.position, Quaternion.identity);

                // TÌM VÀ KIỂM SOÁT CÁI LOA TRÊN HIỆU ỨNG ĐÓ
                AudioSource effectAudio = effect.GetComponent<AudioSource>();
                if (effectAudio != null)
                {
                    // Nếu tắt Sound thì mute cái loa này đi
                    effectAudio.mute = !UIManager.isSoundOn;
                }
            }

            // Kiểm tra xem có quả cấp tiếp theo không
            if (nextLevelPrefab != null)
            {
                // Tình huống gộp bình thường
                ScoreManager.instance.AddScore(fruitLevel * 10);

                int nextLevel = fruitLevel + 1;
                GameManager.instance.CheckAndUnlockLevel(nextLevel);

                GameObject newFruit = Instantiate(nextLevelPrefab, transform.position, Quaternion.identity);

                // Gán isDropped cho quả mới tạo ra, tha cho cái Prefab gốc!
                newFruit.GetComponent<Fruit>().isDropped = true;

                // KIỂM SOÁT LOA CỦA QUẢ MỚI
                AudioSource newFruitAudio = newFruit.GetComponent<AudioSource>();
                if (newFruitAudio != null)
                {
                    newFruitAudio.mute = !UIManager.isSoundOn;
                }
            }
            else
            {
                // TÌNH HUỐNG ĐẶC BIỆT: GỘP 2 QUẢ TO NHẤT (Level 11)
                ScoreManager.instance.AddScore(1000);
                Debug.Log("Đã nổ 2 quả to nhất! Thưởng 1000 điểm!");
            }

            // Dọn dẹp cả 2 quả
            Destroy(gameObject);
            Destroy(otherFruit.gameObject);
        }
    }
}