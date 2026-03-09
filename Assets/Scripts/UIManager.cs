using UnityEngine;
using UnityEngine.UIElements; // Bắt buộc phải có thư viện này cho UI Toolkit

public class UIManager : MonoBehaviour
{
    public static UIManager instance; // Thiết lập Singleton để các script khác dễ gọi

    private Label currentScoreLabel;
    private VisualElement nextFruitIcon;

    void Awake()
    {
        // Đảm bảo chỉ có 1 UIManager tồn tại
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void OnEnable()
    {
        // 1. Lấy gốc của giao diện (Document Root)
        UIDocument uiDoc = GetComponent<UIDocument>();
        VisualElement root = uiDoc.rootVisualElement;

        // 2. Tìm các phần tử trên giao diện bằng hàm Q (Query) dựa vào Name bạn đã đặt trong UXML
        currentScoreLabel = root.Q<Label>("current-score-label");
        nextFruitIcon = root.Q<VisualElement>("next-fruit-icon"); // Khung chứa hình quả tiếp theo

        // Reset điểm về 0 khi mới bật game
        UpdateScore(0);
    }

    // Hàm này sẽ được gọi mỗi khi có điểm mới
    public void UpdateScore(int newScore)
    {
        if (currentScoreLabel != null)
        {
            currentScoreLabel.text = newScore.ToString();

            // Mẹo nhỏ: Thêm hiệu ứng giật nhẹ hoặc đổi màu chữ lúc ăn điểm ở đây nếu muốn!
        }
    }

    // Hàm này dùng để đổi hình ảnh quả tiếp theo (Ta sẽ kết nối nó ở bước sau)
    public void UpdateNextFruit(Sprite fruitSprite)
    {
        if (nextFruitIcon != null && fruitSprite != null)
        {
            // Thay đổi ảnh nền của khối Visual Element
            nextFruitIcon.style.backgroundImage = new StyleBackground(fruitSprite);
            // Bỏ màu nền nháp mờ mờ đi để ảnh trái cây hiện ra rõ nét
            nextFruitIcon.style.backgroundColor = new StyleColor(Color.clear);
        }
    }
}