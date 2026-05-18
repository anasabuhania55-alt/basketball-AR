using UnityEngine;

public class ShootBall : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform cameraTransform;
    public PlaceHoopWithAnchor hoopManager;
    public AudioSource shootAudio;

    public float spawnDistance = 0.35f;
    public float shootDelayAfterPlacement = 0.3f;

    public float minForce = 4f;
    public float maxForce = 12f;
    public float upwardMultiplier = 0.02f;
    public float forwardMultiplier = 0.015f;

    private Vector2 touchStartPos;
    private Vector2 touchEndPos;

    void Update()
    {
        if (!hoopManager.hoopPlaced)
            return;

        if (Time.time < hoopManager.lastPlacementTime + shootDelayAfterPlacement)
            return;

        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            touchStartPos = touch.position;
        }

        if (touch.phase == TouchPhase.Ended)
        {
            touchEndPos = touch.position;

            Vector2 swipe = touchEndPos - touchStartPos;

            if (swipe.y > 50f)
                Shoot(swipe);
        }
    }

    void Shoot(Vector2 swipe)
    {
        Vector3 spawnPosition =
            cameraTransform.position +
            cameraTransform.forward * spawnDistance +
            cameraTransform.up * -0.15f;

        GameObject ball = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);

        Rigidbody rb = ball.GetComponent<Rigidbody>();

        float forwardForce =
            Mathf.Clamp(swipe.magnitude * forwardMultiplier, minForce, maxForce);

        float upwardForce =
            Mathf.Clamp(swipe.y * upwardMultiplier, 1.5f, 5f);

        Vector3 shootDirection =
            cameraTransform.forward * forwardForce +
            Vector3.up * upwardForce;

        rb.AddForce(shootDirection, ForceMode.Impulse);
        rb.AddTorque(cameraTransform.right * 4f, ForceMode.Impulse);

        if (shootAudio != null)
            shootAudio.Play();

        Destroy(ball, 8f);
    }
}