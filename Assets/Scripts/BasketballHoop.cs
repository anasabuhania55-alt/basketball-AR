using UnityEngine;

public class BasketballHoop : MonoBehaviour
{
    public int score = 0;

    public void AddScore()
    {
        score++;
        Debug.Log("Score: " + score);
    }
}