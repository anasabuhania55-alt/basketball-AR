using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI scoreText;
    public AudioSource scoreAudio;

    private int score = 0;

    private void Start()
    {
        UpdateScoreUI();
    }

    public void ShowPlacementState()
    {
        instructionText.text =
            "Move phone to detect surface\nTap to place hoop";
    }

    public void ShowGameplayState()
    {
        instructionText.text =
            "Swipe up to throw the ball\nUse Move Hoop to reposition";
    }

    public void AddScore()
    {
        score++;
        UpdateScoreUI();

        if (scoreAudio != null)
            scoreAudio.Play();
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }

}