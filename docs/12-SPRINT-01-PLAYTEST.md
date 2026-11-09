# Sprint 1 — Playtest e calibragem

**Meta:** transformar "funciona" em "eu quero jogar de novo".
**Duração:** 1 semana (~15 h). **Não adicione conteúdo neste sprint.**

---

## 1. Ferramentas instaladas

| Ferramenta | Como usar | Para quê |
|---|---|---|
| **Overlay (F1)** | aperte `F1` no jogo | FPS, tempo, kills, inimigos vivos, nível, dano, cadência |
| **CSV de runs** | automático ao morrer | `%USERPROFILE%\AppData\LocalLow\Scoty\SWARM\swarm-runs.csv` |
| **Validador** | `SWARM > Validar Cena` | conferir referências antes de testar |

O CSV grava uma linha por run:

```
timestamp,version,duration_s,kills,level,victory,build
2026-11-09 19:22:10,1.0,412.3,318,17,0,"Munição Pesada x2, Tiro Duplo x1, Ímã x1"
```

> `duration_s` usa **ponto** decimal (padrão internacional). `victory = 1` quando você
> sobrevive aos 10:00 (a run termina em vitória, com +20 Núcleos).

---

## 2. Protocolo (faça nesta ordem)

### Dia 1 — 10 runs (40 min)

1. Abra `Assets/Swarm/Scenes/Swarm.unity`.
2. Aperte `F1` para ver o overlay.
3. Jogue **10 runs seguidas**, sem mexer em nada.
4. A cada morte (ou vitória aos 10:00), anote no papel (ou confie no CSV):
   - em que minuto terminou;
   - o que te matou (horda? tanque? boss?);
   - de 1 a 5, o quanto você quis jogar de novo.

**Não ajuste nada no dia 1.** Você precisa da linha de base.

### Dia 2 — leitura fria

Abra o CSV. Calcule:

| Métrica | Meta Sprint 1 | Como |
|---|---|---|
| Mediana da duração | **4 a 8 min** | ordenar e pegar o do meio |
| Level ups por run | **12 a 20** | coluna `level` |
| "Jogaria de novo" | **≥ 7 de 10** | sua nota |
| FPS mínimo (F1) | **≥ 50** | observar na horda |

### Dias 3–4 — ajuste (2 números por vez)

| Sintoma | O que mexer | Onde |
|---|---|---|
| Morre antes de 3 min | `baseRatePerSecond` 1,2 → 0,9 | EnemySpawner |
| Morre depois de 9 min | `rateRampPerSecond` 0,028 → 0,036 | EnemySpawner |
| Level up demora | `xpStepPerLevel` 4 → 3 | XpSystem |
| Level up demais | `xpStepPerLevel` 4 → 5 | XpSystem |
| Tiro "molinho" | `projectileSpeed` 16 → 22 | PlayerStats |
| Movimento "pesado" | `moveSpeed` 5 → 5,5 | PlayerStats |
| Horda "vazia" | `maxAlive` 300 → 400 | EnemySpawner |
| Dano injusto | `contactInterval` 0,4 → 0,6 | PlayerController |

**Regra:** mude **dois números**, jogue 3 runs, compare. Se melhorou, mantenha; se não, reverta.
Anote cada mudança no commit (`tune: spawn 1.2 -> 0.9`).

### Dia 5 — teste cego com 3 pessoas

Sem explicar nada. Só observe:

- [ ] Elas descobrem que o tiro é automático em < 10 s?
- [ ] Elas leem as 3 opções em < 3 s?
- [ ] Elas reiniciam sozinhas?
- [ ] Elas reclamam de algo que você não tinha visto?

Use `docs/feedback-template.md`.

---

## 3. Teste do núcleo (opcional, mas revelador)

Desligue o progresso por 20 minutos:

1. Selecione `Player` → componente **Xp System** → desmarque a caixa (disable).
2. Jogue.

Se desviar e atirar **sem upgrades** ainda for gostoso, o núcleo está pronto.
Se não for, nenhum upgrade vai salvar — volte para o movimento e o ritmo de spawn.

**Reative o componente depois.**

---

## 4. Critérios de saída do Sprint 1

- [ ] 10 runs registradas no CSV.
- [ ] Mediana entre 4 e 8 min.
- [ ] 12–20 level ups por run.
- [ ] 7 de 10 "jogaria de novo".
- [ ] Nenhum erro no Console.
- [ ] Commit `tune:` com os números finais.
- [ ] `SWARM > Validar Cena` = 0 problemas.

Se passar, o Sprint 2 (XP/upgrades) já está pronto no código — você vai **refinar**, não construir.

---

## 5. O que NÃO fazer agora

- ❌ Boss, armas novas, personagens (Sprint 4).
- ❌ Arte definitiva (Sprint 4+).
- ❌ Menu, opções, save de config.
- ❌ Refatorar arquitetura.
- ❌ Mexer em mais de 2 números por rodada.

> O maior erro de um dev solo é adicionar conteúdo para fugir da pergunta
> "o loop é divertido?". Responda essa primeiro.
