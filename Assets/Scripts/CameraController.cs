using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float MIN_FOLLOW_Y_OFFSET = 2.0f;
    private const float MAX_FOLLOW_Y_OFFSET = 12.0f;

    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineTransposer cinemachineTransposer;

    private Vector3 targetFollowOffset;

    private void Awake()
    {
        cinemachineVirtualCamera = GameManager.CinemachineVirtualCamera;
        cinemachineVirtualCamera.Follow = transform;
        cinemachineVirtualCamera.LookAt = transform;

        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector2 inputMoveDir = GameManager.InputManager.GetCameraMoveVector();

        float moveSpeed = 10.0f;
        Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        Vector3 inputRotationVector = Vector3.zero;
        inputRotationVector.y = GameManager.InputManager.GetCameraRotateAmount();

        float rotationSpeed = 100f;
        transform.Rotate(inputRotationVector * rotationSpeed * Time.deltaTime);
    }

    private void HandleZoom()
    {
        float zoomIncreaseAmount = 1.0f;
        targetFollowOffset.y += GameManager.InputManager.GetCameraZoomAmount() * zoomIncreaseAmount;

        float zoomSpeed = 5.0f;
        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
    }
}
