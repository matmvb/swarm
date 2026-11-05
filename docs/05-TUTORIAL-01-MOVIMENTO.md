# Tutorial 01 — Movimento e o primeiro Play

**Objetivo:** entender os objetos da cena e calibrar o movimento. 30 min.

---

## 1. Abrir a cena

1. Na janela **Project**, navegue até `Assets/Swarm/Scenes`.
2. Clique duas vezes em `Swarm`.
3. Aperte **Play** (`Ctrl+P`).

Você deve ver um quadrado ciano no centro, quadrados vermelhos vindo de fora e
o ciano atirando sozinho. Aperte **WASD**.

> Se não houver nada, rode `SWARM > Montar Protótipo Completo` e abra a cena de novo.

---

## 2. O que existe na cena

Selecione os objetos na janela **Hierarchy**:

| Objeto | O que é |
|---|---|
| `Main Camera` | câmera ortográfica (size 6) com `CameraFollow` |
| `GameManager` | dono do estado da run |
| `Player` | a nave: `Rigidbody2D`, `CircleCollider2D` (trigger), `PlayerController`, `PlayerStats`, `Health`, `Weapon`, `XpSystem`, `UpgradeSystem` |
| `HUD` | Canvas com textos e barras |
| `EnemySpawner` | cria inimigos no anel de raio 13 |
| `Sfx` | fonte de áudio + sons gerados |
| `Templates` | **desativados** de propósito: molde de projétil, orbe e inimigos |

> **Por que templates desativados?** O pool clona esses objetos. Se ficassem ativos,
> apareceriam parados na cena e entrariam na lista de inimigos vivos.

---

## 3. Como o movimento funciona

Abra `Assets/Swarm/Scripts/Player/PlayerController.cs`:

```csharp
void FixedUpdate()
{
    if (GameManager.Instance == null || !GameManager.Instance.IsRunning)
    {
        _rb.linearVelocity = Vector2.zero;
        return;
    }
    _rb.linearVelocity = InputReader.Move * stats.moveSpeed;
}
```

Três decisões importantes:

1. **`FixedUpdate`**, não `Update`: física roda em passo fixo (0,02 s). Mover corpo
   rígido no `Update` causa tremida em máquinas com FPS alto.
2. **`linearVelocity`** (Unity 6): substitui o antigo `velocity`.
3. **Sem normalizar aqui**: o `InputReader` já normaliza, então diagonal não é mais rápida.

### O InputReader

`Scripts/Core/InputReader.cs` lê teclado, setas e gamepad. O bloco `#if ENABLE_INPUT_SYSTEM`
faz o mesmo arquivo funcionar com o pacote novo **ou** com o input legado.

---

## 4. Calibrar o movimento (o exercício de verdade)

Selecione `Player` → Inspector → componente **Player Stats**.

| Campo | Padrão | Teste |
|---|---|---|
| `Move Speed` | 5 | 4 (pesado) · 6 (ágil) · 8 (arcade) |
| `Pickup Radius` | 2.5 | 2 (precisa ir buscar) · 4 (confortável) |

Regra prática: a nave deve atravessar a tela (≈ 21 unidades) em **~4 segundos**.
Com `moveSpeed = 5`, dá 4,2 s. Está no ponto.

Teste e responda: *o movimento parece responsivo ou escorregadio?* Se escorregadio,
reduza `smooth` do `CameraFollow` (na câmera) para 0,08 e teste de novo.

---

## 5. Checkpoint

- [ ] A nave anda nas 8 direções, na mesma velocidade na diagonal.
- [ ] Ao parar de teclar, para imediatamente (não desliza).
- [ ] A câmera segue sem tremer.
- [ ] Ao colidir com inimigo, você perde vida (a barra vermelha cai).

---

## 6. Erros comuns

| Sintoma | Causa | Solução |
|---|---|---|
| Não anda | `Player Stats` não atribuído no `PlayerController` | arraste `Player` para o campo `Stats` |
| Anda atravessando parede | colisor do player é `Is Trigger` | **é intencional**; não mude |
| Anda "travado" | `GameManager.IsRunning = false` | você morreu; aperte `R` |
| Diagonal mais rápida | `InputReader` alterado | use `normalized` no vetor |
| Tremida | movimento no `Update` | mantenha no `FixedUpdate` |

---

## 7. Desafio (15 min)

Adicione um **dash** com `Espaço`:

1. Em `InputReader`, crie `GameKey.Dash` (use `kb.spaceKey` / `KeyCode.Space`).
2. Em `PlayerController`, guarde `_dashCooldown` e, ao apertar, aplique
   `_rb.linearVelocity = direção * (moveSpeed * 3f)` por 0,12 s.
3. Coloque um cooldown de 1,5 s.

Se você conseguiu sem quebrar nada, você entendeu o loop de input → física.
