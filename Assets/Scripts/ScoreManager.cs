using UnityEngine;
using UnityEngine.UIElements;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public int currentScore;
    private Label scoreLabel;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        scoreLabel = root.Q<Label>("score-label");
    }

    public void AddScore(int points)
    {
        currentScore += points;
        if (scoreLabel != null)
        {
            scoreLabel.text = "Score: " + currentScore;
        }
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateScore(currentScore);
        }
    }
}