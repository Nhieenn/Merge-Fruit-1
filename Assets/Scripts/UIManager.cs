using System.Collections; // THÊM DÒNG NÀY ĐỂ DÙNG TÍNH NĂNG CHỜ THỜI GIAN
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public static bool isSoundOn = true;

    [Header("UI Audio")]
    public AudioClip clickSound;
    private AudioSource uiAudioSource;

    [Header("Audio Settings")]
    public AudioSource bgmSource;
    private bool isMusicOn = true;

    // --- CÁC BIẾN UI ---
    private VisualElement hudMain;
    private Label currentScoreLabel;
    private VisualElement nextFruitIcon;
    private VisualElement gameOverScreen;
    private Label finalScoreLabel;
    private Label finalHighScoreLabel;
    private Button restartBtn;
    private Button settingsBtn;
    private VisualElement settingsScreen;
    private Button resumeBtn;
    private Button settingsRestartBtn;
    private Button soundBtn;
    private Button musicBtn;
    private Button quitBtn;
    private VisualElement mainMenuScreen;
    private Button startGameBtn;
    private Button mainQuitBtn;
    private VisualElement creditsScreen;
    private Button creditsBtn;
    private Button closeCreditsBtn;
    private Label mainMenuHighScoreLabel;

    // BỘ NHỚ LƯU MÁY THẢ TRÁI CÂY
    private FruitSpawner gameSpawner;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        uiAudioSource = gameObject.AddComponent<AudioSource>();
        uiAudioSource.playOnAwake = false;
    }

    void Start()
    {
        // Thay vì chạy luôn, ép hệ thống gọi hàm chờ bên dưới
        StartCoroutine(InitUIAfterOneFrame());
    }

    // ĐÂY LÀ PHÉP MÀU: HÀM CHỜ THỜI GIAN
    private IEnumerator InitUIAfterOneFrame()
    {
        // Chờ đúng 1 khung hình (1 frame) để đảm bảo Bản Build tải xong 100% UI
        yield return null;

        UIDocument uiDoc = GetComponent<UIDocument>();
        VisualElement root = uiDoc.rootVisualElement;

        if (root == null)
        {
            Debug.LogError("LỖI KHẨN CẤP: Giao diện chưa được vẽ ra!");
            yield break; // Ngừng chạy nếu vẫn lỗi
        }

        hudMain = root.Q<VisualElement>("hud-main");
        currentScoreLabel = root.Q<Label>("current-score-label");
        nextFruitIcon = root.Q<VisualElement>("next-fruit-icon");

        Label hudHighScoreLabel = root.Q<Label>("high-score-label");
        if (hudHighScoreLabel != null) hudHighScoreLabel.text = "BEST: " + PlayerPrefs.GetInt("HighScore", 0);

        gameOverScreen = root.Q<VisualElement>("game-over-screen");
        finalScoreLabel = root.Q<Label>("final-score-label");
        finalHighScoreLabel = root.Q<Label>("final-high-score-label");
        restartBtn = root.Q<Button>("restart-btn");

        settingsBtn = root.Q<Button>("settings-btn");
        settingsScreen = root.Q<VisualElement>("settings-screen");
        resumeBtn = root.Q<Button>("resume-btn");
        settingsRestartBtn = root.Q<Button>("settings-restart-btn");
        soundBtn = root.Q<Button>("sound-btn");
        musicBtn = root.Q<Button>("music-btn");
        quitBtn = root.Q<Button>("quit-btn");

        mainMenuScreen = root.Q<VisualElement>("main-menu-screen");
        startGameBtn = root.Q<Button>("start-game-btn");
        mainQuitBtn = root.Q<Button>("main-quit-btn");

        creditsScreen = root.Q<VisualElement>("credits-screen");
        creditsBtn = root.Q<Button>("credits-btn");
        closeCreditsBtn = root.Q<Button>("close-credits-btn");

        mainMenuHighScoreLabel = root.Q<Label>("main-menu-highscore");
        if (mainMenuHighScoreLabel != null)
        {
            mainMenuHighScoreLabel.text = "BEST: " + PlayerPrefs.GetInt("HighScore", 0);
        }

        // Gắn sự kiện sau khi chắc chắn các nút đã có mặt
        if (restartBtn != null) restartBtn.clicked += RestartGame;
        if (settingsBtn != null) settingsBtn.clicked += OpenSettings;
        if (resumeBtn != null) resumeBtn.clicked += CloseSettings;
        if (settingsRestartBtn != null) settingsRestartBtn.clicked += RestartGame;
        if (soundBtn != null) soundBtn.clicked += ToggleSound;
        if (musicBtn != null) musicBtn.clicked += ToggleMusic;
        if (quitBtn != null) quitBtn.clicked += QuitGame;
        if (startGameBtn != null) startGameBtn.clicked += StartGameFromMenu;
        if (mainQuitBtn != null) mainQuitBtn.clicked += QuitGame;
        if (creditsBtn != null) creditsBtn.clicked += ShowCredits;
        if (closeCreditsBtn != null) closeCreditsBtn.clicked += CloseCredits;

        isSoundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;

        UpdateSoundState();
        UpdateMusicState();

        if (hudMain != null) hudMain.AddToClassList("hidden");

        // TÌM VÀ LƯU LẠI MÁY THẢ QUẢ ĐỂ XÀI SAU
        gameSpawner = FindFirstObjectByType<FruitSpawner>();
        if (gameSpawner != null) gameSpawner.enabled = false;

        UpdateScore(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (mainMenuScreen != null && !mainMenuScreen.ClassListContains("hidden")) return;

            if (settingsScreen != null)
            {
                if (settingsScreen.ClassListContains("hidden")) OpenSettings();
                else CloseSettings();
            }
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

    private void PlayClickSound()
    {
        if (clickSound != null && uiAudioSource != null && isSoundOn)
        {
            uiAudioSource.PlayOneShot(clickSound);
        }
    }

    private void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        PlayerPrefs.SetInt("SoundOn", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();
        UpdateSoundState();
        PlayClickSound();
    }

    private void UpdateSoundState()
    {
        if (soundBtn != null) soundBtn.text = isSoundOn ? "SOUND: ON" : "SOUND: OFF";
    }

    private void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        PlayerPrefs.SetInt("MusicOn", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();
        UpdateMusicState();
        PlayClickSound();
    }

    private void UpdateMusicState()
    {
        if (bgmSource != null) bgmSource.mute = !isMusicOn;
        if (musicBtn != null) musicBtn.text = isMusicOn ? "MUSIC: ON" : "MUSIC: OFF";
    }

    private void StartGameFromMenu()
    {
        PlayClickSound();
        if (hudMain != null) hudMain.RemoveFromClassList("hidden");
        if (mainMenuScreen != null) mainMenuScreen.AddToClassList("hidden");

        // GỌI TRỰC TIẾP TỪ BỘ NHỚ, KHÔNG TÌM LẠI NỮA
        if (gameSpawner != null) gameSpawner.enabled = true;
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

    private void QuitGame()
    {
        PlayClickSound();
        Application.Quit();
    }

    private void ShowCredits()
    {
        PlayClickSound();
        mainMenuScreen.AddToClassList("hidden");
        if (creditsScreen != null) creditsScreen.RemoveFromClassList("hidden");
    }

    private void CloseCredits()
    {
        PlayClickSound();
        if (creditsScreen != null) creditsScreen.AddToClassList("hidden");
        mainMenuScreen.RemoveFromClassList("hidden");
    }
}