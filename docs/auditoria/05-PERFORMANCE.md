# 05 — Performance

**Meta declarada:** 60 fps com 300 inimigos, 100 projéteis e 100 orbes em uma RTX 3050 Laptop.  
**Evidência atual:** **nenhum Profiler capture** em `ProfilerCaptures/`; as metas são
intenção, não resultado comprovado.

---

## 1. Configuração atual

| Item | Valor |
|---|---|
| Câmera | Ortográfica, size 6 (12 unidades) |
| Resolução | 1920×1080 |
| Color Space | Linear |
| Qualidade atual | 5 (Ultra) |
| VSync | 1 no Ultra |
| MSAA | 2 no Ultra; 0 em Low/Medium |
| Post-processing | Bloom + Vignette |
| Inimigos máx. | 300 (`EnemySpawner.maxAlive`) |
| Spawn máx. | 18/s, lote até 4 |
| Projéteis | sem cap explícito |
| Orbes de XP | sem cap explícito |
| Pool | inimigos, projéteis, orbes, partículas, avisos |
| Prewarm | **não** |
| Sprite Atlas | **não** |
| Layers/matriz de colisão | **não configurados** |

---

## 2. Hot paths identificados

| Sistema | Frequência | Complexidade | Risco |
|---|---|---|---|
| `PlayerVisual.LateUpdate` | todo frame | O(N) em `Enemy.All` | Médio |
| `OrbitalWeapon.Update` | todo frame | O(orbs × N), até 8 × 300 | Médio/alto |
| `Enemy.Update` | todo frame | O(1) por inimigo | Médio (300+) |
| `Enemy.FixedUpdate` | 50 Hz | O(1) por inimigo | Médio |
| `XpOrb.Update` | todo frame | O(1) por orbe | Médio (muitos orbes) |
| `Projectile.Update` | todo frame | O(1) por projétil | Médio |
| `BurstParticle.Update` | todo frame | O(1) por partícula | Médio |
| `EnemySpawner.CanSpawn` | por spawn | O(N × tipos) | Baixo em 300 |
| `PlayerController.OnTriggerStay2D` | física | `GetComponent<Enemy>` | Médio |
| `HudController` | 10 Hz | strings só quando muda | Baixo |
| `DebugOverlay` | 5 Hz, se F1 | string 5 Hz | Baixo |
| `Sfx` | eventos | `PlayOneShot` + pitch | Baixo |

### Cálculo rápido

- `PlayerVisual`: 300 distâncias × 60 fps = **18.000/s**.
- `OrbitalWeapon`: 8 × 300 × 60 = **144.000 comparações/s**.
- 300 inimigos: 300 `Update` + 300 `FixedUpdate` + 300 Rigidbody2D + 300 colliders.
- 300 sprites + projéteis + partículas = draw calls e overdraw de Bloom.

São números plausíveis para 60 fps em uma RTX 3050, mas **não medidos**.

---

## 3. Física

`ProjectSettings/TagManager.asset` não tem layers customizadas.
`Physics2DSettings.m_LayerCollisionMatrix` está todo `ff` (todos os pares ligados).

Consequências:

- projétil × projétil;
- projétil × pickup;
- pickup × pickup;
- projétil × player;
- todos os pares testados.

Os callbacks filtram por `GetComponent`, mas o custo de broadphase/narrowphase existe.
Com 300 inimigos + 100 projéteis, isso pode dominar o frame.

### Correção recomendada

1. Criar layers `Player`, `Enemy`, `Projectile`, `Pickup`, `FX`.
2. Desligar pares inúteis na matriz.
3. Usar `LayerMask`/`CompareTag` em vez de `GetComponent` nos callbacks.
4. Medir antes/depois no Profiler.

**Não fazer isso sem teste**: mudar a matriz pode quebrar colisões de gameplay.

---

## 4. Render

- Material único `SpriteUnlit.mat` (bom).
- Texturas separadas para cada sprite procedural e para os 72 novos.
- Sem Sprite Atlas.
- Bloom + vinheta em 1920×1080.
- `TrailRenderer` em cada projétil; vários trails podem gerar overdraw.
- Partículas de morte/muzzle/thruster com SpriteRenderer.

### Riscos

| Risco | Impacto |
|---|---|
| Muitas texturas sem atlas | draw calls altos |
| Bloom em tela cheia | custo GPU constante |
| Muitos trails | overdraw e fill rate |
| 300 sprites com alpha | overdraw |
| Ultra/MSAA 2 em pixel art | custo desnecessário |

### Correção recomendada

- Sprite Atlas com todos os sprites de gameplay, padding 2, sem rotação, compressão None.
- Testar Quality “High” ou “Medium” (MSAA 0) para pixel art.
- Limitar/desativar trails em projéteis de horda, se necessário.
- Perfilar com e sem Bloom para saber o custo real.

---

## 5. Pooling e GC

### Pontos bons

- `ObjectPool` para inimigos, projéteis, orbes, partículas e avisos.
- HUD evita strings por frame.
- `Enemy.All` evita `FindObjectsOfType`.
- Sem LINQ no runtime.
- Sem `Instantiate`/`Destroy` no caminho quente principal (exceto orbes orbitais
  quando a quantidade muda e `new GameObject` no boss telegraph, eventos raros).

### Riscos

| Risco | Evidência | Impacto |
|---|---|---|
| Sem prewarm | `ObjectPool.Spawn` instancia no primeiro uso | hitch no primeiro combate |
| Sem cap de projéteis/orbes | nenhum limite em `Weapon`/`XpSystem` | picos de memória/CPU |
| `XpOrb.Update` por orbe | cada orbe faz `Distance` e `MoveTowards` | muitos orbes = custo |
| `Enemy` Update/FixedUpdate | 300 × 2 callbacks | CPU |
| `TrailRenderer` por projétil | cada projétil tem trail | GPU/overdraw |

### Correção recomendada

- Prewarm de 20–50 projéteis, 50 orbes, 50 partículas no `Start`.
- Cap de orbes (ex.: 150) com merge/coleta automática.
- Cap de projéteis por tiro/build (ex.: 40) ou pool com reciclagem.
- Testar um `OrbitalWeapon` com tick de 0,1 s em vez de todo frame.

---

## 6. Teste de stress reproduzível (planejado, não executado)

### Objetivo

Medir 60 fps com 300 inimigos + 100 projéteis + 100 orbes em 1920×1080 e 1280×720.

### Setup

1. Abrir `Swarm.unity`.
2. No `EnemySpawner`, setar:
   - `maxAlive = 300`
   - `baseRatePerSecond = 10`
   - `rateRampPerSecond = 0`
   - `maxRatePerSecond = 30`
3. No `Weapon`, dar dano 0 ou desativar a morte para manter a horda viva.
4. Invocar 300 inimigos via script/harness ou esperar o spawner.
5. Disparar 100 projéteis (build de Tiro Duplo/Gatilho).
6. Criar 100 orbes de XP no chão.

### Medições

- Profiler `CPU Usage` por 60 s.
- `BehaviourUpdate`, `Physics2D`, `Rendering`, `GC.Alloc`.
- Frame time p95.
- Draw calls (`Rendering`/Frame Debugger).
- Memória gerenciada e nativa.
- FPS no F1 overlay.

### Critérios

| Métrica | Alvo | Aceitável |
|---|---|---|
| FPS médio (horda) | 60 | 50 |
| Frame time p95 | 16,6 ms | 20 ms |
| GC Alloc/frame | 0 B | < 64 B |
| Draw calls | < 100 | < 200 |
| Sem hitch > 50 ms | sim | até 1 por run |

### Variações

- Com/sem Bloom.
- Quality Ultra vs High (MSAA 0).
- Com/sem Sprite Atlas.
- Com/sem layers de colisão.
- 1920×1080 vs 1280×720.

**Não executei esse teste nesta auditoria** porque não havia harness e para não
alterar a cena/WIP.

---

## 7. Orçamento e recomendação

O projeto está dentro de um orçamento plausível para um survivor-like 2D, mas o
gargalo provável não é o número de inimigos isolado: é a combinação de
**física sem layers + trails + sem atlas + loops O(N) por frame**.

### Prioridade

1. Medir antes de otimizar.
2. Layers/matriz de colisão.
3. Sprite Atlas.
4. Prewarm e caps.
5. Reduzir `FindNearest`/orbital para um tick compartilhado.
6. Testar Quality High/Medium.

**Não declare “suporta milhares de inimigos” sem o stress test.**
