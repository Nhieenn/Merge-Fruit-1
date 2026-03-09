using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private Label mainMenuHighScoreLabel;
    public static UIManager instance;

    [Header("UI Audio")]
    public AudioClip clickSound; // Kéo file âm thanh tiếng "Click" vào đây
    private AudioSource uiAudioSource;

    private VisualElement hudMain; // Khung HUD chứa điểm và Next fruit
    private Label currentScoreLabel;
    private VisualElement nextFruitIcon;

    // Game Over UI
    private VisualElement gameOverScreen;
    private Label finalScoreLabel;
    private Label finalHighScoreLabel;
    private Button restartBtn;

    // Settings UI
    private Button settingsBtn;
    private VisualElement settingsScreen;
    private Button resumeBtn;
    private Button settingsRestartBtn; // Nút Restart mới trong Settings
    private Button soundBtn;
    private Button quitBtn;

    // Main Menu UI
    private VisualElement mainMenuScreen;
    private Button startGameBtn;
    private Button mainQuitBtn;

    private bool isSoundOn = true;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // Tự động tạo một cái loa (AudioSource) để phát tiếng Click UI
        uiAudioSource = gameObject.AddComponent<AudioSource>();
        uiAudioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        UIDocument uiDoc = GetComponent<UIDocument>();
        VisualElement root = uiDoc.rootVisualElement;

        // TÌM HUD CHÍNH
        hudMain = root.Q<VisualElement>("hud-main");
        currentScoreLabel = root.Q<Label>("current-score-label");
        nextFruitIcon = root.Q<VisualElement>("next-fruit-icon");

        Label hudHighScoreLabel = root.Q<Label>("high-score-label");
        if (hudHighScoreLabel != null) hudHighScoreLabel.text = "BEST: " + PlayerPrefs.GetInt("HighScore", 0).ToString();

        // TÌM CÁC THÀNH PHẦN KHÁC
        gameOverScreen = root.Q<VisualElement>("game-over-screen");
        finalScoreLabel = root.Q<Label>("final-score-label");
        finalHighScoreLabel = root.Q<Label>("final-high-score-label");
        restartBtn = root.Q<Button>("restart-btn");

        settingsBtn = root.Q<Button>("settings-btn");
        settingsScreen = root.Q<VisualElement>("settings-screen");
        resumeBtn = root.Q<Button>("resume-btn");
        settingsRestartBtn = root.Q<Button>("settings-restart-btn"); // Tìm nút Restart mới
        soundBtn = root.Q<Button>("sound-btn");
        quitBtn = root.Q<Button>("quit-btn");

        mainMenuScreen = root.Q<VisualElement>("main-menu-screen");
        startGameBtn = root.Q<Button>("start-game-btn");
        mainQuitBtn = root.Q<Button>("main-quit-btn");

        mainMenuHighScoreLabel = root.Q<Label>("main-menu-highscore");

        // Cập nhật điểm kỷ lục ra ngoài Main Menu
        if (mainMenuHighScoreLabel != null)
        {
            mainMenuHighScoreLabel.text = "BEST: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
        }

        // GẮN SỰ KIỆN CLICK (Thêm PlayClickSound vào mỗi hành động)
        if (restartBtn != null) restartBtn.clicked += RestartGame;
        if (settingsBtn != null) settingsBtn.clicked += OpenSettings;
        if (resumeBtn != null) resumeBtn.clicked += CloseSettings;
        if (settingsRestartBtn != null) settingsRestartBtn.clicked += RestartGame; // Nút Restart trong setting cũng gọi hàm RestartGame
        if (soundBtn != null) soundBtn.clicked += ToggleSound;
        if (quitBtn != null) quitBtn.clicked += QuitGame;
        if (startGameBtn != null) startGameBtn.clicked += StartGameFromMenu;
        if (mainQuitBtn != null) mainQuitBtn.clicked += QuitGame;

        // VỪA VÀO GAME: Ẩn HUD, hiện Main Menu, Tắt máy thả quả
        if (hudMain != null) hudMain.AddToClassList("hidden");

        FruitSpawner spawner = FindFirstObjectByType<FruitSpawner>();
        if (spawner != null) spawner.enabled = false;

        UpdateScore(0);
    }

    // --- HÀM PHÁT ÂM THANH CLICK ---
    private void PlayClickSound()
    {
        if (clickSound != null && uiAudioSource != null && isSoundOn)
        {
            uiAudioSource.PlayOneShot(clickSound);
        }
    }

    public void UpdateScore(int newScore)
    {
        if (currentScoreLabel != null) currentScoreLabel.text = newScore.ToString();
    }

    public void UpdateNextFruit(Sprite fruitSprite)
    {
        if (nextFruitIcon != null && fruitSprite != null)
        {
            nextFruitIcon.style.backgroundImage = new StyleBackground(fruitSprite);
            nextFruitIcon.style.backgroundColor = new StyleColor(Color.clear);
        }
    }

    void Update()
    {
        // Lắng nghe phím ESC trên bàn phím
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Nếu đang ở Main Menu thì không cho bật Setting bằng ESC
            if (mainMenuScreen != null && !mainMenuScreen.ClassListContains("hidden")) return;

            // Nếu bảng Setting đang đóng thì mở ra, đang mở thì đóng lại
            if (settingsScreen != null)
            {
                if (settingsScreen.ClassListContains("hidden")) OpenSettings();
                else CloseSettings();
            }
        }
    }

    public void ShowGameOverPanel(int finalScore)
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.RemoveFromClassList("hidden");
            finalScoreLabel.text = "SCORE: " + finalScore;

            int currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
            if (finalScore > currentHighScore)
            {
                currentHighScore = finalScore;
                PlayerPrefs.SetInt("HighScore", currentHighScore);
                PlayerPrefs.Save();
            }
            finalHighScoreLabel.text = "BEST: " + currentHighScore;
        }
    }

    private void StartGameFromMenu()
    {
        PlayClickSound(); // Kêu tiếng Click

        // Bật HUD lên, Ẩn Main Menu đi
        if (hudMain != null) hudMain.RemoveFromClassList("hidden");
        if (mainMenuScreen != null) mainMenuScreen.AddToClassList("hidden");

        FruitSpawner spawner = FindFirstObjectByType<FruitSpawner>();
        if (spawner != null) spawner.enabled = true;
    }

    private void RestartGame()
    {
        PlayClickSound();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OpenSettings()
    {
        PlayClickSound();
        if (settingsScreen != null)
        {
            settingsScreen.RemoveFromClassList("hidden");
            Time.timeScale = 0f;
        }
    }

    private void CloseSettings()
    {
        PlayClickSound();
        if (settingsScreen != null)
        {
            settingsScreen.AddToClassList("hidden");
            Time.timeScale = 1f;
        }
    }

    private void ToggleSound()
    {
        PlayClickSound();
        isSoundOn = !isSoundOn;
        AudioListener.pause = !isSoundOn;

        if (soundBtn != null)
        {
            soundBtn.text = isSoundOn ? "SOUND: ON" : "SOUND: OFF";
        }
    }

    private void QuitGame()
    {
        PlayClickSound();
        Debug.Log("Đang thoát game...");
        Application.Quit();
    }
}