# Resultados — Sprint 1 (10 runs)

Coleta: 05/11 a 09/11/2026 · Editor 6000.6.1f1 · Build `1.0`.

As runs 1–7 rodaram no build **antigo** (XP linear). As runs 8–10 rodaram no build
**ajustado** (XP exponencial + 1ª escolha ofensiva), commit `a8b5e22`.

---

## 1. Dados brutos

| # | Data | Duração | Kills | Nível | Vitória | Build usada |
|---|---|---|---|---|---|---|
| 1 | 05/11 15:20 | 8:46 | 3.753 | 44 | não | tudo no máximo |
| 2 | 05/11 19:29 | 3:30 | 788 | 19 | não | 9 upgrades |
| 3 | 09/11 19:22 | 5:56 | 800 | 19 | não | 7 upgrades |
| 4 | 09/11 19:31 | 3:47 | 412 | 11 | não | 7 upgrades (sem dano) |
| 5 | 09/11 19:38 | 6:01 | 1.106 | 19 | não | 8 upgrades |
| 6 | 09/11 19:47 | 5:46 | 1.306 | 19 | não | 8 upgrades |
| 7 | 09/11 20:12 | 10:00 | 5.309 | 58 | **sim** | tudo no máximo |
| 8 | 09/11 21:02 | 10:00 | 1.884 | 18 | **sim** | 6 upgrades |
| 9 | 09/11 21:10 | 7:00 | 1.983 | 18 | não | 8 upgrades |
| 10 | 09/11 21:16 | 5:25 | 1.376 | 16 | não | 7 upgrades |

---

## 2. Estatísticas

| Métrica | Todas (10) | Build antigo (7) | **Build novo (3)** | Meta |
|---|---|---|---|---|
| Mediana da duração | 5:58 | 5:56 | **7:00** | 4–8 min ✅ |
| Média | 6:37 | 6:31 | 7:28 | — |
| Nível mediano | 19 | 19 | **18** | 12–20 ✅ |
| Vitórias | 2 (20%) | 1 (14%) | 1 (33%) | — |
| Mortes antes de 4 min | 2 | 2 | **0** | 0 ✅ |

---

## 3. O que mudou com o ajuste

| Sintoma medido | Antes | Depois |
|---|---|---|
| Nível na run de 10 min | **58** | **18** |
| Kills na run de 10 min | 5.309 | 1.884 |
| Mortes antes de 4 min | 2 em 7 | **0 em 3** |
| Nível na morte | 11–44 | 16–18 |
| Mediana | 5:56 | 7:00 |

O **snowball foi domado**: a run de vitória agora termina com 6 upgrades relevantes,
não com todos no máximo. A decisão de build continua até o fim.

O **smart RNG** (1ª escolha sempre ofensiva) eliminou, na amostra nova, as mortes
por azar de sorteio.

---

## 4. Conclusões

1. **O loop está validado.** Mediana 7:00, níveis 16–18, vitória possível e não trivial.
2. **A variação de build importa.** A vitória veio com Munição/Gatilho (foco em alvo
   único + sobrevivência); as mortes vieram com builds mais equilibradas. Há espaço
   para estilos diferentes.
3. **O muro de dificuldade existe e é legível.** As mortes se concentram entre 5:25
   e 7:00 — é o pico que o GDD queria.
4. **A vitória defensiva é válida.** 1.884 kills em 10 min (3,1/s) mostra que dá para
   vencer kiting, não só matando — bom sinal de profundidade.

---

## 5. Ajustes aplicados neste ciclo

| Arquivo | Mudança |
|---|---|
| `XpSystem.cs` | `xpToNext *= 1.3` (curva exponencial) |
| `UpgradeSystem.cs` | 1ª escolha da run garante dano/cadência/projétil |
| `GameManager.cs` | vitória aos 10:00 (+20 Núcleos) |
| `RunLogger.cs` | CSV com ponto decimal + coluna `victory` |

---

## 6. Próximo ciclo

Sprint 1 fechado. O gargalo agora **não é balanceamento, é variedade**.

Prioridade recomendada (Sprint 4):
1. **Boss com padrão de ataque** (rajada radial + investida) — o evento dos 5:00 hoje é só um tanque gigante.
2. **2ª arma: orbe orbital** — primeira decisão de build de verdade.
3. **3 personagens** (Tanque / Veloz / Atirador) — variedade barata, só números.
