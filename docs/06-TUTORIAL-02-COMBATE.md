# Tutorial 02 — Tiro automático, inimigos e morte

**Objetivo:** entender o combate inteiro e a curva de dificuldade. 45 min.

---

## 1. O caminho de um tiro

```
Weapon.Update
  └─ Enemy.FindNearest(pos, range)
       └─ achou? → ObjectPool.Spawn(projectileTemplate)
            └─ Projectile.Init(dir, dano, pierce, velocidade, vida)
                 └─ OnTriggerEnter2D(Enemy)
                      └─ Enemy.TakeDamage(dano)
                           └─ vida ≤ 0 → morre → RegisterKill + SpawnOrb
```

### `Weapon.cs`

```csharp
cooldown -= Time.deltaTime;
if (cooldown > 0f) return;
Enemy target = Enemy.FindNearest(transform.position, stats.range);
if (target == null) return;                     // não gasta tiro sem alvo
cooldown = 1f / Mathf.Max(0.05f, stats.fireRate);
FireAt(target.transform.position);
```

O `Mathf.Max(0.05f, ...)` impede divisão por zero se `fireRate` virar 0.

### `Enemy.FindNearest`

Percorre a lista estática `Enemy.All`. Para 300 inimigos e 2,5 tiros/s, são
~750 comparações por segundo — irrelevante. **Só otimize quando o Profiler mandar.**

---

## 2. Por que existe o ObjectPool

Todo `Instantiate` aloca memória; todo `Destroy` gera lixo para o GC limpar depois.
O GC limpa em bloco e **congela o jogo** por alguns milissegundos.

`ObjectPool.Spawn`:
1. pega um objeto desativado da fila do prefab;
2. se não houver, cria um (`Instantiate`) e registra a origem;
3. posiciona e ativa.

`ObjectPool.Despawn` só desativa e devolve para a fila. **Nunca `Destroy` em gameplay.**

---

## 3. Inimigos

Abra `Scripts/Combat/Enemy.cs`. O ciclo:

```csharp
void FixedUpdate()
{
    Vector2 dir = (target.position - transform.position).normalized;
    _rb.linearVelocity = dir * data.moveSpeed;
}
```

- `data` é um `EnemyData` (ScriptableObject): HP, velocidade, dano, XP, cor, escala.
- `OnEnable` multiplica o HP pela dificuldade atual da run.
- `TakeDamage` seta `flash = 1f` e o `Update` interpola a cor para branco.

> O `target` é `PlayerController.PlayerTransform`, uma referência estática. Evita
> `FindGameObjectWithTag` a cada frame em 300 inimigos.

---

## 4. Spawner e curva

`Scripts/Combat/EnemySpawner.cs`:

```csharp
rate = Mathf.Min(18f, 1.2f + elapsed * 0.028f); // 1,2/s → 18/s
int batch = 1 + Mathf.FloorToInt(rate / 6f);    // vira lote no fim da run
_timer = batch / rate;
```

- Spawn em `player.position + direção_aleatória * 13` → sempre fora da tela.
- `maxAlive = 300` protege a performance.
- `PickTemplate` sorteia por `weight`, respeitando `minTime` e `maxAliveOfType`.
- O **boss** não entra no sorteio: ele nasce uma vez em `bossTime` (5:00).

### Ajuste fino (faça este teste)

Jogue 3 runs e anote em que minuto morreu. Meta: **6 a 11 min**.

| Se morre… | Ajuste |
|---|---|
| antes de 4 min | `baseRatePerSecond` 1,2 → 0,9 · `rateRampPerSecond` 0,028 → 0,022 |
| depois de 12 min | `rateRampPerSecond` 0,028 → 0,036 · `maxRatePerSecond` 18 → 22 |
| sempre no boss | `bossTime` 300 → 360 ou HP do boss −20% |
| horda "vazia" | `maxAlive` 300 → 400 (meça o FPS) |

---

## 5. Dano de contato

O player é **trigger**; o inimigo é **sólido**. Quem detecta é o player:

```csharp
void OnTriggerStay2D(Collider2D other)
{
    var enemy = other.GetComponent<Enemy>();
    if (enemy == null) return;
    if (Time.time - _contactTimer < 0.4f) return;
    _contactTimer = Time.time;
    _health.TakeDamage(enemy.Data.contactDamage);
    CameraFollow.Instance?.Shake(0.1f, 0.12f);
}
```

O intervalo de 0,4 s é o que impede morrer instantaneamente encostado em 10 inimigos.

---

## 6. Checkpoint

- [ ] O tiro sai do `Muzzle` e vai na direção do inimigo mais próximo.
- [ ] Inimigo pisca branco ao levar dano.
- [ ] Inimigo morre, some e deixa orbe verde.
- [ ] Encostar em inimigo drena vida em ticks de 0,4 s.
- [ ] Ao chegar a 0 de vida, o tempo para e a tela de fim aparece.

---

## 7. Erros comuns

| Sintoma | Causa | Solução |
|---|---|---|
| Não atira | `projectilePrefab` vazio | arraste o `ProjectileTemplate` para o `Weapon` |
| Tiro sai parado | `Rigidbody2D` do projétil sem `gravityScale = 0` | ajuste no template |
| Tiro atravessa sem dano | inimigo sem `Rigidbody2D` ou collider trigger | inimigo precisa de corpo **sólido** |
| Tiro acerta 2x o mesmo inimigo | faltou `hits` no `Projectile` | mantenha o `HashSet<Enemy>` |
| Inimigos empilhados | todos com collider trigger | inimigos **não** podem ser trigger |
| Spawn em cima do player | raio de spawn pequeno | mantenha ≥ 12 |

---

## 8. Desafio (30 min)

Crie o upgrade **"Escopeta"**: quando `projectiles >= 3`, reduza `range` em 20%
e aumente o `spread` de 10° para 25°.

Dica: o `spread` está em `Weapon.FireAt`. Mova para `PlayerStats` e ajuste pelo upgrade.
