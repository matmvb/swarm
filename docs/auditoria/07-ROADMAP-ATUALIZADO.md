# 07 — Roadmap atualizado

**Premissa:** 1 pessoa, ~15 h/semana.  
**Regra:** nenhuma feature nova antes de fechar P0–P2.

---

## Resumo das etapas

| Etapa | Objetivo | Esforço |
|---|---|---|
| P0 | Preservar o WIP, proteger o playtest e alinhar build/fonte | 0,5–1 dia |
| P1 | Integrar a arte e estabilizar o núcleo | 2–3 dias |
| P2 | Balanceamento e experiência | 2–3 dias |
| P3 | Performance medida | 2–3 dias |
| P4 | Polimento visual/sonoro | 3–5 dias |
| P5 | Conteúdo adicional | 1–2 semanas |
| P6 | Playtest externo e publicação | 1 semana |

---

# P0 — Correções de processo e segurança

**Objetivo:** parar de perder trabalho e parar de vazar acesso.

| # | Tarefa | Arquivos | Dependências | DoD |
|---|---|---|---|---|
| P0.1 | Criar branch `wip/art-0.9.8` e commitar cena + `Art/Sprites` + manifest + preview | `Swarm.unity`, `Art/Sprites/**`, `docs/art/sprites-preview.*` | Aprovação do autor | Tudo recuperável pelo Git |
| P0.2 | Rotacionar a senha do itch | itch.io (externo) | Acesso à conta | Nova senha só no convite |
| P0.3 | Remover a senha real dos docs públicos; usar `[SENHA]`/placeholder | `docs/20`, `docs/21`, `docs/24` | P0.2 | Nenhuma senha real no repo/Pages |
| P0.4 | Confirmar que o ZIP do itch é o build correto | `Builds/SWARM-windows.zip`, itch | P0.1/P0.2 | Versão e data conferidas |
| P0.5 | Decidir: arte nova entra agora ou fica para depois? | — | Autor | Decisão registrada |

**Riscos:** commitar WIP quebrado; resolver com branch separada, não em `main`.
**Critério de saída:** WIP salvo, senha trocada, docs limpos, decisão de arte tomada.

---

# P1 — Integração de arte e estabilização

**Objetivo:** fazer a arte existir no jogo sem quebrar o que funciona.

| # | Tarefa | Arquivos | Dependências | DoD |
|---|---|---|---|---|
| P1.1 | Criar editor script que aplica o `manifest.json` nos `.meta` | novo `Editor/SpriteImportPipeline.cs` | P0.1 | 72 sprites com PPU/frame, Point, FullRect, Extrude 0 |
| P1.2 | Atribuir sprites de inimigos | `EnemyData.sprite`, `SwarmSceneBuilder` | P1.1 | 6 inimigos com arte nova |
| P1.3 | Atribuir sprites de naves por piloto | `CharacterData`, `CharacterSelect`, `PlayerVisual` | P1.1 | 3 naves distintas; tint branco |
| P1.4 | Atribuir projéteis/orbes/FX/UI | `Weapon`, `Projectile`, `XpOrb`, `OrbitalWeapon`, `HudController` | P1.1 | Assets referenciados na cena |
| P1.5 | Implementar `SpriteAnimator` leve (se usar frames) | novo `Scripts/Art/SpriteAnimator.cs` | P1.2 | Idle/death do boss e elites animam |
| P1.6 | Corrigir hit flash para sprite colorido | `Enemy`, shader/overlay | P1.3 | Flash visível em todos os inimigos |
| P1.7 | Corrigir `GameManager.Restart` por nome | `GameManager.cs` | — | Restart funciona fora do Build Settings |
| P1.8 | Centralizar fonte de verdade do layout de UI | `SwarmSceneBuilder.MakeText` | M-01 | Sem ajustes manuais perdidos |
| P1.9 | Limpar sprites com alpha/cores fora da paleta | `Art/Sprites/Projectiles`, `FX`, `*_death` | P1.1 | Validador de paleta passa |

**Riscos:** importar arte com escala errada; validar com uma cena de teste antes.
**Critério de saída:** build do WIP mostra a arte nova, sem erro, com uma run de 2 min.

---

# P2 — Balanceamento e experiência

**Objetivo:** provar que o loop atual é divertido e justo com a arte nova.

| # | Tarefa | Arquivos | Dependências | DoD |
|---|---|---|---|---|
| P2.1 | Documentar HP base vs HP no spawn do boss | `docs/01-GDD.md`, `EnemyData` | — | GDD e código concordam |
| P2.2 | Trocar `data.scale > 1.4f` por flag/`BossController` | `Enemy.cs`, `BossController` | — | Boss robusto a mudança de escala |
| P2.3 | Adicionar `[M]` no game over | `HudController` | — | Volta ao menu sem fechar o jogo |
| P2.4 | `ESC` fecha opções | `OptionsMenu` | — | Consistência de navegação |
| P2.5 | Rodar 10 runs com F1/CSV | — | P1 | Mediana, nível, vitória, boss |
| P2.6 | Ajustar boss/XP/spawn com 2 números por vez | `EnemySpawner`, `XpSystem`, `EnemyData` | P2.5 | Mediana 6–10 min, vitória possível |
| P2.7 | Prewarm do pool | `ObjectPool`/`GameManager` | — | Sem hitch no primeiro combate |

**Critério de saída:** 10 runs, 0 erros, mediana 6–10 min, boss 10–25 s, 7/10 “jogaria de novo”.

---

# P3 — Performance medida

**Objetivo:** sair de “parece ok” para “medido”.

| # | Tarefa | Arquivos | Dependências | DoD |
|---|---|---|---|---|
| P3.1 | Stress test 300/100/100 | cena/harness | P2 | Captura de Profiler |
| P3.2 | Layers + matriz de colisão | `TagManager`, `Physics2DSettings`, scripts | P3.1 | Menos pares, sem quebrar colisão |
| P3.3 | Sprite Atlas | `Art/Sprites` | P1 | Draw calls reduzidos |
| P3.4 | Cap de orbes/projéteis + reciclagem | `XpSystem`, `Weapon` | P3.1 | Sem picos de memória |
| P3.5 | Target provider compartilhado | `PlayerVisual`, `Weapon` | P3.1 | Sem O(N) duplicado por frame |
| P3.6 | Tick orbital 0,1 s | `OrbitalWeapon` | P3.1 | Redução de CPU sem perder feel |
| P3.7 | Testar Quality High/Medium (MSAA 0) | `QualitySettings` | P3.1 | 60 fps estável |

**Critério de saída:** 60 fps médio, p95 < 20 ms, GC < 64 B/frame, sem hitch > 50 ms.

---

# P4 — Polimento visual e sonoro

**Objetivo:** elevar a percepção de qualidade sem mudar o loop.

| # | Tarefa | Dependências | DoD |
|---|---|---|---|
| P4.1 | Fonte pixel para títulos/HUD | — | Títulos e HUD consistentes |
| P4.2 | Screenshots de gameplay (5) | P1 | Página do itch com imagens reais |
| P4.3 | 3 GIFs de 6–10 s | P1 | Divulgação |
| P4.4 | Trailer 30 s | P1/P4.2 | Link no itch/Pages |
| P4.5 | Ajustar ícones/feedback de dano | P1 | Legibilidade |
| P4.6 | Testar TrailRenderer na build | P1 | Rastro visível |
| P4.7 | Pixel Perfect Camera (opcional) | P3 | Nitidez em 1080p |

**Critério de saída:** página com 5 screenshots, 3 GIFs, trailer e identidade visual.

---

# P5 — Conteúdo adicional

**Objetivo:** só depois de P0–P4.

| Ordem | Conteúdo | Por quê |
|---|---|---|
| 1 | Novos inimigos/elites | variedade de horda |
| 2 | Novas armas/evoluções | builds distintas |
| 3 | 4º piloto | replay |
| 4 | Segundo mapa/setor | variedade visual |
| 5 | Conquistas/recordes | retenção |
| 6 | Leaderboard | comunidade |

**Não** adicionar conteúdo antes de validar performance e feedback.

---

# P6 — Teste externo e publicação

| # | Tarefa | DoD |
|---|---|---|
| P6.1 | 5–8 testadores com build atual | 5 respostas completas |
| P6.2 | Triagem com `triage.csv` | top 3 problemas |
| P6.3 | Corrigir bloqueadores/2+ ocorrências | build nova |
| P6.4 | Página itch com screenshots/GIFs/trailer | página pronta |
| P6.5 | Restricted → público | link público |
| P6.6 | Patch notes/devlog | comunicação |
| P6.7 | Teste em máquina limpa | build abre e roda |

**Critério de saída:** 5–8 testadores, 0 crash bloqueador, 60% chegam ao boss, 6/8 jogariam de novo.

---

## Sequência recomendada

```text
Hoje:
  P0.1 preservar WIP
  P0.2/P0.3 senha
  P0.4 conferir ZIP

Depois:
  P1.1 import pipeline
  P1.2/P1.3 sprites no jogo
  P1.7 restart
  P1.8 UI

Semana seguinte:
  P2 runs + ajuste
  P3 perfil
  P4 screenshots/GIF/trailer

Só então:
  P5 conteúdo
  P6 público
```

---

## Estimativas

| Etapa | Horas |
|---|---:|
| P0 | 4–8 h |
| P1 | 16–24 h |
| P2 | 16–24 h |
| P3 | 16–24 h |
| P4 | 24–40 h |
| P5 | 40–80 h |
| P6 | 20–40 h |

**Próxima tarefa concreta:** P0.1 — preservar o WIP em branch/commit.
