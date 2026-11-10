# Boss Núcleo — iterações de tuning

Objetivo: fazer dos 5:00 o **ponto alto** da run, não um saco de HP.

---

## Linha do tempo

| Versão | Mudança | Resultado medido |
|---|---|---|
| 1 | 600 HP; telegrafo só pulsava a escala | "morreu rápido"; depois **159,8 s** com F2 aos 2:10 (arma atirava nos adds) |
| 2 | 1.200 HP; halo vermelho + som; arma foca o boss; horda para de nascer | **63,3 s** aos 5:00 — o boss lento saía do alcance de 7,5 |
| 3 | **Núcleo limpa o campo ao nascer** (duelo) | **14,7 s** ✅ |

---

## O que aprendemos

1. **Mira automática é design, não detalhe.** Com a arma mirando no mais próximo, o boss levou 160 s porque a horda absorvia os tiros. Priorizar o boss foi a mudança de maior impacto.
2. **Alcance + boss lento = fuga.** Se o jogador kitava a horda, o boss ficava fora de 7,5 e a arma voltava para os adds. Limpar o campo resolveu alcance e legibilidade de uma vez.
3. **Telegrafo precisa de mais de um canal.** Escala sozinha passou batido. Halo vermelho + som + pausa da horda fizeram o ataque ser lido.
4. **Um número por vez.** HP sozinho não resolvia: 600 → 1.200 não mudou a sensação enquanto o alvo estava errado.

---

## Parâmetros finais

| Parâmetro | Valor |
|---|---|
| HP base (× dificuldade no 5:00 ≈ 2,75) | 1.200 |
| Rajada | 14 projéteis · 8 de dano · a cada 5 s · telegrafo 0,75 s |
| Investida | 10 u/s · 0,5 s · a cada 11 s · telegrafo 0,65 s |
| Recuperação após ataque | 0,8 s |
| Comportamento | limpa a horda, pausa spawns, arma foca o boss |
| Recompensa | 40 XP + 20 Núcleos se sobreviver aos 10:00 |

---

## Próximo

2ª arma (orbe orbital) e 3 personagens — ver `02-PLANO-DEV.md`, Sprint 4.
