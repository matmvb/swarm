using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Painel de opções acessível na tela de seleção de piloto (tecla O).
/// </summary>
public class OptionsMenu : MonoBehaviour
{
    public static OptionsMenu Instance { get; private set; }

    [SerializeField] GameObject panel;
    [SerializeField] Text title;
    [SerializeField] Text options;
    [SerializeField] CharacterSelect characterSelect;

    bool _open;
    float _resetConfirmUntil;

    public bool IsOpen => _open;

    /// <summary>Evita que o ESC que fecha opções também feche o menu de baixo no mesmo frame.</summary>
    public static int ClosedFrame { get; private set; } = -1;
    public bool ClosedThisFrame => ClosedFrame == Time.frameCount;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        GameOptions.Apply();
        ClosedFrame = -1;
        if (panel != null) panel.SetActive(false);
    }

    void Update()
    {
        if (!_open)
        {
            bool menuOpen =
                (MainMenu.Instance != null && MainMenu.Instance.IsOpen) ||
                (PauseMenu.Instance != null && PauseMenu.Instance.IsOpen) ||
                (characterSelect != null && characterSelect.IsChoosing);

            if (menuOpen && InputReader.WasPressed(GameKey.Options)) Open();
            return;
        }

        if (InputReader.WasPressed(GameKey.Options) || InputReader.WasPressed(GameKey.Back))
        {
            Close();
            return;
        }

        if (InputReader.WasPressed(GameKey.VolumeDown))
        {
            GameOptions.Volume -= 0.1f;
            Refresh();
        }

        if (InputReader.WasPressed(GameKey.VolumeUp))
        {
            GameOptions.Volume += 0.1f;
            Refresh();
        }

        if (InputReader.WasPressed(GameKey.Fullscreen))
        {
            GameOptions.ToggleFullscreen();
            Refresh();
        }

        if (InputReader.WasPressed(GameKey.ResetSave))
        {
            if (Time.unscaledTime < _resetConfirmUntil)
            {
                GameOptions.ResetSave();
                _resetConfirmUntil = 0f;
                if (characterSelect != null) characterSelect.RefreshPanel();
            }
            else
            {
                _resetConfirmUntil = Time.unscaledTime + 3f;
            }

            Refresh();
        }
    }

    void Open()
    {
        _open = true;
        _resetConfirmUntil = 0f;
        if (panel != null) panel.SetActive(true);
        if (title != null) title.text = "OPÇÕES";
        CharacterSelect.InputBlocked = true;
        Refresh();
    }

    void Close()
    {
        _open = false;
        _resetConfirmUntil = 0f;
        ClosedFrame = Time.frameCount;
        if (panel != null) panel.SetActive(false);
        CharacterSelect.InputBlocked = false;
    }

    void Refresh()
    {
        if (options != null)
            options.text = GameOptions.Describe(Time.unscaledTime < _resetConfirmUntil);
    }
}
