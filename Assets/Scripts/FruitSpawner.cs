using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject[] fruitPrefabs;
    public float moveSpeed = 5f;
    public float spawnLimitX = 2.5f;

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        Vector3 newPos = transform.position + Vector3.right * moveInput * moveSpeed * Time.deltaTime;

        newPos.x = Mathf.Clamp(newPos.x, -spawnLimitX, spawnLimitX);
        transform.position = newPos;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            DropFruit();
        }
    }

    void DropFruit()
    {
        if (fruitPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, fruitPrefabs.Length);
            Instantiate(fruitPrefabs[randomIndex], transform.position, Quaternion.identity);
        }
    }
}