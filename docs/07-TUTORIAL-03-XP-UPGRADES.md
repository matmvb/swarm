# Tutorial 03 — XP, level up e upgrades

**Objetivo:** dominar o sistema que faz o jogador querer "só mais uma". 45 min.

---

## 1. O ciclo de dopamina

```
mata → orbe → XP → barra enche → TELA CONGELA → 3 opções → escolhe → mais forte
```

O congelamento (`Time.timeScale = 0`) é essencial: dá **peso** à escolha. Se o jogo
continuasse rodando, o jogador escolheria no automático e não sentiria a recompensa.

---

## 2. XP e níveis

`Scripts/Progression/XpSystem.cs`:

```csharp
xpToNext = Mathf.RoundToInt(xpToNext * 1.3f);   // 5, 7, 9, 12, 15, 20...
```

Curva **exponencial** (fator 1.3). Medimos 7 runs: com a curva linear antiga, uma
run de 10 min chegou ao **nível 58** e maxou todos os upgrades. A exponencial
mantém 14–20 níveis nas runs normais e impede o snowball de virar passeio.

### Fila de level ups

Matar o boss pode dar XP para 2 níveis de uma vez. Sem fila, a segunda escolha
apareceria **por cima** da primeira. O `pending` resolve:

```csharp
pending++;                        // cada nível ganho
...
hud.ShowLevelUp(...);             // abre a primeira
// ao escolher:
pending--;
if (pending > 0) ShowLevelUp(...); // abre a próxima
else Time.timeScale = 1f;          // volta ao jogo
```

---

## 3. UpgradeData (o dado)

Selecione `Assets/Swarm/Data/Upgrades/MunicaoPesada.asset` no Project:

| Campo | Valor | Significado |
|---|---|---|
| `Title` | Munição Pesada | nome na tela |
| `Description` | +20% de dano | texto curto, começa com o número |
| `Stat` | Damage | qual número muda |
| `Amount` | 20 | valor |
| `Percent` | ✅ | 20 = +20% (se falso, +20 absoluto) |
| `Max Stacks` | 5 | quantas vezes pode ser oferecido |

### Regra de ouro do texto

O jogador lê 3 opções em ~1,5 s. **Número primeiro, adjetivo depois.**
`+20% dano` vence `Aumenta significativamente o poder de fogo`.

---

## 4. Sorteio

`UpgradeSystem.Roll(3)`:

1. monta a lista de candidatos (ignora os que já estão no `maxStacks`);
2. sorteia 3 **sem repetição**;
3. devolve.

Se só houver 2 candidatos, devolve 2 — e o jogador vê 2. Isso é melhor que
oferecer um upgrade esgotado.

---

## 5. Criar um upgrade novo (faça agora)

1. Botão direito em `Assets/Swarm/Data/Upgrades` → `Create > SWARM > Upgrade`.
2. Nome do arquivo: `CascaDensa`.
3. Preencha:
   - Title: `Casca Densa`
   - Description: `+15% vida máxima`
   - Stat: `MaxHealth`
   - Amount: `15`
   - Percent: ✅
   - Max Stacks: `3`
4. Selecione o `Player` → componente `Upgrade System` → arraste o asset para a lista `Pool`.

**Pronto, sem uma linha de código.** É para isso que existe ScriptableObject.

---

## 6. Checkpoint

- [ ] Barra de XP sobe ao coletar orbes.
- [ ] Ao encher, o jogo congela e aparecem 3 opções.
- [ ] `1/2/3` aplica e o jogo volta **imediatamente**.
- [ ] O efeito é visível: dano maior, tiro mais rápido, mais vida etc.
- [ ] Matar muitos inimigos de uma vez mostra uma escolha, depois a outra.
- [ ] O nível no HUD acompanha.

---

## 7. Erros comuns

| Sintoma | Causa | Solução |
|---|---|---|
| Jogo congela para sempre | `Time.timeScale` ficou em 0 | verifique se `Choose()` chama `ConsumePending` |
| Escolha não aplica | `stats`/`health` nulos no `UpgradeSystem.Apply` | religue no gerador de cena |
| Mesmo upgrade 3x na tela | sorteio sem remover candidato | use `candidates.RemoveAt(idx)` |
| Texto cortado | `levelUpOptions` pequeno | aumente o `RectTransform` no HUD |
| Nível sobe sem barra | `OnChanged` não inscrito | confira `hud.xp` no Inspector |

---

## 8. Desafio (30 min)

Crie um upgrade **com trade-off**: `Sobrecarga` — `+30% dano` e `−15% vida máxima`.

Você vai precisar de **dois efeitos** no mesmo upgrade. Duas saídas:

- **Simples:** `UpgradeSystem.Apply` com um `case StatType.Sobrecarga` que mexe nos dois.
- **Escalável:** trocar `StatType` por uma `List<StatModifier>` no `UpgradeData`.

Faça a simples agora; a escalável só quando tiver 30+ upgrades.
