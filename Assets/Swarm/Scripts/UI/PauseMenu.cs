using UnityEngine;

/// <summary>
/// Pausa durante a run. Esc abre/fecha; O abre as opções por cima.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    [SerializeField] GameObject panel;
    [SerializeField] OptionsMenu options;
    [SerializeField] HudController hud;
    [SerializeField] CharacterSelect characterSelect;

    public bool IsOpen { get; private set; }

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
        if (panel != null) panel.SetActive(false);
    }

    void Update()
    {
        if (MainMenu.Instance != null && MainMenu.Instance.IsOpen) return;
        if (characterSelect != null && characterSelect.IsChoosing) return;
        if (options != null && options.IsOpen) return;

        if (!IsOpen)
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsRunning) return;
            if (hud != null && hud.LevelUpOpen) return;
            if (InputReader.WasPressed(GameKey.Back)) Open();
            return;
        }

        if (InputReader.WasPressed(GameKey.Back)) Resume();
        else if (InputReader.WasPressed(GameKey.Restart)) Restart();
        else if (InputReader.WasPressed(GameKey.MainMenuKey)) ToMainMenu();
        else if (InputReader.WasPressed(GameKey.Quit)) MainMenu.QuitGame();
    }

    void Open()
    {
        IsOpen = true;
        if (panel != null) panel.SetActive(true);
        Time.timeScale = 0f;
    }

    void Resume()
    {
        IsOpen = false;
        if (panel != null) panel.SetActive(false);
        Time.timeScale = 1f;
    }

    void Restart()
    {
        IsOpen = false;
        if (panel != null) panel.SetActive(false);
        if (GameManager.Instance != null) GameManager.Instance.Restart(true);
    }

    void ToMainMenu()
    {
        IsOpen = false;
        if (panel != null) panel.SetActive(false);
        if (GameManager.Instance != null) GameManager.Instance.Restart(false);
    }
}
