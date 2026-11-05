# Arquitetura

## 1. Princípios

1. **Dados fora do código.** Inimigos e upgrades são `ScriptableObject`s (`.asset`) —
   você ajusta no Inspector sem recompilar.
2. **Um sistema, uma responsabilidade.** Quem move não atira; quem atira não sabe de XP.
3. **Comunicação por evento.** `Health` não conhece a HUD; emite `Changed` e quem quiser escuta.
4. **Nada nasce/morre em massa sem pool.** 300 inimigos + 100 projéteis com
   `Instantiate/Destroy` gera GC e travadas.
5. **Números de tuning no topo da classe**, com `[Header]`, nunca espalhados.

---

## 2. Estrutura de pastas

```
Assets/Swarm/
├── Art/
│   └── white.png                 sprite 1x1 branco (tint por SpriteRenderer.color)
├── Data/
│   ├── Enemies/                  Drone, Corredor, Tanque, Nucleo
│   └── Upgrades/                 12 upgrades
├── Scenes/
│   └── Swarm.unity
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs        estado da run
│   │   ├── ObjectPool.cs         pool genérico
│   │   ├── InputReader.cs        abstração de input (novo/legado)
│   │   ├── Sfx.cs                sons gerados por código
│   │   └── SaveSystem.cs         Núcleos e recordes
│   ├── Player/
│   │   ├── PlayerController.cs   movimento + dano de contato
│   │   ├── PlayerStats.cs        números do jogador
│   │   ├── Health.cs             vida com eventos
│   │   └── Weapon.cs             mira e dispara
│   ├── Combat/
│   │   ├── Enemy.cs              perseguição, dano, morte
│   │   ├── EnemyData.cs          ScriptableObject
│   │   ├── EnemySpawner.cs       curva de dificuldade
│   │   └── Projectile.cs         projétil com perfuração
│   ├── Progression/
│   │   ├── XpSystem.cs           XP, nível, fila
│   │   ├── UpgradeSystem.cs      sorteio e aplicação
│   │   └── UpgradeData.cs        ScriptableObject
│   ├── Pickup/
│   │   └── XpOrb.cs              coleta magnética
│   ├── Camera/
│   │   └── CameraFollow.cs       follow + shake
│   └── UI/
│       └── HudController.cs      HUD + telas
└── Editor/
    └── SwarmSceneBuilder.cs      gerador de cena/assets
```

> `Editor/` é especial: só compila no editor e pode usar `UnityEditor`.

---

## 3. Fluxo de uma run

```
GameManager.Start
   └─ IsRunning = true

Update
   ├─ InputReader.Move ──────────► PlayerController ──► Rigidbody2D.linearVelocity
   ├─ EnemySpawner ──────────────► ObjectPool.Spawn(Enemy)
   ├─ Enemy.FixedUpdate ─────────► persegue PlayerTransform
   ├─ Weapon.Update ─────────────► Enemy.FindNearest ──► ObjectPool.Spawn(Projectile)
   └─ Projectile.OnTriggerEnter2D ► Enemy.TakeDamage
                                        └─ morre ──► GameManager.RegisterKill
                                                 └─ XpSystem.SpawnOrb

XpOrb.Update ──► perto do player ──► XpSystem.AddXp
                                        └─ nível ↑ ──► HudController.ShowLevelUp (timeScale = 0)
                                                          └─ [1/2/3] ──► UpgradeSystem.Apply
                                                                          └─ timeScale = 1

Health.Died ──► GameManager.EndRun ──► HudController.ShowGameOver
                                   └─ SaveSystem.AddCores
```

---

## 4. Por que essas escolhas

### Por que `ScriptableObject` para inimigo/upgrade?

- Ajustar 90 de HP não exige abrir 4 scripts.
- O mesmo dado é reutilizável (Tanque aparece em 3 ondas diferentes).
- Evita "prefab com valores mágicos" que ninguém sabe de onde vieram.

### Por que pool?

`Instantiate` aloca memória gerenciada e chama o GC depois. Em um jogo que cria
~50 objetos por segundo, isso vira **spike de 40 ms** a cada poucos segundos — o
famoso "micro stutter". O pool reaproveita e o custo cai para ~0.

### Por que o jogador é trigger e o inimigo é sólido?

- Player trigger → inimigos **não** empurram o jogador (controle fica preciso).
- Inimigos sólidos entre si → a horda se espalha em vez de virar uma pilha.
- `OnTriggerStay2D` no player centraliza o dano de contato em um lugar só.

### Por que `linearVelocity` e não `transform.position`?

Em Unity 6, `Rigidbody2D.velocity` virou `linearVelocity`. Mover o corpo pela
velocidade respeita a física e mantém as colisões estáveis.

### Por que input abstraído?

`ENABLE_INPUT_SYSTEM` permite o mesmo código compilar com o pacote novo **ou** com
o sistema legado. Trocar de input depois não quebra nada.

---

## 5. Eventos (quem escuta quem)

| Emissor | Evento | Quem escuta | Efeito |
|---|---|---|---|
| `Health` | `Changed(current, max)` | `HudController` | atualiza barra de vida |
| `Health` | `Died` | `PlayerController` | chama `GameManager.EndRun` |
| `XpSystem` | `OnChanged` | `HudController` | atualiza barra de XP |
| `GameManager` | `OnRunEnded` | `HudController` | mostra tela de fim |

Regra: **o emissor não chama a HUD diretamente.** Se um dia você trocar a HUD por
UI Toolkit, nada no gameplay muda.

---

## 6. Como estender

### Novo inimigo (3 passos)

1. `Assets/Swarm/Data/Enemies` → botão direito → `Create > SWARM > Enemy Data`.
2. Preencha: nome, HP, velocidade, dano, XP, cor, escala, `minTime`, `weight`.
3. Na cena, duplique um template de inimigo, troque o `Data` e **desative** o objeto.
   Arraste para a lista `Templates` do `EnemySpawner`.

### Novo upgrade (2 passos)

1. Botão direito em `Data/Upgrades` → `Create > SWARM > Upgrade`.
2. Escolha `stat`, `amount`, `percent`, `maxStacks`. Se o `stat` for novo, adicione
   um caso no `switch` de `UpgradeSystem.Apply`.

### Nova arma (Sprint 4)

Crie `WeaponData` (ScriptableObject) + uma classe por comportamento (`OrbitWeapon`,
`BurstWeapon`) que implemente uma interface `IWeapon { void Tick(); }`. O `Weapon`
atual vira uma dessas implementações.

---

## 7. Convenções

| Coisa | Padrão | Exemplo |
|---|---|---|
| Classe | `PascalCase` | `EnemySpawner` |
| Campo privado | `_camelCase` | `_cooldown` |
| `[SerializeField]` | `camelCase` | `moveSpeed` |
| Constante | `SCREAMING_SNAKE` | `MAX_ENEMIES` |
| Evento | `OnAlgo` | `OnRunEnded` |
| ScriptableObject | `AlgoData` | `EnemyData` |
| Cena | `PascalCase` | `Swarm.unity` |

- Um `MonoBehaviour` por arquivo, **com o nome do arquivo igual ao da classe**.
- Sem `namespace` no MVP (evita `using` desnecessário). Se o projeto crescer,
  adote `namespace Swarm`.
- Sem `public` em campo que não precisa ser público — use `[SerializeField]`.

---

## 8. Orçamento de performance (alvo)

| Recurso | Orçamento | Como medir |
|---|---|---|
| Frame | 16,6 ms (60 fps) | Profiler |
| Scripts | < 4 ms | Profiler > CPU |
| Física 2D | < 2 ms | Profiler > Physics2D |
| Render | < 6 ms | Frame Debugger |
| GC por frame | **0 B** | Profiler > GC Alloc |
| Inimigos vivos | 300–400 | contador no HUD (debug) |

Se estourar, a ordem de ataque é: **pool → reduzir `GetComponent` → layers →
simplificar colisores** (nessa ordem).
