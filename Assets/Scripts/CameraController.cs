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
        Vector3 inputMoveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDir.z += 1.0f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            inputMoveDir.z -= 1.0f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDir.x -= 1.0f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            inputMoveDir.x += 1.0f;
        }

        float moveSpeed = 10.0f;
        Vector3 moveVector = transform.forward * inputMoveDir.z + transform.right * inputMoveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        Vector3 inputRotationVector = Vector3.zero;
        if (Input.GetKey(KeyCode.Q))
        {
            inputRotationVector.y += 1.0f;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            inputRotationVector.y -= 1.0f;
        }

        float rotationSpeed = 100f;
        transform.Rotate(inputRotationVector * rotationSpeed * Time.deltaTime);
    }

    private void HandleZoom()
    {
        if (Input.mouseScrollDelta.y > 0.0f)
        {
            targetFollowOffset.y -= 1.0f;
        }
        else if (Input.mouseScrollDelta.y < 0.0f)
        {
            targetFollowOffset.y += 1.0f;
        }

        float zoomSpeed = 5.0f;
        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
    }
}
