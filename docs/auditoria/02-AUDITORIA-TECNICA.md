# 02 — Auditoria técnica e de arquitetura

**Escopo:** 47 scripts C#, cena principal, ScriptableObjects, build e pipeline de arte.  
**Modo:** leitura + compilação/validação em batch, sem editar código.

---

## 1. Arquitetura geral

O projeto segue uma arquitetura **simples, orientada a cena e singletons implícitos**:

- `GameManager` centraliza estado da run.
- Sistemas de gameplay usam singletons estáticos (`PlayerController`, `XpSystem`,
  `Sfx`, `CameraFollow`, `DeathBurst`, `MuzzleFlash`, `HitStop`, `BossController`).
- `Enemy.All` é uma lista estática de inimigos vivos.
- `ObjectPool` estático por prefab.
- ScriptableObjects guardam dados de inimigos, pilotos e upgrades.
- A cena é gerada por `SwarmSceneBuilder`; não há prefabs persistidos.
- UI é Canvas/UGUI com `Text` legado.
- Não há assembly definitions, namespace ou testes.

Para um projeto solo e um vertical slice, a arquitetura é **adequada e legível**. Para
escalar conteúdo e equipe, ela tem acoplamentos que vão cobrar juros: singletons,
dependência de cena, `Enemy.All` global e um gerador de 1.600 linhas.

---

## 2. Ciclo de vida e estado

### `GameManager`

- `Awake` define singleton e `Time.timeScale = 1`.
- `Start` reseta tempo, kills, vitória e stats do boss.
- `Update` incrementa `Elapsed` e encerra em `runDuration`.
- `EndRun` calcula Núcleos, salva, dispara `OnRunEnded`, pausa.
- `Restart` recarrega a cena por `buildIndex`.
- `OnDestroy` limpa `ObjectPool`.

**Pontos bons:** estado centralizado, evento único de fim, sem lógica de vitória espalhada.

**Problemas:**

| ID | Gravidade | Evidência | Consequência |
|---|---|---|---|
| M-03 | MÉDIO | `GameManager.cs:107-112` usa `SceneManager.GetActiveScene().buildIndex` | Se a cena ativa não estiver no Build Settings, `buildIndex == -1` e `LoadScene(-1)` falha |
| — | BAIXO | `GameManager.cs:92-97` calcula Núcleos no fim | Fórmula correta, mas não há teste automatizado da regra |

### `Time.timeScale`

Fluxo revisado:

- menu principal: 0
- seleção de piloto: 0
- level up: 0
- pausa: 0
- hitstop: 0,05 por tempo real
- game over/vitória: 0
- restart: 1

Não encontrei deadlock óbvio. `HitStop` não sobrescreve pausa porque verifica
`Time.timeScale > 0.01f` antes de restaurar (`HitStop.cs:32-43`).

**Risco:** o fluxo depende de vários componentes escrevendo `Time.timeScale`. Uma
classe nova pode quebrar a ordem. Recomendo um `TimeScaleOwner`/contador central no
futuro, não nesta rodada.

---

## 3. Jogador e combate

### Movimento

- `PlayerController.FixedUpdate` usa `Rigidbody2D.linearVelocity`.
- Input novo via `InputReader`.
- Jogador é trigger; inimigos não o empurram.
- Dano de contato usa `OnTriggerStay2D` + intervalo de 0,4 s.

**Problemas:**

| ID | Gravidade | Evidência | Consequência |
|---|---|---|---|
| M-04 | MÉDIO | `PlayerController.cs:64-76` faz `GetComponent<Enemy>()` em callback de física | Custo por contato; aceitável agora, mas evitável com layer/tag/cache |
| L-12 | BAIXO | Só um inimigo causa dano por intervalo global | Se 3 inimigos encostam, o dano não escala; validar se é intencional |

### Arma e alvo

- `Weapon` prioriza boss no alcance, depois `Enemy.FindNearest`.
- `Enemy.FindNearest` percorre `Enemy.All` O(N).
- `PlayerVisual` usa a mesma prioridade para girar a nave.
- `PlayerVisual` roda em `LateUpdate` e chama `FindNearest` **todo frame**.

**Problemas:**

| ID | Gravidade | Evidência | Consequência |
|---|---|---|---|
| M-06 | MÉDIO | `PlayerVisual.cs:16-53` + `Enemy.cs:194-213` | O(N) por frame; com 300 inimigos são 18k comparações/s, mas o custo cresce com o tempo |
| — | BAIXO | `Weapon.cs:60-69` não devolve ao pool se `GetComponent<Projectile>()` falhar | Vazamento raro se o prefab estiver sem componente |

**Ponto bom:** a prioridade do boss é consistente entre arma e visual; o tiro sai da
ponta da nave. Isso foi corrigido em auditoria anterior e está no código.

### Vida e dano

- `Health` tem eventos `Changed`/`Died`, sem conhecer HUD/GameManager.
- Armadura altera `damageTakenMultiplier`.
- `ScaleMaxHealth` aplica piloto/meta.
- `AddMaxHealth` cura junto.

**Ponto bom:** classe pequena, testável, sem dependência de UI.

**Risco:** `Health.Start` dispara `Changed`; a HUD também faz `RefreshHp` no `Start`.
A ordem não quebra hoje, mas é frágil.

---

## 4. Inimigos, spawner e boss

### `Enemy`

- Lista estática `All`.
- `OnEnable` reativa colisor, zera cooldown/velocidade, reseta vida.
- `FixedUpdate` persegue o player.
- `Update` decai flash e cooldown orbital.
- `Die` registra kill, dropa XP, burst, shake, desativa colisor.
- Boss é identificado por `data.scale > 1.4f`.

**Problemas:**

| ID | Gravidade | Evidência | Consequência |
|---|---|---|---|
| M-02 | MÉDIO | `Enemy.cs:154` e `CameraFollow` usam `data.scale > 1.4f` como “é boss” | Se a escala do Núcleo mudar, ele deixa de ser tratado como boss |
| M-02b | MÉDIO | `Enemy.cs:64-66` multiplica HP de **todos** os inimigos por `Difficulty` | Núcleo base 1200 × 2,75 às 5:00 = **3300 HP**, não 1200 como a tabela do GDD sugere |
| M-08 | MÉDIO | `Enemy.cs:68-77` e `110-114` usam `SpriteRenderer.color` para tint/flash | Com sprite colorido e `color = white`, o flash branco não aparece; precisa overlay/shader |
| L-09 | BAIXO | `Enemy.cs:154-158` despawna boss imediatamente | A animação de morte do boss não é usada |
| — | BAIXO | `Enemy.DeathRoutine` usa `Time.deltaTime` | Se o jogo pausar durante a morte, a rotina congela; não trava, mas fica pendurada |

**Ponto bom:** `OnEnable` corrige o bug crítico de colisor do pool; `All.Remove` no
`OnDisable`; `_dead` impede dano duplo.

### `EnemySpawner`

- Anel de spawn 13 u.
- Taxa 1,2/s → 18/s.
- `maxAlive = 300`.
- Aviso de spawn de 0,35 s.
- Boss em 300 s; limpa horda; sem reforços durante duelo.

**Problemas:**

| ID | Gravidade | Evidência | Consequência |
|---|---|---|---|
| — | BAIXO | `CanSpawn` percorre `Enemy.All` para cada template | O(N × tipos) por spawn; aceitável em 300, mas pode ser cacheado |
| — | BAIXO | F2 no editor não marca `_bossSpawned` | Após matar o boss de debug, outro boss pode nascer às 5:00 |

**Ponto bom:** a luta do boss é limpa, sem adds, e o spawner pausa durante o duelo.

### `BossController`

- Máquina de estados: Chase, TelegraphBurst, TelegraphCharge, Charge, Recover.
- Rajada de 14 projéteis a cada 5 s.
- Investida a cada 11 s.
- Telegrafo por halo + som.
- `OnDisable` calcula `LastFightWon/Duration`.

**Problemas:**

| ID | Gravidade | Evidência | Consequência |
|---|---|---|---|
| M-02c | MÉDIO | `BossController.cs:104-110` usa `Health01 <= 0` no `OnDisable` | Se o boss for desativado por outro motivo com vida 0, conta vitória; hoje só despawna na morte/cena |
| — | BAIXO | `EnsureTelegraph` copia o sprite atual uma vez | Se o boss usar sprite sheet/animado, o halo não acompanha os frames |

**Ponto bom:** telegrafo legível, recuperação, hitstop, sem `Update` pesado.

---

## 5. Progressão

### XP e level up

- Curva `xpToNext *= 1.3`.
- Fila `_pendingLevelUps`.
- Guard de 100 loops.
- `UpgradeSystem.Roll(3)`.

**Problemas:**

| ID | Gravidade | Evidência | Consequência |
|---|---|---|---|
| L-06 | BAIXO | `XpSystem.cs:6` diz “curva linear” | Comentário stale; a curva é exponencial |
| — | BAIXO | `XpSystem.AddXp` não limita XP pendente | Guard evita loop, mas XP acima do limiar fica para o próximo add |

**Ponto bom:** HUD trata `options.Length == 0` com “Sem upgrades disponíveis / [1] continuar”.
Não há softlock quando todos os upgrades estão no máximo.

### `UpgradeSystem`

- Remove upgrades no máximo.
- Respeita pré-requisitos.
- Filtra cura acima de 90%.
- Garante opção ofensiva na primeira escolha.
- Aplica principal + extras.

**Problemas:**

| ID | Gravidade | Evidência | Consequência |
|---|---|---|---|
| L-07 | BAIXO | `IsOffensive` considera só Damage/FireRate/Projectiles | Orbe Orbital não conta como ofensivo na primeira escolha |
| — | BAIXO | `_taken` é um `Dictionary<UpgradeData,int>` | Sem reset explícito; nova run recarrega a cena, então ok |

**Ponto bom:** regras do GDD (pré-requisito, limite, cura, sem repetir) estão implementadas.

### Meta-progressão e save

- 3 upgrades, custo 1,5×, máximo 20.
- PlayerPrefs (`meta.damage`, `meta.health`, `meta.speed`, `swarm.cores`).
- `GameOptions.ResetSave` apaga tudo.

**Problemas:**

| ID | Gravidade | Evidência | Consequência |
|---|---|---|---|
| L-03 | BAIXO | `SaveSystem.BestKills`, `BestTime`, `ResetAll` não usados | Código morto; pode confundir manutenção |
| M-17 | MÉDIO | Sem teste de persistência em runtime | Não há evidência de save entre sessões nesta rodada |

**Ponto bom:** operações pequenas, síncronas e com `PlayerPrefs.Save()`.

---

## 6. Interface

### `HudController`

- HUD a 10 Hz e só quando o valor muda.
- Level up com fila.
- Game over.
- Barra do boss.
- Toast do boss.
- Flash de dano.

**Problemas:**

| ID | Gravidade | Evidência | Consequência |
|---|---|---|---|
| M-09 | MÉDIO | `HudController.cs:250-273` só oferece `[R] JOGAR DE NOVO` | Após a run, o jogador não volta ao menu sem fechar o jogo |
| L-01 | BAIXO | `OptionsMenu` só fecha com `O` | `ESC` não fecha opções; não trava, mas é menos intuitivo |
| — | BAIXO | `damageFlash.color = new Color(...)` por frame durante flash | Alocação de struct (não heap) e rebuild de UI; baixo impacto |

**Ponto bom:** HUD com throttle, sem strings por frame; level up e game over usam
StringBuilder.

### Menus

- MainMenu, PauseMenu, OptionsMenu, CharacterSelect.
- `AutoStartRun` para restart rápido.
- `Time.timeScale` controlado por contexto.

**Problemas:**

| ID | Gravidade | Evidência | Consequência |
|---|---|---|---|
| M-12 | MÉDIO | `CharacterSelect.cs:147-159` só muda a cor do sprite | As 3 naves novas não podem ser usadas sem campo/atribuição de sprite |
| — | BAIXO | `MainMenu.QuitGame` sai direto no ESC | Sem confirmação; aceitável para demo |

### Fontes e layout

- `MakeText` define `fontSize` e `rect`, mas **não** define `resizeTextForBestFit`,
  `maxSize` ou overflow.
- A cena atual tem `m_MaxSize` manual 46/60/72 em 5 textos; `m_BestFit: 0`.
- `SwarmSceneBuilder` não reproduz essas alterações.
- `HudController` e `CharacterSelect` usam `Text` legado.

**Consequência:** o layout pode ser perdido ao regenerar a cena, e os ajustes de max
size podem ser no-op com `BestFit=0`. É uma divergência de fonte de verdade, não um
bug visual comprovado.

---

## 7. Pool, FX, áudio, câmera, telemetria

### `ObjectPool`

- Genérico por prefab, `Queue`, `Origin`.
- `Spawn` reativa; `Despawn` ignora inativo; `ClearAll` no restart.
- Sem pré-aquecimento, sem limite máximo.

**Ponto bom:** resolve o grosso de Instantiate/Destroy em gameplay.

**Risco:** primeiro combate pode instanciar vários objetos e gerar hitch; o
`10-PERFORMANCE-QA.md` recomenda prewarm, mas ele não existe.

### FX

- `BurstParticle`, `DeathBurst`, `MuzzleFlash`, `ThrusterTrail`.
- Poolados.
- `BurstParticle` usa `Color` struct e `SpriteRenderer.color`; sem alocação heap.

**Risco:** muitos `TrailRenderer` em projéteis e muitos `BurstParticle` em hordas;
sem atlas/cap, pode pesar.

### `Sfx`

- Clips sintetizados no `Awake`.
- Música de 16 s em 22.050 Hz.
- `PlayOneShot` em um único `AudioSource`.
- `pitch` altera todas as one-shots ativas naquele source.

**Riscos:** startup de áudio, pitch compartilhado, sem limitador de vozes.

### `CameraFollow`

- SmoothDamp, shake/zoom em tempo real.
- Sem bounds.
- `ZoomPunch` pode ser sobrescrito.

**Ponto bom:** shake não cancela um shake mais forte.

### `RunLogger`

- CSV por run, cultura invariante, UTF-8 sem BOM.
- `boss_time`, `character`, `meta_*`, `build`.
- Sem log de runs abandonadas.

**Ponto bom:** schema consistente e suficiente para balanceamento.

---

## 8. Qualidade de código e manutenção

### Pontos positivos

- Sem LINQ no runtime.
- Sem `FindObjectOfType` no runtime.
- Eventos desinscritos em `OnDestroy`/`OnDisable` nos casos revisados.
- Sem `m_Script: {fileID: 0}`.
- HUD com throttle.
- `ObjectPool` centralizado.
- Comentários em português e nomes claros.

### Dívidas

| ID | Gravidade | Evidência | Consequência |
|---|---|---|---|
| M-19 | MÉDIO | `SwarmSceneBuilder.cs` ~1.600 linhas, gera cena + assets + valida | Difícil manter/testar; qualquer rerun pode alterar assets |
| M-19 | MÉDIO | `SwarmSceneBuilder.BuildAndValidateBatch` regenera a cena | Pode apagar ajustes manuais não reproduzidos |
| M-13 | MÉDIO | `EnsureAsset` só configura asset novo | Campos novos (ex.: sprite de nave) não chegam aos assets existentes |
| — | BAIXO | Classes sem namespace/asmdef | Colisões e compile time no futuro |
| — | BAIXO | `Sfx` e `HudController` poderiam ser divididos | Manutenção, não urgente |
| M-15 | MÉDIO | `BuildScript` e `ProjectSettings` repetem a versão `0.9.8` | Risco de build com versão divergente |

---

## 9. Conclusão técnica

O código **não está quebrado**: compila, a cena valida e os sistemas principais estão
implementados com qualidade razoável para um protótipo avançado.

O problema é de **integração e processo**:

1. arte nova fora do jogo;
2. import settings errados;
3. WIP fora do Git;
4. build antiga;
5. sem testes/performance;
6. segredo de acesso público.

Antes de adicionar conteúdo, o projeto precisa **fechar o ciclo código → asset →
cena → build → teste**.
