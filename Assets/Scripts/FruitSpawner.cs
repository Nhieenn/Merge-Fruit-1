using UnityEngine;
using System.Collections;

public class FruitSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject[] allFruitPrefabs;
    public float spawnLimitX = 2.5f;
    public float spawnDelay = 1f;

    [Header("Trajectory Line")]
    public LineRenderer trajectoryLine;

    private GameObject currentPreviewFruit;
    private bool canDrop = true;

    [Header("Spawn Boundaries")]
    public float minX = -0.527f;
    public float maxX = 0.507f;

    private GameObject nextFruitPrefab;

    void Start()
    {
        if (trajectoryLine != null) trajectoryLine.positionCount = 2;

        // ÉP CHỜ 1 NHỊP ĐỂ ĐẢM BẢO GAMEMANAGER VÀ UIMANAGER ĐÃ SẴN SÀNG
        StartCoroutine(SafeStart());
    }

    IEnumerator SafeStart()
    {
        // Chờ đến khi GameManager và UIManager không còn null nữa
        while (GameManager.instance == null) yield return null;

        PrepareNextFruit();
        yield return new WaitForSeconds(0.1f); // Chờ thêm một tẹo cho chắc
        SpawnPreviewFruit();
    }

    void Update()
    {
        // Phải có quả trên tay VÀ quả đó không bị null mới chạy logic
        if (currentPreviewFruit != null && canDrop)
        {
            TrackMousePosition();
            DrawTrajectoryLine();

            if (Input.GetMouseButtonDown(0))
            {
                // Kiểm tra component Fruit trước khi truy cập
                Fruit fScript = currentPreviewFruit.GetComponent<Fruit>();
                if (fScript != null) fScript.isDropped = true;

                StartCoroutine(DropFruitRoutine());
            }
        }
    }

    void TrackMousePosition()
    {
        if (currentPreviewFruit == null) return;

        Vector3 inputPosition = Input.mousePosition;
        inputPosition.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

        float clampedX = Mathf.Clamp(worldPosition.x, minX, maxX);
        currentPreviewFruit.transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    void DrawTrajectoryLine()
    {
        if (trajectoryLine == null || currentPreviewFruit == null) return;

        trajectoryLine.enabled = true;
        Vector3 startPos = currentPreviewFruit.transform.position;
        trajectoryLine.SetPosition(0, startPos);

        RaycastHit hit;
        if (Physics.Raycast(startPos, Vector3.down, out hit, 20f))
        {
            trajectoryLine.SetPosition(1, hit.point);
        }
        else
        {
            trajectoryLine.SetPosition(1, startPos + Vector3.down * 10f);
        }
    }

    void PrepareNextFruit()
    {
        if (allFruitPrefabs == null || allFruitPrefabs.Length == 0) return;

        // Kiểm tra an toàn cho GameManager
        int maxAllowed = 3; // Mặc định nếu GameManager lỗi
        if (GameManager.instance != null)
        {
            maxAllowed = Mathf.Min(GameManager.instance.highestUnlockedLevel, GameManager.instance.maxSpawnableLevelLimit);
        }

        // Đảm bảo randomIndex không vượt quá độ dài mảng thực tế
        int randomIndex = Random.Range(0, Mathf.Min(maxAllowed, allFruitPrefabs.Length));
        nextFruitPrefab = allFruitPrefabs[randomIndex];

        if (nextFruitPrefab != null)
        {
            Fruit fruitScript = nextFruitPrefab.GetComponent<Fruit>();
            if (fruitScript != null && fruitScript.fruitIcon != null && UIManager.instance != null)
            {
                UIManager.instance.UpdateNextFruit(fruitScript.fruitIcon);
            }
        }
    }

    void SpawnPreviewFruit()
    {
        // Nếu vì lý do gì đó mà nextFruitPrefab bị trống, chạy Prepare ngay
        if (nextFruitPrefab == null) PrepareNextFruit();
        if (nextFruitPrefab == null) return; // Nếu vẫn trống thì chịu, thoát để không crash

        currentPreviewFruit = Instantiate(nextFruitPrefab, transform.position, Quaternion.identity);

        Rigidbody rb = currentPreviewFruit.GetComponent<Rigidbody>();
        Collider col = currentPreviewFruit.GetComponent<Collider>();

        if (rb != null) rb.isKinematic = true;
        if (col != null) col.enabled = false;

        PrepareNextFruit();
    }

    IEnumerator DropFruitRoutine()
    {
        if (currentPreviewFruit == null) yield break;

        canDrop = false;
        if (trajectoryLine != null) trajectoryLine.enabled = false;

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