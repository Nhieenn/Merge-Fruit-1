using UnityEngine;
using System.Collections;

public class FruitSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject[] allFruitPrefabs;
    public float spawnLimitX = 2.5f;
    public float spawnDelay = 1f;

    [Header("Trajectory Line")]
    public LineRenderer trajectoryLine; // Kéo Line Renderer vào đây

    private GameObject currentPreviewFruit;
    private bool canDrop = true;

    void Start()
    {
        // Khởi tạo số điểm của đường kẻ là 2 (Điểm đầu và Điểm cuối)
        if (trajectoryLine != null) trajectoryLine.positionCount = 2;
        SpawnPreviewFruit();
    }

    void Update()
    {
        if (currentPreviewFruit != null && canDrop)
        {
            TrackMousePosition();
            DrawTrajectoryLine(); // Vẽ đường kẻ mỗi frame

            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(DropFruitRoutine());
            }
        }
    }

    void TrackMousePosition()
    {
        Vector3 inputPosition = Input.mousePosition;
        inputPosition.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

        float clampedX = Mathf.Clamp(worldPosition.x, -spawnLimitX, spawnLimitX);
        currentPreviewFruit.transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    void DrawTrajectoryLine()
    {
        if (trajectoryLine == null) return;

        trajectoryLine.enabled = true;
        Vector3 startPos = currentPreviewFruit.transform.position;
        trajectoryLine.SetPosition(0, startPos);

        // Bắn tia Raycast thẳng xuống để tìm điểm chạm
        RaycastHit hit;
        if (Physics.Raycast(startPos, Vector3.down, out hit, 20f))
        {
            trajectoryLine.SetPosition(1, hit.point); // Dừng lại ở điểm chạm
        }
        else
        {
            trajectoryLine.SetPosition(1, startPos + Vector3.down * 10f); // Dừng ở đáy (dự phòng)
        }
    }

    void SpawnPreviewFruit()
    {
        int maxAllowedLevel = Mathf.Min(GameManager.instance.highestUnlockedLevel, GameManager.instance.maxSpawnableLevelLimit);
        int randomIndex = Random.Range(0, maxAllowedLevel);

        currentPreviewFruit = Instantiate(allFruitPrefabs[randomIndex], transform.position, Quaternion.identity);

        Rigidbody rb = currentPreviewFruit.GetComponent<Rigidbody>();
        Collider col = currentPreviewFruit.GetComponent<Collider>();

        if (rb != null) rb.isKinematic = true;
        if (col != null) col.enabled = false;  // Tắt va chạm để tia Raycast ngắm bắn xuyên qua quả lơ lửng được
    }

    IEnumerator DropFruitRoutine()
    {
        canDrop = false;
        trajectoryLine.enabled = false; // Ẩn đường kẻ khi đang thả

        Rigidbody rb = currentPreviewFruit.GetComponent<Rigidbody>();
        Collider col = currentPreviewFruit.GetComponent<Collider>();

        if (rb != null) rb.isKinematic = false;
        if (col != null) col.enabled = true;

        currentPreviewFruit = null;

        yield return new WaitForSeconds(spawnDelay);

        SpawnPreviewFruit();
        canDrop = true;
    }
}