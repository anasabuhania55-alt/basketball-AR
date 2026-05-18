using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class PlaceHoopWithAnchor : MonoBehaviour
{
    public GameObject hoopPrefab;
    public GameUI gameUI;
    public Transform cameraTransform;

    public bool hoopPlaced = false;
    public float lastPlacementTime = -10f;

    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;

    private ARAnchor hoopAnchor;
    private GameObject spawnedHoop;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();

        gameUI.ShowPlacementState();
    }

    void Update()
    {
        if (spawnedHoop != null)
            return;

        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began)
            return;

        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            Vector3 hoopPosition = hitPose.position;

            Vector3 lookDirection = cameraTransform.position - hoopPosition;
            lookDirection.y = 0;

            Quaternion hoopRotation =
                Quaternion.Euler(
                    0f,
                    Quaternion.LookRotation(lookDirection).eulerAngles.y,
                    0f
                );

            GameObject anchorObject = new GameObject("Hoop Anchor");
            anchorObject.transform.position = hoopPosition;
            anchorObject.transform.rotation = hoopRotation;

            hoopAnchor = anchorObject.AddComponent<ARAnchor>();

            spawnedHoop = Instantiate(hoopPrefab, hoopAnchor.transform);
            spawnedHoop.transform.localPosition = Vector3.zero;
            spawnedHoop.transform.localRotation = Quaternion.identity;

            hoopPlaced = true;
            lastPlacementTime = Time.time;

            SetPlanesVisible(false);

            gameUI.ShowGameplayState();
        }
    }

    public void MoveHoop()
    {
        if (spawnedHoop != null)
            Destroy(spawnedHoop);

        if (hoopAnchor != null)
            Destroy(hoopAnchor.gameObject);

        spawnedHoop = null;
        hoopAnchor = null;

        hoopPlaced = false;

        SetPlanesVisible(true);

        gameUI.ShowPlacementState();
    }

    void SetPlanesVisible(bool visible)
    {
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(visible);
        }

        planeManager.enabled = visible;
    }
}