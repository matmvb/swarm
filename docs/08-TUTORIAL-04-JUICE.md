# Tutorial 04 — Juice (o que separa "funciona" de "vicia")

**Objetivo:** fazer cada ação **sentir** algo. 45 min.

> Regra dos três canais: **todo evento importante precisa de som + movimento + cor.**
> Se falta um, o cérebro registra o evento como "não aconteceu".

---

## 1. O que já está ligado

| Evento | Som | Movimento | Cor |
|---|---|---|---|
| Tiro | `Sfx.PlayShoot` (blip 880 Hz) | projétil | ciano |
| Acerto | `PlayHit` (ruído 180 Hz + pitch aleatório) | flash branco | branco |
| Morte de inimigo | `PlayHit` grave | shake 0,03 | some |
| Dano no player | `PlayDeath` só na morte | shake 0,10 | barra cai |
| Level up | `PlayLevelUp` (arpejo) | tempo congela | painel |
| Coleta | `PlayPickup` | orbe voa | verde |

### Por que os sons são gerados por código

`Sfx.cs` sintetiza senoides e ruído com `AudioClip.Create`. Vantagens:
- zero asset, zero import, zero licença;
- pitch aleatório por tiro evita o "metralhadora de robô";
- o projeto roda em qualquer máquina.

Troque por samples reais quando tiver arte — a interface (`PlayHit()`, etc.) não muda.

---

## 2. Camera shake

`Scripts/Camera/CameraFollow.cs`:

```csharp
public void Shake(float amplitude, float duration)
{
    _shakeAmplitude = amplitude;
    _shakeTime = duration;
}
```

O shake usa `Time.unscaledDeltaTime` de propósito: se o jogo estiver congelado
(level up), a câmera **não** treme.

### Tabela de calibração

| Evento | Amplitude | Duração |
|---|---|---|
| Acerto comum | 0,02 | 0,05 s |
| Morte comum | 0,03 | 0,08 s |
| Dano no player | 0,10 | 0,12 s |
| Boss morrendo | 0,25 | 0,40 s |

> Shake demais = náusea e ilegibilidade. Se a tela treme a 2,5 tiros/s, **está errado**.
> Shake é para eventos raros e importantes.

---

## 3. Flash de dano

Em `Enemy.Update`:

```csharp
if (_flash > 0f)
{
    _flash -= Time.deltaTime * 6f;
    _sr.color = Color.Lerp(_baseColor, Color.white, Mathf.Clamp01(_flash));
}
```

Por que `Color.Lerp` e não trocar para branco e voltar de uma vez?
Porque o decaimento dá a sensação de **impacto** (rápido) sem piscar (epilético).

---

## 4. Ajuste de feel (faça um por vez)

Mude **um** número, jogue 1 min, decida. Nunca mexa em 5 de uma vez.

| Sensação | O que mexer |
|---|---|
| Tiro "molinho" | `projectileSpeed` 16 → 22 e `fireRate` 2,5 → 3 |
| Inimigos "de borracha" | `moveSpeed` do Drone 2,0 → 2,4 |
| Nível demora | `xpToNext` 5 + 4·n → 4 + 3·n |
| Muito fácil | `HpMult` 0,35 → 0,5 por minuto |
| Câmera "longe" | Orthographic Size 6 → 5,2 |
| Tela "vazia" | `maxAlive` 300 → 450 (medir FPS) |

---

## 5. Hitstop (o efeito mais poderoso por linha de código)

Breve congelamento no impacto. Implementação segura:

```csharp
// coloque no Sfx ou em um ScreenJuice
public void HitStop(float duration = 0.04f)
{
    if (_hitStopRunning) return;
    StartCoroutine(HitStopRoutine(duration));
}

IEnumerator HitStopRoutine(float duration)
{
    _hitStopRunning = true;
    float previous = Time.timeScale;
    Time.timeScale = 0.05f;
    yield return new WaitForSecondsRealtime(duration);
    Time.timeScale = previous;
    _hitStopRunning = false;
}
```

**Cuidado:** se um level up abrir no meio do hitstop, o `previous` pode ser 0.
Por isso o `_hitStopRunning` e por isso o level up sempre tem prioridade.

Aplique só em **crítico** e **morte de boss**. Em tudo, vira lag.

---

## 6. Checkpoint sensorial

Jogue 60 s e responda:

- [ ] Eu sei quando acertei sem olhar a barra de vida do inimigo? (som)
- [ ] Eu sei quando levei dano sem olhar o HUD? (shake + som)
- [ ] O level up parece uma recompensa ou uma interrupção?
- [ ] Consigo jogar com o som mudo e ainda entender? (cor + movimento)
- [ ] Consigo jogar sem olhar e ainda entender? (som)

Se as duas últimas estão OK, o juice está equilibrado.

---

## 7. Desafio (30 min)

Adicione **números de dano**: um objeto poolado com `TextMesh` que sobe 0,5 unidade,
desaparece em 0,4 s e é **desativado**, nunca destruído.

Restrições:
- máximo 40 números vivos (acima disso, ignore);
- crítico em amarelo e 1,4x maior;
- sem `Instantiate`/`Destroy` em gameplay.
