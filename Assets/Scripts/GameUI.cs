using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI scoreText;
    public AudioSource scoreAudio;

    public GameObject gameplayButtons;   // Panel: زر Move Hoop
    public GameObject moveButtons;       // Panel: الازرار الاربعة + Done + Reset

    private int score = 0;

    private void Start()
    {
        UpdateScoreUI();
    }

    public void ShowPlacementState()
    {
        instructionText.text = "Move phone to detect surface\nTap to place hoop";
        gameplayButtons.SetActive(false);
        moveButtons.SetActive(false);
    }

    public void ShowGameplayState()
    {
        instructionText.text = "Swipe up to throw the ball\nUse Move Hoop to reposition";
        gameplayButtons.SetActive(true);
        moveButtons.SetActive(false);
    }

    public void ShowMoveState()
    {
        instructionText.text = "Use arrows to position the hoop";
        gameplayButtons.SetActive(false);
        moveButtons.SetActive(true);
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