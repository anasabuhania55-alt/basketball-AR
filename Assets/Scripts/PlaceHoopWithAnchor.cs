using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlaceHoopWithAnchor : MonoBehaviour
{
    public GameObject hoopPrefab;
    public GameUI gameUI;
    public Transform cameraTransform;

    public bool hoopPlaced = false;
    public float lastPlacementTime = -10f;
    public float moveStep = 0.15f;

    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private ARAnchorManager anchorManager;

    private ARAnchor hoopAnchor;
    private GameObject spawnedHoop;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
        anchorManager = GetComponent<ARAnchorManager>();

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

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;

        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            Vector3 hoopPosition = hitPose.position;

            Vector3 lookDirection = cameraTransform.position - hoopPosition;
            lookDirection.y = 0;

            Quaternion hoopRotation = Quaternion.Euler(
                0f,
                Quaternion.LookRotation(lookDirection).eulerAngles.y,
                0f
            );

            ARPlane hitPlane = planeManager.GetPlane(hits[0].trackableId);

            if (hitPlane != null && anchorManager != null)
            {
                hoopAnchor = anchorManager.AttachAnchor(hitPlane, new Pose(hoopPosition, hoopRotation));
            }

            if (hoopAnchor == null)
            {
                GameObject anchorObject = new GameObject("Hoop Anchor");
                anchorObject.transform.position = hoopPosition;
                anchorObject.transform.rotation = hoopRotation;
                hoopAnchor = anchorObject.AddComponent<ARAnchor>();
            }

            spawnedHoop = Instantiate(hoopPrefab, hoopAnchor.transform);
            spawnedHoop.transform.localPosition = Vector3.zero;
            spawnedHoop.transform.localRotation = Quaternion.identity;

            hoopPlaced = true;
            lastPlacementTime = Time.time;

            SetPlanesVisible(false);
            gameUI.ShowGameplayState();
        }
    }

    // زر Move Hoop — يطلع ازرار التحريك
    public void MoveHoop()
    {
        if (spawnedHoop == null) return;
        gameUI.ShowMoveState();
    }

    // ازرار الاتجاهات
    public void MoveForward()  { ShiftHoop(Vector3.forward); }
    public void MoveBackward() { ShiftHoop(Vector3.back);    }
    public void MoveLeft()     { ShiftHoop(Vector3.left);    }
    public void MoveRight()    { ShiftHoop(Vector3.right);   }

    void ShiftHoop(Vector3 localDir)
    {
        if (hoopAnchor == null) return;

        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 worldDir = camForward * localDir.z + camRight * localDir.x;
        hoopAnchor.transform.position += worldDir * moveStep;
    }

    // زر Done — يرجع للعبة
    public void ConfirmPosition()
    {
        gameUI.ShowGameplayState();
    }

    // زر Reset — يمسح السلة ويبدأ من جديد
    public void ResetHoop()
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
            plane.gameObject.SetActive(visible);

        planeManager.enabled = visible;
    }
}