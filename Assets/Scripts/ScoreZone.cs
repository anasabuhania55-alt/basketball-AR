using UnityEngine;


public class ScoreZone : MonoBehaviour
{
    private GameUI gameUI;

    private void Start()
    {
            gameUI = FindAnyObjectByType<GameUI>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball"))
            return;

        BallScoreState state = other.GetComponent<BallScoreState>();

        if (state == null)
            return;

        if (state.hasScored)
            return;

        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb != null && rb.linearVelocity.y < -0.2f)
        {
            state.hasScored = true;
            gameUI.AddScore();
        }
    }
}