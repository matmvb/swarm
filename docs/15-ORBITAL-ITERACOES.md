# Orbe orbital — iterações

Objetivo: criar a primeira arma alternativa de verdade (área/defesa vs tiro focado).

---

## Iterações

| Versão | Mudança | Resultado |
|---|---|---|
| 1 | 0,8× dano · raio 0,45 · 180°/s | 155 kills/min · nível 16 · "acerta, mas parece fraco" |
| 2 | 1,6× dano · raio 0,70 · 160°/s | **287 kills/min** (+85%) · nível 19 · **boss 13,2 s** |

Comparação com as builds de tiro: 226–282 kills/min. O orbe agora **compete** com o tiro, sem substituí-lo.

---

## O que aprendemos

1. **Contato transitório engana.** O cooldown de 0,3 s sugeria dano contínuo, mas o orbe passava raspando: ~1 toque por volta. Aumentar o **raio de acerto** e o **dano por toque** resolveu; desacelerar a rotação ajudou.
2. **Dano escala com a arma.** Como o orbe usa `PlayerStats.damage`, todo upgrade de dano fortalece os dois — é a sinergia que faz a build valer.
3. **Medir kills/min é melhor que "parece fraco".** A percepção estava certa, mas o número mostrou a magnitude (quase dobrou).
4. **O tempo do boss agora vai para o CSV** (`boss_time`), então não dependemos mais de ler o toast.

---

## Parâmetros finais

| Parâmetro | Valor |
|---|---|
| Dano | 1,6 × dano da arma |
| Intervalo por inimigo | 0,3 s |
| Raio de acerto | 0,70 |
| Raio da órbita | 3,0 local (~2,4 no mundo) |
| Rotação | 160°/s |
| Stacks do upgrade | 3 |
| Atalho de teste | F3 (editor) |

---

## Observação

O teste usou **F3 + 2 upgrades**, então rodou com ~5 orbes (acima do máximo normal de 3).
Mesmo assim o boss levou 13,2 s — dentro do alvo. Com 3 orbes o tempo deve ficar um pouco maior,
o que é o comportamento desejado.
