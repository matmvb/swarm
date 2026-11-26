using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Card de piloto com emblema, nome e descrição curta.
/// </summary>
public class CharacterCardUI : MonoBehaviour
{
    [SerializeField] Image border;
    [SerializeField] Image panel;
    [SerializeField] Image portrait;
    [SerializeField] Text nameLabel;
    [SerializeField] Text description;
    [SerializeField] Text keyLabel;

    public void Bind(CharacterData data, int index)
    {
        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        Color accent = data.color;
        if (border != null) border.color = new Color(accent.r, accent.g, accent.b, 0.85f);
        if (panel != null) panel.color = new Color(0.025f, 0.038f, 0.075f, 0.98f);
        if (portrait != null)
        {
            portrait.sprite = data.portrait;
            portrait.enabled = data.portrait != null;
        }

        if (nameLabel != null) nameLabel.text = data.displayName;
        if (description != null) description.text = data.description;
        if (keyLabel != null) keyLabel.text = "[" + (index + 1) + "]";
    }

    public void Clear()
    {
        gameObject.SetActive(false);
    }
}
