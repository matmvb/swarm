using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Arma secundária: orbes que giram ao redor do jogador e causam dano de contato.
/// A quantidade vem de PlayerStats.orbitals (upgrade "Orbe Orbital").
/// </summary>
[RequireComponent(typeof(PlayerStats))]
public class OrbitalWeapon : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] float radius = 3f;
    [SerializeField] float rotationSpeed = 160f;

    [Header("Combate")]
    [SerializeField] float damageMultiplier = 1.6f;
    [SerializeField] float hitInterval = 0.3f;
    [SerializeField] float hitRadius = 0.7f;

    [Header("Visual")]
    [SerializeField] float orbScale = 0.45f;
    [SerializeField] Sprite orbSprite;

    readonly List<Transform> _orbs = new List<Transform>();

    PlayerStats _stats;
    SpriteRenderer _playerSprite;
    float _angle;

    void Awake()
    {
        _stats = GetComponent<PlayerStats>();
        _playerSprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsRunning) return;
        if (_stats == null) return;

#if UNITY_EDITOR
        // Atalho de teste: F3 adiciona um orbe (não existe em build).
        if (InputReader.WasPressed(GameKey.DebugOrbital))
            _stats.orbitals = Mathf.Clamp(_stats.orbitals + 1, 0, 8);
#endif

        EnsureOrbs(Mathf.Clamp(_stats.orbitals, 0, 8));

        if (_orbs.Count == 0) return;

        _angle += rotationSpeed * Time.deltaTime;
        PositionOrbs();
        DamageEnemies();
    }

    void EnsureOrbs(int count)
    {
        while (_orbs.Count < count)
            _orbs.Add(CreateOrb(_orbs.Count));

        while (_orbs.Count > count)
        {
            int last = _orbs.Count - 1;
            Transform orb = _orbs[last];
            _orbs.RemoveAt(last);
            if (orb != null) Destroy(orb.gameObject);
        }
    }

    Transform CreateOrb(int index)
    {
        var go = new GameObject("OrbitalOrb_" + index);
        go.transform.SetParent(transform, false);
        go.transform.localScale = Vector3.one * orbScale;

        var sprite = go.AddComponent<SpriteRenderer>();
        sprite.sprite = orbSprite != null ? orbSprite : (_playerSprite != null ? _playerSprite.sprite : null);
        if (_playerSprite != null) sprite.sharedMaterial = _playerSprite.sharedMaterial;
        sprite.color = new Color(0.6f, 1f, 1f);
        sprite.sortingOrder = 8;

        return go.transform;
    }

    void PositionOrbs()
    {
        float step = 360f / _orbs.Count;

        for (int i = 0; i < _orbs.Count; i++)
        {
            float radians = (_angle + step * i) * Mathf.Deg2Rad;
            _orbs[i].localPosition = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f) * radius;
        }
    }

    void DamageEnemies()
    {
        float damage = _stats.damage * damageMultiplier * _stats.orbitalDamage;

        for (int i = _orbs.Count - 1; i >= 0; i--)
        {
            Transform orb = _orbs[i];
            if (orb == null) continue;

            Vector2 orbPosition = orb.position;

            for (int e = Enemy.All.Count - 1; e >= 0; e--)
            {
                Enemy enemy = Enemy.All[e];
                if (enemy == null || enemy.orbitalCooldown > 0f) continue;

                float enemyRadius = enemy.transform.localScale.x * 0.5f;
                float threshold = hitRadius + enemyRadius;

                if (((Vector2)enemy.transform.position - orbPosition).sqrMagnitude > threshold * threshold)
                    continue;

                enemy.orbitalCooldown = hitInterval;
                enemy.TakeDamage(damage, false);
            }
        }
    }
}
