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
        if (GameManager.AutoStartRun)
        {
            GameManager.AutoStartRun = false;
            Play();
            return;
        }

        IsOpen = true;
        if (panel != null) panel.SetActive(true);
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (!IsOpen) return;
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
