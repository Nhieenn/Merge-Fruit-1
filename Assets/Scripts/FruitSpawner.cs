using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject fruitPrefab; // Quả sẽ được thả
    public float moveSpeed = 5f;   // Tốc độ di chuyển trái/phải
    public float spawnLimitX = 2.5f; // Giới hạn không cho đi quá mép màn hình

    void Update()
    {
        // 1. Xử lý người chơi di chuyển trái/phải
        float moveInput = Input.GetAxis("Horizontal"); // Phím A/D hoặc Mũi tên
        Vector3 newPos = transform.position + Vector3.right * moveInput * moveSpeed * Time.deltaTime;

        // Chặn không cho điểm thả đi quá xa ra ngoài màn hình
        newPos.x = Mathf.Clamp(newPos.x, -spawnLimitX, spawnLimitX);
        transform.position = newPos;

        // 2. Xử lý thả quả khi nhấn phím Space hoặc Click chuột trái
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            DropFruit();
        }
    }

    void DropFruit()
    {
        // Tạo ra quả tại vị trí hiện tại của FruitSpawner
        if (fruitPrefab != null)
        {
            Instantiate(fruitPrefab, transform.position, Quaternion.identity);
        }
    }
}