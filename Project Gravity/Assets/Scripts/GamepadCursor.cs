using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

public class GamepadCursor : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private RectTransform cursorTransform;
    [SerializeField] private RectTransform canvasTransform;
    [SerializeField] private Canvas canvas;
    [SerializeField] private float cursorSpeed = 2000f;
    [SerializeField] private float padding = 50f;
    [SerializeField] private Sprite aimCursor;
    [SerializeField] private Sprite regularCursor;
    [SerializeField] private Image cursorImage;
    private Camera _mainCamera;
    public Mouse VirtualMouse;
    private bool _previousMouseState;
    private string _previousControlScheme = "";
    private Mouse _currentMouse;
    
    
    private const string gamePadScheme = "GamePad";
    private const string mouseScheme = "Mouse";

    private void OnEnable()
    {
        _mainCamera = Camera.main;
        _currentMouse = Mouse.current;
        //cursorImage = cursorTransform.gameObject.GetComponent<Image>();
        
        if (VirtualMouse == null)
        {
            VirtualMouse = (Mouse) InputSystem.AddDevice("VirtualMouse");
        }
        else if (!VirtualMouse.added)
        {
            InputSystem.AddDevice(VirtualMouse);
        }

        InputUser.PerformPairingWithDevice(VirtualMouse, playerInput.user);

        if (cursorTransform != null)
        {
            Vector2 position = cursorTransform.anchoredPosition;
            InputState.Change(VirtualMouse.position, position);
        }

        InputSystem.onAfterUpdate += UpdateMotion;
    }

    private void OnDisable()
    {
        if (VirtualMouse != null && VirtualMouse.added)
        {
            InputSystem.RemoveDevice(VirtualMouse);
        }
        InputSystem.onAfterUpdate -= UpdateMotion;
    }

    private void OnControlsChanged(PlayerInput input)
    {
        if (playerInput.currentControlScheme == mouseScheme && _previousControlScheme != mouseScheme)
        {
            cursorTransform.gameObject.SetActive(false);
            Cursor.visible = true;
            _currentMouse.WarpCursorPosition(VirtualMouse.position.ReadValue());
            _previousControlScheme = mouseScheme;
        } else if (playerInput.currentControlScheme == gamePadScheme && _previousControlScheme != gamePadScheme)
        {
            cursorTransform.gameObject.SetActive(true);
            Cursor.visible = false;
            InputState.Change(VirtualMouse.position, _currentMouse.position.ReadValue());
            AnchorCursor(_currentMouse.position.ReadValue());
            _previousControlScheme = gamePadScheme;
        }
    }

    private void UpdateMotion()
    {
        if (VirtualMouse == null || Gamepad.current == null)
        {
            return;
        }

        var stickValue = Gamepad.current.rightStick.ReadValue();
        stickValue *= cursorSpeed * Time.unscaledDeltaTime;

        var currentPosition = VirtualMouse.position.ReadValue();
        var newPosition = currentPosition + stickValue;

        newPosition.x = Mathf.Clamp(newPosition.x, padding, Screen.width - padding);
        newPosition.y = Mathf.Clamp(newPosition.y, padding, Screen.height - padding);

        InputState.Change(VirtualMouse.position, newPosition);
        InputState.Change(VirtualMouse.delta, stickValue);

        var rightTriggerPressed = Gamepad.current.rightTrigger.IsPressed();

        if (_previousMouseState != rightTriggerPressed)
        {
            VirtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, rightTriggerPressed);
            InputState.Change(VirtualMouse, mouseState);
            _previousMouseState = rightTriggerPressed;
        }
        ChangeCursorSprite();
        AnchorCursor(newPosition);
    }

    private void ChangeCursorSprite()
    {
        if (playerInput.currentActionMap.name == "MenuControls")
        {
            cursorImage.sprite = regularCursor;
        } else if (playerInput.currentActionMap.name == "PlayerControls")
        {
            cursorImage.sprite = aimCursor;
        }
    }

    private void AnchorCursor(Vector2 position)
    {
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _mainCamera, out anchoredPosition);

        cursorTransform.anchoredPosition = anchoredPosition;
    }
    
    private void Update()
    {
        if (_previousControlScheme != playerInput.currentControlScheme)
        {
            OnControlsChanged(playerInput);
        }
    
        _previousControlScheme = playerInput.currentControlScheme;
        if (playerInput.currentControlScheme == gamePadScheme)
        {
            if (Cursor.visible)
            {
                Cursor.visible = false;
            }
        } else if (playerInput.currentControlScheme == mouseScheme)
        {
            if (!Cursor.visible)
            {
                Cursor.visible = true;
            }
            
        }
    }
}