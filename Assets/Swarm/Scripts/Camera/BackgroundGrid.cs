using UnityEngine;

/// <summary>
/// Grid de fundo que acompanha a câmera em passos de uma célula.
/// O padrão fica alinhado ao mundo e dá sensação de movimento.
/// </summary>
public class BackgroundGrid : MonoBehaviour
{
    [SerializeField] Transform follow;
    [SerializeField] float cell = 4f;

    void LateUpdate()
    {
        if (follow == null) return;

        Vector3 position = follow.position;
        transform.position = new Vector3(
            Mathf.Round(position.x / cell) * cell,
            Mathf.Round(position.y / cell) * cell,
            transform.position.z);
    }
}
