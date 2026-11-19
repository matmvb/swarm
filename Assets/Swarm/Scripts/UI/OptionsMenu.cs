using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Painel de opções acessível na tela de seleção de piloto (tecla O).
/// </summary>
public class OptionsMenu : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] Text title;
    [SerializeField] Text options;
    [SerializeField] CharacterSelect characterSelect;

    bool _open;
    float _resetConfirmUntil;

    void Start()
    {
        GameOptions.Apply();
        if (panel != null) panel.SetActive(false);
    }

    void Update()
    {
        if (!_open)
        {
            if (characterSelect != null && characterSelect.IsChoosing && InputReader.WasPressed(GameKey.Options))
                Open();
            return;
        }

        if (InputReader.WasPressed(GameKey.Options))
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
        if (panel != null) panel.SetActive(false);
        CharacterSelect.InputBlocked = false;
    }

    void Refresh()
    {
        if (options != null)
            options.text = GameOptions.Describe(Time.unscaledTime < _resetConfirmUntil);
    }
}
