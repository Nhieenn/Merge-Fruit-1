using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    // Biến tĩnh cực kỳ quan trọng: Để các script khác (ScoreManager, Fruit) có thể xin phép trước khi phát tiếng
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

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // Khởi tạo nguồn phát tiếng UI
        uiAudioSource = gameObject.AddComponent<AudioSource>();
        uiAudioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        UIDocument uiDoc = GetComponent<UIDocument>();
        VisualElement root = uiDoc.rootVisualElement;

        // 1. TÌM TẤT CẢ THÀNH PHẦN GIAO DIỆN
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

        // 2. GẮN SỰ KIỆN NÚT BẤM (Gắn 1 lần duy nhất)
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
    }

    void Start()
    {
        // 3. LOAD CÀI ĐẶT ÂM THANH TỪ BỘ NHỚ
        isSoundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;

        UpdateSoundState();
        UpdateMusicState();

        // 4. THIẾT LẬP BAN ĐẦU
        if (hudMain != null) hudMain.AddToClassList("hidden");

        FruitSpawner spawner = FindFirstObjectByType<FruitSpawner>();
        if (spawner != null) spawner.enabled = false;

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

    // --- LOGIC GIAO DIỆN CHUNG ---
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

    // --- LOGIC ÂM THANH (ĐÃ FIX LỖI TÁCH BIỆT) ---
    private void PlayClickSound()
    {
        // Phải kiểm tra isSoundOn trước khi kêu
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
        PlayClickSound(); // Bật lên thì kêu cái click cho vui tai
    }

    private void UpdateSoundState()
    {
        // KHÔNG CÒN LỆNH AudioListener.pause Ở ĐÂY NỮA
        if (soundBtn != null)
        {
            soundBtn.text = isSoundOn ? "SOUND: ON" : "SOUND: OFF";
        }
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
        if (bgmSource != null)
        {
            bgmSource.mute = !isMusicOn;
        }
        if (musicBtn != null)
        {
            musicBtn.text = isMusicOn ? "MUSIC: ON" : "MUSIC: OFF";
        }
    }

    // --- LOGIC NÚT BẤM CƠ BẢN ---
    private void StartGameFromMenu()
    {
        PlayClickSound();
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