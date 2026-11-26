# 03 — Auditoria de gameplay e aderência ao GDD

**Referência:** `docs/01-GDD.md` (GDD), `docs/02-PLANO-DEV.md` (plano) e código atual.

---

## 1. Loop principal

O loop está **completo no código**:

```
mover/desviar → tiro automático → matar → orbe de XP
     → level up (3 opções) → build mais forte
     → boss às 5:00 → duelo → vitória/morte
     → Núcleos → meta-progressão → nova run
```

| Etapa | Implementação | Evidência |
|---|---|---|
| Mover | Sim | `PlayerController.FixedUpdate` |
| Tiro automático | Sim | `Weapon.Update` + `Enemy.FindNearest` |
| Matar/dano | Sim | `Projectile`, `Enemy.TakeDamage`, `Health` |
| XP/orbes | Sim | `Enemy.Die` → `XpSystem.SpawnOrb` → `XpOrb` |
| Level up | Sim | `XpSystem.AddXp` → `HudController.ShowLevelUp` |
| Upgrades | Sim | `UpgradeSystem.Roll/Apply` |
| Boss | Sim | `BossController` + `EnemySpawner` |
| Morte/vitória | Sim | `GameManager.EndRun`, `HudController.ShowGameOver` |
| Meta | Sim | `MetaProgression`, `SaveSystem` |
| Nova run | Sim | `GameManager.Restart` |

**Validação:** compila e a cena passa na validação. **Não** foi executada uma run
completa na versão atual; a última run registrada no CSV é 0.9.5.

---

## 2. Aderência ao GDD

| Item do GDD | Código atual | Situação |
|---|---|---|
| Run de 10 min | `runDuration = 600` | ✅ |
| Dificuldade +35%/min | `hpRampPerMinute = 0.35` | ✅ |
| Spawn 1,2/s → 18/s | `baseRatePerSecond/rateRampPerSecond` | ✅ |
| Lote 1→4 | `batchDivisor = 6` | ✅ |
| Spawn ring 13 u | `spawnRadius = 13` | ✅ |
| Máx. 300 vivos | `maxAlive = 300` | ✅ |
| 6 inimigos | 6 `EnemyData` | ✅ |
| 20 upgrades | 20 assets | ✅ |
| 3 pilotos | 3 `CharacterData` | ✅ |
| Boss às 5:00 | `bossTime = 300` | ✅ |
| Boss 1200 HP | base 1200 × dificuldade | ⚠️ spawn real 3300 |
| Rajada 14 projéteis/5 s | `burstCount=14`, `burstEvery=5` | ✅ |
| Investida 11 s | `chargeEvery=11` | ✅ |
| Telegrafo 0,75/0,65 s | `burstTelegraph`, `chargeTelegraph` | ✅ |
| Recuperação 0,8 s | `recoverTime = 0.8` | ✅ |
| Limpa horda | `ClearHorde()` | ✅ |
| Sem reforços no duelo | `BossController.Active` guard | ✅ |
| Arma prioriza boss | `Weapon.FindTarget` | ✅ |
| Level up a cada 20 s | XP por kill, não timer | ⚠️ divergência de descrição |
| Sprites coloridos por piloto | `CharacterSelect.Apply` altera `SpriteRenderer.color` | ⚠️ tint global, não sprite autoral |
| 3 opções sempre | `Roll(3)` pode retornar 0–3 | ✅ HUD trata 0 |

### Divergência importante: boss HP

O GDD lista o Núcleo com **1200 HP**. O código aplica `GameManager.Difficulty` a
**todos** os inimigos em `Enemy.OnEnable`:

```csharp
float multiplier = GameManager.Instance != null ? GameManager.Instance.Difficulty : 1f;
_health = data != null ? data.maxHealth * multiplier : 10f;
```

Às 5:00:

```text
Difficulty = 1 + (300 / 60) * 0.35 = 2.75
HP real = 1200 * 2.75 = 3300
```

O CSV de 0.9.5 mostra uma luta de **15,9 s** e vitória em 10:00. O número pode estar
intencional, mas o GDD precisa dizer “1200 base” ou “3300 no spawn às 5:00”. Hoje o
documento e o jogo não falam a mesma língua.

### Divergência: “a cada 20 segundos”

O GDD e a página antiga diziam “a cada ~20 s”. O código é **XP por kill**, com curva
exponencial. O intervalo inicial pode ficar perto de 20 s, mas não é garantido. A
página do itch já foi corrigida para “ao subir de nível”; o GDD ainda não.

### Divergência: versão da Unity

- GDD/plano: `6000.6.7f1`.
- `ProjectVersion.txt` e setup: `6000.6.1f1`.
- Setup explica que é a mesma linha 6.6; GDD/plano precisam ser alinhados.

---

## 3. Variedade e builds

### O que existe

- 20 upgrades com pré-requisitos, extras e limites.
- 3 pilotos com multiplicadores diferentes.
- 2 armas (tiro + orbes).
- 6 inimigos com papéis distintos.
- Meta com 3 bônus permanentes.

### O que está comprovado

O CSV tem apenas **3 runs**:

| Build | Duração | Kills | Nível | Vitória | Boss |
|---|---:|---:|---:|---|---:|
| 0.5.0 | 7:01 | 1.261 | 17 | não | 73,8 s |
| 0.9.1 | 3:06 | 708 | 14 | não | — |
| 0.9.5 | 10:00 | 2.695 | 20 | sim | 15,9 s |

A run de 0.9.5 valida que a build **consegue** vencer e que o boss foi ajustado para
~16 s. Não há dados de 0.9.8. Não há evidência de 3 builds distintas na versão atual.

### O que é promessa, não prova

- “3 builds reconhecíveis” — os upgrades existem, mas não há telemetria/playtest atual.
- “80% das runs entre 6 e 11 min” — 3 amostras, uma de 3:06.
- “Variedade de inimigos” — 6 tipos implementados; comportamento distinto no código,
  mas não revalidado em 0.9.8.

---

## 4. Dificuldade e ritmo

### Curva implementada

```text
rate = min(18, 1.2 + elapsed * 0.028)
batch = 1 + floor(rate / 6)
interval = batch / rate
HP = base * (1 + elapsed/60 * 0.35)
```

- 0:00: 1,2/s, 1 inimigo por lote, HP 1×
- 5:00: ~9,6/s, 2 por lote, HP 2,75×
- 10:00: 18/s, 4 por lote, HP 4,5×

### Pontos positivos

- Escalonamento legível e centralizado.
- Boss limpa a horda e evita “luta contra adds”.
- Telegrafo + recuperação dão janela de dano.
- Dano de contato tem intervalo, evitando morte instantânea por sobreposição.

### Riscos

- **Boss real 3300 HP** pode tornar a luta longa se a build não tiver dano.
- **Sem camadas de física**, o custo de colisão cresce com 300 inimigos/projéteis.
- **Sem profiler**, não há evidência de 60 fps na horda máxima.
- **Sem dados 0.9.8**, a mediana de duração e a taxa de vitória são desconhecidas.

---

## 5. Feedback e clareza

Implementado:

- flash de dano no inimigo e no jogador;
- shake de câmera;
- SFX procedurais;
- orbe de XP com cor por valor;
- level up com pausa e 3 opções;
- barra do boss;
- toast “NÚCLEO DESTRUÍDO”;
- game over com tempo, kills, nível, Núcleos e tempo do boss;
- HUD com timer/kills/level/vida/XP.

Gaps de UX:

| Item | Impacto |
|---|---|
| Game over só tem `[R]` | jogador não volta ao menu sem fechar o jogo |
| Options não fecha com ESC | atrito pequeno |
| Sem ícones de upgrade | 20 textos; a arte nova tem 20 ícones, mas não estão integrados |
| Sem retratos/identidade visual dos pilotos | 3 pilotos diferem só por números/cor |
| Feedback de dano no jogador é um flash de tela | pode ser pouco legível no caos |

---

## 6. Conteúdo previsto x implementado

### Implementado

- 6 inimigos, boss, 20 upgrades, 3 pilotos, 2 armas, meta, save, telemetria,
  menu, pausa, opções, game over, vitória.

### Planejado no GDD para depois do MVP/1.0

- 3 mapas, 8 personagens, 60 upgrades, evoluções de arma, conquistas, leaderboard,
  mobile, Steam. **Não são bugs**; são escopo futuro.

### Lacunas reais para o próximo ciclo

1. Integrar a arte nova (naves, inimigos, ícones, FX).
2. Revalidar 0.9.8 com uma run completa e 5–8 testadores.
3. Ajustar boss/HP com dados atuais.
4. Melhorar o fluxo de fim de run (menu).
5. Medir performance antes de aumentar `maxAlive`.

---

## 7. Veredito de gameplay

**O jogo é jogável e tem o loop completo.** O que falta não é “mais conteúdo”; é
**provar que o loop atual continua divertido e legível na versão 0.9.8**, agora com
a arte autoral e uma build atualizada.

Nota de gameplay: **7,5/10** (vertical slice), com potencial para 8,5+ depois de
arte integrada + playtest + ajuste de boss.
