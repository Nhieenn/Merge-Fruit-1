using UnityEngine;

public class GameOver : MonoBehaviour
{
    public float timeToLose = 3f;
    private float timer = 0f;

    [Header("Visuals")]
    public float warningDistance = 1.5f;

    private MeshRenderer meshRenderer;
    private Material warningMaterial;
    private float deathY;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            // Tự động sao chép Material để không làm hỏng file gốc
            warningMaterial = meshRenderer.material;
            meshRenderer.enabled = false; // Mới vào game thì tàng hình nắp đi
        }
    }

    void Start()
    {
        deathY = transform.position.y;
    }

    void Update()
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

            if (distance <= 0)
            {
                SetWarningVisual(0.8f); // Đỏ đậm cảnh báo
                timer += Time.deltaTime;
                if (timer >= timeToLose)
                {
                    Debug.Log("Game Over!");
                    Time.timeScale = 0;
                }
            }
            else if (distance <= warningDistance)
            {
                timer = 0f;
                // Nhấp nháy mượt mà từ mờ sang vừa
                float alpha = Mathf.PingPong(Time.time * 2f, 0.4f) + 0.1f;
                SetWarningVisual(alpha);
            }
            else
            {
                timer = 0f;
                if (meshRenderer != null) meshRenderer.enabled = false;
            }
        }
    }

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
}