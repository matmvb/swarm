# Performance e QA

Meta: **60 fps estáveis com 300 inimigos, 100 projéteis e 100 orbes** em uma máquina
modesta (como a sua RTX 3050 Laptop).

---

## 1. Como medir (não adivinhe)

1. `Window > Analysis > Profiler` (`Ctrl+7`).
2. Play → deixe rodar 2 min.
3. Selecione um frame no meio da horda.
4. Olhe a coluna **GC Alloc**: precisa ser **0 B** por frame no gameplay.

### Onde olhar

| Módulo | O que significa |
|---|---|
| `PlayerLoop` | tempo total do frame (16,6 ms = 60 fps) |
| `Physics2D` | simulação de colisão |
| `BehaviourUpdate` | seus `Update`s |
| `Rendering` | desenho |
| `GC.Collect` | **nunca** deve aparecer no meio da run |

---

## 2. Checklist

### Código

- [ ] Nenhum `Instantiate`/`Destroy` durante a run (só no pool).
- [ ] Nenhum `GetComponent` dentro de `Update`/`FixedUpdate`.
- [ ] Nenhum `FindObjectOfType` / `FindGameObjectWithTag` em loop.
- [ ] Nenhum LINQ (`Where`, `Select`, `OrderBy`) em `Update`.
- [ ] Nenhuma concatenação de string por frame (`"Kills " + n` aloca!). Use `TMP`/cache.
- [ ] `Enemy.All` é `List<Enemy>`, não `FindObjectsOfType`.
- [ ] Eventos desinscritos em `OnDestroy`.

> **Atenção ao HUD:** `timerText.text = ...` a cada frame aloca e força rebuild de
> layout. No MVP é tolerável; para shipping, atualize 10x/s ou quando o valor mudar.

### Física

- [ ] `Fixed Timestep = 0.02`.
- [ ] Colisores são círculos (`CircleCollider2D`), não polígonos.
- [ ] **Collision Matrix** desliga pares inúteis (ex.: projétil × projétil).
- [ ] `Rigidbody2D` dos inimigos com `Sleep` permitido (padrão).
- [ ] Nenhum `Rigidbody2D` dinâmico sem necessidade.

### Render

- [ ] Um único material para todos os sprites (Sprite Lit/Unlit padrão).
- [ ] `Sprite Atlas` se houver mais de ~50 sprites diferentes.
- [ ] Sem luzes 2D no MVP (custa caro e não muda o feel).
- [ ] Sem post-processing pesado (bloom é o único que vale a pena).

---

## 3. Camadas (fazer no Sprint 6)

Crie em `Edit > Project Settings > Tags and Layers`:

| Layer | Uso |
|---|---|
| `Player` | nave |
| `Enemy` | hordas |
| `Projectile` | tiros |
| `Pickup` | orbes |

Depois desligue na **Collision Matrix**:

- `Projectile × Projectile` ❌
- `Pickup × Pickup` ❌
- `Pickup × Enemy` ❌
- `Enemy × Enemy` ✅ (precisa para se espalhar)
- `Projectile × Enemy` ✅
- `Player × Enemy` ✅

Isso reduz o número de pares que a física testa por frame.

---

## 4. Problemas típicos e solução

| Sintoma | Causa provável | Solução |
|---|---|---|
| Travada a cada ~5 s | GC | eliminar alocação (strings, LINQ, `new`) |
| FPS cai com a horda | física | layers + colisores círculo + menos corpos |
| FPS cai ao coletar | muitos orbes | mover orbes em `Update` (já é), limitar a 150 |
| Primeiro tiro trava | `Instantiate` | pré-aquecer o pool (`Spawn` 20 e `Despawn` no Start) |
| Tela "borrada" em movimento | filtro bilinear no sprite | `Filter Mode = Point` |
| Build mais lento que o editor | IL2CPP | normal; teste com Mono durante o dev |

### Pré-aquecer o pool

```csharp
void Start()
{
    var prewarm = new List<GameObject>();
    for (int i = 0; i < 30; i++)
        prewarm.Add(ObjectPool.Spawn(projectileTemplate, transform.position, Quaternion.identity));
    foreach (var go in prewarm) ObjectPool.Despawn(go);
}
```

Evita o micro-freeze do primeiro combate.

---

## 5. QA manual (rodar antes de cada build)

### Gameplay

- [ ] Mover nas 4 diagonais e nas bordas da tela.
- [ ] Ficar parado: os inimigos chegam e te matam? (sim, esperado)
- [ ] Atravessar a horda sem travar.
- [ ] Subir 5 níveis seguidos sem quebrar a fila.
- [ ] Matar 50 inimigos em 1 s (boss) — sem erro.
- [ ] Morrer e reiniciar 5x seguidas.

### UI

- [ ] HUD legível em 1280x720.
- [ ] Texto de upgrade cabe em 1 linha por opção.
- [ ] Tela de fim mostra tempo, kills, nível e Núcleos.
- [ ] Números não ficam negativos ou `NaN`.

### Sistema

- [ ] Fechar e abrir mantém Núcleos.
- [ ] Trocar de janela e voltar não pausa errado.
- [ ] Alt+Enter alterna fullscreen sem quebrar.

---

## 6. Orçamento final

| Métrica | Alvo | Aceitável |
|---|---|---|
| FPS médio (horda máxima) | 60 | 50 |
| Frame time p95 | 16,6 ms | 20 ms |
| GC Alloc/frame | 0 B | < 64 B |
| Tempo de load | < 3 s | 5 s |
| Tamanho do build | < 200 MB | 400 MB |

Se estiver dentro, **pare de otimizar**. Otimização sem Profiler é hobby, não trabalho.
