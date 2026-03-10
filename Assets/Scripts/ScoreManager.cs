using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public int currentScore;
    public int highScore;

    [Header("High Score Effects")]
    public AudioClip highScoreSound;
    private AudioSource audioSource;
    private bool hasCelebrated = false;

    private Label currentScoreLabel;
    private Label highScoreLabel;

    void Awake()
    {
        instance = this;
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        currentScoreLabel = root.Q<Label>("current-score-label");
        highScoreLabel = root.Q<Label>("high-score-label");

        ResetScoreUI();
    }

    public void ResetScoreUI()
    {
        currentScore = 0;
        hasCelebrated = false;

        if (currentScoreLabel != null) currentScoreLabel.text = currentScore.ToString();

        if (highScoreLabel != null)
        {
            highScoreLabel.text = "BEST: " + highScore;
            highScoreLabel.RemoveFromClassList("highscore-gold");
            highScoreLabel.RemoveFromClassList("highscore-pulse");
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;

        if (currentScoreLabel != null) currentScoreLabel.text = currentScore.ToString();

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();

            if (highScoreLabel != null) highScoreLabel.text = "BEST: " + highScore;

            if (!hasCelebrated)
            {
                hasCelebrated = true;
                CelebrateHighScore();
            }
        }

        if (UIManager.instance != null) UIManager.instance.UpdateScore(currentScore);
    }

    private void CelebrateHighScore()
    {
        // ĐÃ SỬA CHỖ NÀY: Kiểm tra biến UIManager.isSoundOn
        if (audioSource != null && highScoreSound != null && UIManager.isSoundOn)
        {
            audioSource.PlayOneShot(highScoreSound);
        }

        if (highScoreLabel != null)
        {
            highScoreLabel.AddToClassList("highscore-gold");
            highScoreLabel.AddToClassList("highscore-pulse");
            StartCoroutine(ResetPulseEffect());
        }
    }

    private IEnumerator ResetPulseEffect()
    {
        yield return new WaitForSeconds(0.3f);
        if (highScoreLabel != null)
        {
            highScoreLabel.RemoveFromClassList("highscore-pulse");
        }
    }
}