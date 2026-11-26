using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Card visual de um upgrade. Substitui a parede de texto do level up.
/// </summary>
public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] Image border;
    [SerializeField] Image panel;
    [SerializeField] Image accent;
    [SerializeField] Image icon;
    [SerializeField] Text title;
    [SerializeField] Text description;
    [SerializeField] Text keyLabel;

    public void Bind(UpgradeData data, int index, Color category)
    {
        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        if (icon != null)
        {
            icon.sprite = data.icon;
            icon.enabled = data.icon != null;
        }

        if (title != null) title.text = data.title;
        if (description != null) description.text = data.description;
        if (keyLabel != null) keyLabel.text = "[" + (index + 1) + "]";
        if (accent != null) accent.color = category;
        if (border != null) border.color = new Color(category.r, category.g, category.b, 0.85f);
        if (panel != null) panel.color = new Color(0.025f, 0.038f, 0.075f, 0.98f);
    }

    public void Clear()
    {
        gameObject.SetActive(false);
    }
}
