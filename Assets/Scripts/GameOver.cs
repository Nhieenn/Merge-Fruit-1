using UnityEngine;
using System.Collections.Generic;

public class GameOver : MonoBehaviour
{
    [Header("Game Over Settings")]
    public float timeToLose = 3f;
    private float timer = 0f;
    private bool isGameOver = false;

    [Header("Visuals")]
    public float warningDistance = 1.5f;
    private MeshRenderer meshRenderer;
    private Material warningMaterial;
    private float deathY;

    // Danh sách theo dõi các quả chạm vạch (Tối ưu hơn rất nhiều so với FindGameObjectsWithTag)
    private List<Collider> fruitsInZone = new List<Collider>();

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            warningMaterial = meshRenderer.material;
            meshRenderer.enabled = false; // Tàng hình khi mới vào
        }
    }

    void Start()
    {
        deathY = transform.position.y;
    }

    void Update()
    {
        if (isGameOver) return;

        // Dọn dẹp danh sách lỡ có quả nào bị xóa do gộp lại
        fruitsInZone.RemoveAll(item => item == null);

        // TRƯỜNG HỢP 1: Có quả đang chạm vạch (Nguy kịch)
        if (fruitsInZone.Count > 0)
        {
            SetWarningVisual(0.8f); // Đỏ đậm như code cũ của bạn
            timer += Time.deltaTime;

            if (timer >= timeToLose)
            {
                TriggerGameOver();
            }
        }
        // TRƯỜNG HỢP 2: Không chạm vạch, kiểm tra xem có ai ở gần không (Cảnh báo)
        else
        {
            timer = 0f; // Reset đồng hồ
            CheckWarningDistance();
        }
    }

    // Hàm cảnh báo đã được cách ly, chỉ chạy khi không có quả nào chạm vạch
    void CheckWarningDistance()
    {
        GameObject[] fruits = GameObject.FindGameObjectsWithTag("Fruit");
        float highestFruitY = -100f;

        foreach (var f in fruits)
        {
            Rigidbody rb = f.GetComponent<Rigidbody>();
            if (rb != null && rb.isKinematic) continue;

            if (f.transform.position.y > highestFruitY)
            {
                highestFruitY = f.transform.position.y;
            }
        }

        if (highestFruitY != -100f)
        {
            float distance = deathY - highestFruitY;

            if (distance > 0 && distance <= warningDistance)
            {
                // Hiệu ứng nhấp nháy mượt mà của bạn
                float alpha = Mathf.PingPong(Time.time * 2f, 0.4f) + 0.1f;
                SetWarningVisual(alpha);
            }
            else
            {
                if (meshRenderer != null) meshRenderer.enabled = false;
            }
        }
    }

    // --- HỆ THỐNG CẢM BIẾN VẬT LÝ ---
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Fruit>() != null && !fruitsInZone.Contains(other))
        {
            fruitsInZone.Add(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (fruitsInZone.Contains(other))
        {
            fruitsInZone.Remove(other);
        }
    }

    // --- HÀM HỖ TRỢ ---
    void SetWarningVisual(float alpha)
    {
        if (meshRenderer != null && warningMaterial != null)
        {
            meshRenderer.enabled = true;
            Color c = warningMaterial.color;
            c.a = alpha;
            warningMaterial.color = c;
        }
    }

    void TriggerGameOver()
    {
        isGameOver = true;

        // Tắt máy thả quả
        FruitSpawner spawner = FindFirstObjectByType<FruitSpawner>();
        if (spawner != null) spawner.enabled = false;

        // Dừng thời gian game
        Time.timeScale = 0;

        // GỌI BẢNG UI HIỆN LÊN
        if (UIManager.instance != null && ScoreManager.instance != null)
        {
            UIManager.instance.ShowGameOverPanel(ScoreManager.instance.currentScore);
        }
    }
}