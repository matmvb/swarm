using System.Collections;
using UnityEngine;

/// <summary>
/// Menu principal. É a primeira tela do jogo; "Jogar" abre a escolha de piloto.
/// </summary>
public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }

    [SerializeField] GameObject panel;
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
        bool auto = HasArgument("-autostart");
        bool showSelect = HasArgument("-showselect");

        if (GameManager.AutoStartRun || auto || showSelect)
        {
            GameManager.AutoStartRun = false;
            Play();
            if (auto && characterSelect != null)
                characterSelect.AutoChooseFirst();
            if (auto && HasArgument("-showlevelup")) StartCoroutine(ShowLevelUpPreview());
            return;
        }

        IsOpen = true;
        if (panel != null) panel.SetActive(true);
        Time.timeScale = 0f;
    }

    static bool HasArgument(string argument)
    {
        string[] args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
            if (string.Equals(args[i], argument, System.StringComparison.OrdinalIgnoreCase)) return true;

        return false;
    }

    IEnumerator ShowLevelUpPreview()
    {
        yield return null;

        HudController hud = Object.FindAnyObjectByType<HudController>();
        UpgradeSystem upgrades = Object.FindAnyObjectByType<UpgradeSystem>();

        if (hud != null && upgrades != null)
            hud.ShowLevelUp(upgrades.Roll(3));
    }

    void Update()
    {
        if (!IsOpen) return;
        if (OptionsMenu.Instance != null && OptionsMenu.Instance.ClosedThisFrame) return;
        if (OptionsMenu.Instance != null && OptionsMenu.Instance.IsOpen) return;

        if (InputReader.WasPressed(GameKey.Confirm)) Play();
        else if (InputReader.WasPressed(GameKey.Back)) QuitGame();
    }

    void Play()
    {
        IsOpen = false;
        if (panel != null) panel.SetActive(false);
        if (characterSelect != null) characterSelect.Begin();
    }

    public static void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
