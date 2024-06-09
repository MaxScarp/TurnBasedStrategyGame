#define USE_NEW_INPUT_SYSTEM

using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    public Vector2 GetMouseScreenPosition()
    {
#if USE_NEW_INPUT_SYSTEM
        return Mouse.current.position.value;
#else
        return Input.mousePosition;
#endif
    }

    public bool IsMouseButtonDownThisFrame()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.Click.WasPressedThisFrame();
#else
        Input.GetMouseButtonDown(0);
#endif
    }

    public Vector2 GetCameraMoveVector()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraMovement.ReadValue<Vector2>();
#else
        Vector2 inputMoveDir = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDir.y += 1.0f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            inputMoveDir.y -= 1.0f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDir.x -= 1.0f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            inputMoveDir.x += 1.0f;
        }

        return inputMoveDir;
#endif
    }

    public float GetCameraRotateAmount()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraRotate.ReadValue<float>();
#else
        float rotateAmount = 0.0f;

        if (Input.GetKey(KeyCode.Q))
        {
            rotateAmount = 1.0f;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            rotateAmount = -1.0f;
        }

        return rotateAmount;
#endif
    }

    public float GetCameraZoomAmount()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraZoom.ReadValue<float>();
#else
        float zoomAmount = 0.0f;

        if (Input.mouseScrollDelta.y > 0.0f)
        {
            zoomAmount = -1.0f;
        }
        else if (Input.mouseScrollDelta.y < 0.0f)
        {
            zoomAmount = 1.0f;
        }

        return zoomAmount;
#endif
    }
}
