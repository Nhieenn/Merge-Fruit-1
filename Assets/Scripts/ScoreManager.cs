using UnityEngine;
using UnityEngine.UIElements;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    private int currentScore = 0;
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
    }
}