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
    public float minX = -0.527f; // Giới hạn tường bên trái
    public float maxX = 0.507f;  // Giới hạn tường bên phải

    // Biến này sẽ lưu trữ quả đang hiển thị trên ô NEXT của UI
    private GameObject nextFruitPrefab;

    void Start()
    {
        if (trajectoryLine != null) trajectoryLine.positionCount = 2;

        // 1. Vừa vào game, random ngay quả đầu tiên để hiện lên UI
        PrepareNextFruit();

        // 2. Lấy luôn quả vừa random đó đặt lên tay người chơi
        SpawnPreviewFruit();
    }

    void Update()
    {
        if (currentPreviewFruit != null && canDrop)
        {
            TrackMousePosition();
            DrawTrajectoryLine();

            if (Input.GetMouseButtonDown(0))
            {
                currentPreviewFruit.GetComponent<Fruit>().isDropped = true;
                StartCoroutine(DropFruitRoutine());
            }
        }
    }

    void TrackMousePosition()
    {
        Vector3 inputPosition = Input.mousePosition;
        inputPosition.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

        // Thay vì dùng -spawnLimitX và spawnLimitX, ta ép vào đúng 2 tọa độ bạn vừa đo được
        float clampedX = Mathf.Clamp(worldPosition.x, minX, maxX);

        currentPreviewFruit.transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    void DrawTrajectoryLine()
    {
        if (trajectoryLine == null) return;

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

    // HÀM 1: RANDOM QUẢ MỚI VÀ GỬI LÊN UI
    void PrepareNextFruit()
    {
        // Dùng logic mở khóa level xịn xò của bạn
        int maxAllowedLevel = Mathf.Min(GameManager.instance.highestUnlockedLevel, GameManager.instance.maxSpawnableLevelLimit);
        int randomIndex = Random.Range(0, maxAllowedLevel);

        // Lưu quả vừa random lại vào biến nextFruitPrefab
        nextFruitPrefab = allFruitPrefabs[randomIndex];

        // Cập nhật hình ảnh lên UI
        Fruit fruitScript = nextFruitPrefab.GetComponent<Fruit>();
        if (fruitScript != null && fruitScript.fruitIcon != null && UIManager.instance != null)
        {
            UIManager.instance.UpdateNextFruit(fruitScript.fruitIcon);
        }
    }

    // HÀM 2: LẤY QUẢ TỪ UI ĐẶT LÊN TAY
    void SpawnPreviewFruit()
    {
        // Sinh ra ĐÚNG CÁI QUẢ đang nằm chờ ở nextFruitPrefab
        currentPreviewFruit = Instantiate(nextFruitPrefab, transform.position, Quaternion.identity);

        Rigidbody rb = currentPreviewFruit.GetComponent<Rigidbody>();
        Collider col = currentPreviewFruit.GetComponent<Collider>();

        if (rb != null) rb.isKinematic = true;
        if (col != null) col.enabled = false;

        // Vừa bốc quả từ ô NEXT lên tay xong, thì phải lập tức random quả mới bù vào ô NEXT
        PrepareNextFruit();
    }

    IEnumerator DropFruitRoutine()
    {
        canDrop = false;
        trajectoryLine.enabled = false;

        Rigidbody rb = currentPreviewFruit.GetComponent<Rigidbody>();
        Collider col = currentPreviewFruit.GetComponent<Collider>();

        // Thả quả rơi xuống
        if (rb != null) rb.isKinematic = false;
        if (col != null) col.enabled = true;

        currentPreviewFruit = null;

        yield return new WaitForSeconds(spawnDelay);

        // Hết thời gian delay, tự động lấy quả từ ô NEXT đặt lên tay tiếp
        SpawnPreviewFruit();
        canDrop = true;
    }
}