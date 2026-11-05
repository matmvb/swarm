using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Teclas de interface do jogo (não confundir com movimento).
/// </summary>
public enum GameKey
{
    Digit1,
    Digit2,
    Digit3,
    Restart,
    ToggleDebug
}

/// <summary>
/// Abstração de input: compila e funciona tanto com o Input System novo
/// quanto com o sistema legado (ENABLE_LEGACY_INPUT_MANAGER).
/// </summary>
public static class InputReader
{
    /// <summary>Direção de movimento normalizada (-1..1).</summary>
    public static Vector2 Move
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            var keyboard = Keyboard.current;
            var gamepad = Gamepad.current;
            Vector2 value = Vector2.zero;

            if (keyboard != null)
            {
                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) value.y += 1f;
                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) value.y -= 1f;
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) value.x += 1f;
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) value.x -= 1f;
            }

            if (gamepad != null)
            {
                Vector2 stick = gamepad.leftStick.ReadValue();
                if (stick.sqrMagnitude > value.sqrMagnitude) value = stick;
            }

            return value.sqrMagnitude > 1f ? value.normalized : value;
#else
            Vector2 value = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            return value.sqrMagnitude > 1f ? value.normalized : value;
#endif
        }
    }

    /// <summary>True no frame em que a tecla foi apertada.</summary>
    public static bool WasPressed(GameKey key)
    {
#if ENABLE_INPUT_SYSTEM
        var keyboard = Keyboard.current;
        if (keyboard == null) return false;

        switch (key)
        {
            case GameKey.Digit1:
                return keyboard.digit1Key.wasPressedThisFrame || keyboard.numpad1Key.wasPressedThisFrame;
            case GameKey.Digit2:
                return keyboard.digit2Key.wasPressedThisFrame || keyboard.numpad2Key.wasPressedThisFrame;
            case GameKey.Digit3:
                return keyboard.digit3Key.wasPressedThisFrame || keyboard.numpad3Key.wasPressedThisFrame;
            case GameKey.Restart:
                return keyboard.rKey.wasPressedThisFrame;
            case GameKey.ToggleDebug:
                return keyboard.f1Key.wasPressedThisFrame;
            default:
                return false;
        }
#else
        switch (key)
        {
            case GameKey.Digit1:
                return Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1);
            case GameKey.Digit2:
                return Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2);
            case GameKey.Digit3:
                return Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3);
            case GameKey.Restart:
                return Input.GetKeyDown(KeyCode.R);
            case GameKey.ToggleDebug:
                return Input.GetKeyDown(KeyCode.F1);
            default:
                return false;
        }
#endif
    }
}
