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
    private Image cursorImage;
    private Camera mainCamera;
    public Mouse virtualMouse;
    private bool previousMouseState;
    private string previousControlScheme = "";
    private Mouse currentMouse;
    
    
    private const string gamePadScheme = "GamePad";
    private const string mouseScheme = "Mouse";

    private void OnEnable()
    {
        mainCamera = Camera.main;
        currentMouse = Mouse.current;
        cursorImage = cursorTransform.gameObject.GetComponent<Image>();
        
        if (virtualMouse == null)
        {
            virtualMouse = (Mouse) InputSystem.AddDevice("VirtualMouse");
        }
        else if (!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }

        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        if (cursorTransform != null)
        {
            Vector2 position = cursorTransform.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }

        InputSystem.onAfterUpdate += UpdateMotion;
    }

    private void OnDisable()
    {
        if (virtualMouse != null && virtualMouse.added)
        {
            InputSystem.RemoveDevice(virtualMouse);
        }
        InputSystem.onAfterUpdate -= UpdateMotion;
    }

    private void OnControlsChanged(PlayerInput input)
    {
        if (playerInput.currentControlScheme == mouseScheme && previousControlScheme != mouseScheme)
        {
            cursorTransform.gameObject.SetActive(false);
            Cursor.visible = true;
            currentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
            previousControlScheme = mouseScheme;
        } else if (playerInput.currentControlScheme == gamePadScheme && previousControlScheme != gamePadScheme)
        {
            cursorTransform.gameObject.SetActive(true);
            Cursor.visible = false;
            InputState.Change(virtualMouse.position, currentMouse.position.ReadValue());
            AnchorCursor(currentMouse.position.ReadValue());
            previousControlScheme = gamePadScheme;
        }
    }

    private void UpdateMotion()
    {
        if (virtualMouse == null || Gamepad.current == null)
        {
            return;
        }

        var stickValue = Gamepad.current.rightStick.ReadValue();
        stickValue *= cursorSpeed * Time.unscaledDeltaTime ;

        var currentPosition = virtualMouse.position.ReadValue();
        var newPosition = currentPosition + stickValue;

        newPosition.x = Mathf.Clamp(newPosition.x, padding, Screen.width - padding);
        newPosition.y = Mathf.Clamp(newPosition.y, padding, Screen.height - padding);

        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.delta, stickValue);

        var rightTriggerPressed = Gamepad.current.rightTrigger.IsPressed();

        if (previousMouseState != rightTriggerPressed)
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, rightTriggerPressed);
            InputState.Change(virtualMouse, mouseState);
            previousMouseState = rightTriggerPressed;
        }
        //ChangeCursorSprite();
        AnchorCursor(newPosition);
        currentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
    }

    // private void ChangeCursorSprite()
    // {
    //     if (playerInput.currentActionMap.name == "MenuControls")
    //     {
    //         cursorImage.sprite = regularCursor;
    //     } else if (playerInput.currentActionMap.name == "PlayerControls")
    //     {
    //         cursorImage.sprite = aimCursor;
    //     }
    // }

    private void AnchorCursor(Vector2 position)
    {
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera, out anchoredPosition);

        cursorTransform.anchoredPosition = anchoredPosition;
    }
    
    private void Update()
    {
        if (previousControlScheme != playerInput.currentControlScheme)
        {
            OnControlsChanged(playerInput);
        }
    
        previousControlScheme = playerInput.currentControlScheme;
        if (playerInput.currentControlScheme == gamePadScheme)
        {
            if (Cursor.visible)
            {
                //Cursor.visible = false;
            }
        } else if (playerInput.currentControlScheme == mouseScheme)
        {
            if (!Cursor.visible)
            {
                //Cursor.visible = true;
            }
            
        }
    }
}