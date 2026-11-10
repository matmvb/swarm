# SWARM — Game Design Document

| Campo | Valor |
|---|---|
| Gênero | Bullet heaven / survivor-like 2D |
| Referência | Space Invaders + Vampire Survivors |
| Plataforma | PC (Steam) primeiro, mobile depois |
| Sessão | Run de 10 min, sessão de 20–30 min |
| Público | 16–35, fãs de roguelite e jogos "de número subindo" |
| Motor | Unity 6.6 (6000.6.7f1) |
| Equipe | 1 pessoa (você) |

---

## 1. Pitch

Você é a última nave de defesa de um setor invadido. Não atira: **o sistema atira por você**.
Sua decisão é *para onde ir* e *qual upgrade escolher*. Cada 20 segundos o jogo pergunta:
"você quer ficar mais forte ou mais seguro?" — e as duas alternativas parecem certas.

**Frase de loja:** *"Space Invaders na dose de Survivors."*

---

## 2. Pilares

1. **Caos legível.** A tela enche, mas você sempre entende onde está o perigo.
2. **Decisão a cada 20s.** Upgrade é a pausa dramática; nunca mais de 25s sem escolher algo.
3. **Orgulho de build.** Ao morrer, você consegue contar em 1 frase o que sua build fazia.

---

## 3. Loop central

```
        ┌──────────────────────────────────────────────┐
        │                                              │
  [Mover e desviar] → [Tiro automático mata] → [Orbe de XP] 
        │                                              │
        └──────────────┬───────────────────────────────┘
                       ↓
                 [LEVEL UP]  ← pausa, 3 opções, escolha
                       ↓
                [Build mais forte] → inimigos mais fortes
                       ↓
                  [Morte] → [Núcleos] → [Upgrade permanente]
                       ↓
                  [Nova run]  ("só mais uma")
```

**Micro-loop:** 8–15 s (mover, matar, coletar).
**Meso-loop:** 20–25 s (level up).
**Macro-loop:** 10 min (run) e 30–60 min (desbloqueios).

---

## 4. Controles

| Ação | Teclado | Gamepad |
|---|---|---|
| Mover | WASD / setas | Stick esquerdo |
| Escolher upgrade | 1 / 2 / 3 | — (v1) |
| Reiniciar | R | — (v1) |

Tiro automático: o jogador **nunca** aperta atirar. Isso é design, não preguiça:
libera o cérebro para posicionamento e build.

---

## 5. Sistemas

| Sistema | Responsabilidade | Arquivo |
|---|---|---|
| `GameManager` | Estado da run, tempo, kills, morte, restart | `Core/GameManager.cs` |
| `PlayerController` | Movimento + dano de contato | `Player/PlayerController.cs` |
| `PlayerStats` | Todos os números do jogador | `Player/PlayerStats.cs` |
| `Health` | Vida genérica com eventos | `Player/Health.cs` |
| `Weapon` | Mira no inimigo mais próximo e atira | `Player/Weapon.cs` |
| `Projectile` | Voo, perfuração e dano | `Combat/Projectile.cs` |
| `Enemy` | Perseguição, vida, flash, morte | `Combat/Enemy.cs` |
| `EnemySpawner` | Curva de dificuldade e escolha de tipo | `Combat/EnemySpawner.cs` |
| `XpSystem` | XP, nível, fila de level ups | `Progression/XpSystem.cs` |
| `UpgradeSystem` | Sorteio e aplicação de upgrades | `Progression/UpgradeSystem.cs` |
| `XpOrb` | Coleta magnética de XP | `Pickup/XpOrb.cs` |
| `HudController` | HUD e telas de level up / game over | `UI/HudController.cs` |
| `ObjectPool` | Reuso de projéteis, inimigos e orbes | `Core/ObjectPool.cs` |
| `Sfx` | Efeitos sonoros gerados por código | `Core/Sfx.cs` |

---

## 6. Fórmulas (tuning inicial)

```csharp
// Dificuldade: multiplicador de HP dos inimigos
HpMult = 1 + (Elapsed / 60) * 0.35          // 10 min → 4.5x

// Spawner
rate   = min(18, 1.2 + Elapsed * 0.028)      // 1,2/s → 18/s em 10 min
lote   = 1 + floor(rate / 6)                 // 1 → 4
intervalo = lote / rate

// XP
xpProximoNivel *= 1.3                    // 5, 7, 9, 12... nv10=57, nv19=463

// Meta-moeda ao morrer
nucleos = floor(kills / 10) + floor(minutos)
```

> **Regra de ouro:** toda fórmula acima é uma constante num só lugar (`GameManager`,
> `EnemySpawner`, `XpSystem`). Nunca espalhe números mágicos pelo código.

---

## 7. Inimigos (MVP)

| Nome | Entra em | HP | Vel | Dano/0,4s | XP | Peso | Papel |
|---|---|---|---|---|---|---|---|
| Drone | 0:00 | 20 | 2.0 | 6 | 1 | 60 | básico, forma a horda |
| Corredor | 0:45 | 12 | 3.6 | 4 | 1 | 30 | pune jogador parado |
| Tanque | 2:00 | 90 | 1.3 | 14 | 3 | 20 | parede que exige dano |
| **Núcleo (boss)** | 5:00 | 1200 | 1.0 | 15 | 40 | evento | clímax do meio da run |

### Núcleo — padrão de ataque

| Ataque | Gatilho | Telegrafo | Efeito | Como esquivar |
|---|---|---|---|---|
| **Rajada radial** | a cada 5 s | 0,75 s com halo vermelho + som | 14 projéteis em círculo, 8 de dano cada | passar entre os projéteis |
| **Investida** | a cada 11 s | 0,65 s com halo vermelho + som | dash a 10 u/s por 0,5 s | sair da linha reta |

- Entre ataques, persegue a 1,0 u/s.
- Depois de cada ataque: **0,8 s de recuperação** (janela de dano).
- O telegrafo é um **halo vermelho pulsando ao redor do boss + som de aviso**. A escala também pulsa.
- Barra de vida do boss aparece no topo da tela enquanto ele estiver vivo.

> Arquivos: `Combat/BossController.cs` e `Combat/EnemyProjectile.cs`.
> Ajuste de dificuldade no Inspector do template `Enemy_Nucleo`.

Regras:
- Inimigos **não** empurram o jogador (colisor do jogador é trigger).
- Inimigos **se empurram entre si** (corpo sólido) → a horda se abre como líquido.
- Spawn sempre em um anel de raio 13 ao redor do jogador (nunca na tela).

---

## 8. Upgrades (MVP: 12)

| # | Nome | Efeito | Tipo | Máx |
|---|---|---|---|---|
| 1 | Munição Pesada | +20% dano | % | 5 |
| 2 | Gatilho Rápido | +15% cadência | % | 5 |
| 3 | Tiro Duplo | +1 projétil | flat | 3 |
| 4 | Perfuração | +1 perfuração | flat | 3 |
| 5 | Perna Leve | +12% velocidade | % | 5 |
| 6 | Blindagem | +25 vida máxima (e cura 25) | flat | 5 |
| 7 | Ímã | +40% raio de coleta | % | 3 |
| 8 | Mira Fria | +5% crítico | flat | 4 |
| 9 | Alcance | +15% alcance | % | 3 |
| 10 | Projétil Veloz | +15% velocidade do projétil | % | 3 |
| 11 | Reparo | cura 30% da vida máxima | flat | ilimitado |
| 12 | Núcleo Instável | +25% dano (raro e forte) | % | 2 |

**Regra do sorteio:** nunca oferecer upgrade no máximo; nunca repetir a mesma opção
na mesma tela; 3 opções sempre que houver 3 candidatos.

---

## 9. Curva da run (alvo)

| Minuto | Sensação desejada | Evento |
|---|---|---|
| 0–1 | "consigo matar tudo" | só Drones |
| 1–2 | "começa a apertar" | Corredores |
| 2–4 | "preciso escolher build" | Tanques, 8–10 upgrades |
| 5 | **pico** | Boss Núcleo |
| 6–8 | "estou poderoso" | tela cheia, build ligada |
| 9–10 | "não aguento mais" | horda máxima |
| 10+ | morte gloriosa | vitória se sobreviver a 10:00 |

Meta: **80% das runs terminam entre 6 e 11 minutos.**

---

## 10. Feedback (juice) — obrigatório

| Evento | Feedback |
|---|---|
| Acerto em inimigo | flash branco 0,15s + som "tick" com pitch aleatório |
| Morte de inimigo | shake 0,03 + som + orbe de XP voando |
| Dano no jogador | shake 0,10 + som grave + barra de vida pisca |
| Level up | som ascendente + tela escurece + 3 opções |
| Escolha de upgrade | som "confirm" + número atualizado no HUD |
| Crítico | dano em dobro + flash amarelo |

**Regra:** nenhum evento de gameplay pode acontecer sem **som + movimento + cor**.
Se acontecer, o jogador não sente que aconteceu.

---

## 11. Meta-progressão

Moeda: **Núcleos** (ganhos ao morrer, nunca perdidos).

| Melhoria | Custo | Efeito |
|---|---|---|
| Núcleo de Dano | 50 | +5% dano permanente |
| Núcleo de Casco | 50 | +5% vida permanente |
| Núcleo de Motor | 50 | +5% velocidade permanente |
| Piloto reserva | 200 | desbloqueia personagem (Sprint 5) |

MVP implementa apenas o **contador** de Núcleos + save. As compras entram no Sprint 5.

---

## 12. Escopo

**MVP (Sprint 0–3, ~4 semanas):** 4 inimigos, 12 upgrades, 1 arma, 1 mapa infinito,
HUD, level up, game over, restart. **É o que este repositório entrega.**

**v0.5 / demo (Sprint 4–7, +6 semanas):** boss, 2ª arma, 6 inimigos, 20 upgrades,
3 personagens, tela de meta-upgrade, build Windows, página Steam.

**1.0:** 3 mapas, 8 personagens, 60 upgrades, evoluções de arma, conquistas, leaderboard.

---

## 13. Arte e som

- **Estilo:** neon vetorial sobre fundo quase preto. Formas geométricas, glow barato.
- **Paleta:** fundo `#0A0E1A`; jogador ciano `#4DFFE0`; inimigos magenta/vermelho; XP verde.
- **Por que funciona:** formas legíveis em qualquer resolução, produção barata, e o
  contraste alto é o que faz o jogo "parecer bom" em clipe de 6 segundos.
- **Som:** sintetizado (o `Sfx.cs` já gera tudo por código). Zero asset externo no MVP.

---

## 14. KPIs do protótipo

| Métrica | Meta | Como medir |
|---|---|---|
| "Só mais uma" | > 50% | playtest: quem reinicia em < 10 s |
| Duração média da run | 6–11 min | log do `GameManager` |
| Level ups por run | 12–20 | contador |
| FPS | 60 estável com 300 inimigos | Profiler |
| D1 (demo) | > 35% | Steam/itch |

---

## 15. Anti-metas (o que NÃO fazer)

- ❌ Loot box paga, energia, timers, pay-to-win.
- ❌ 3D realista (custo alto, retorno baixo para 1 pessoa).
- ❌ História com cutscene no MVP.
- ❌ Multiplayer no primeiro ano.
- ❌ Mais de 1 arma antes de o loop estar divertido com 1.
