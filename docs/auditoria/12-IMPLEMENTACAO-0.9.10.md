# SWARM 0.9.10 — Combat Visual Polish

**Relatório de implementação e validação das Fases 0 e 1 da auditoria artística.**

| Item | Valor |
|---|---|
| Versão resultante | **0.9.10 — Combat Visual Polish** |
| Commit base | `f661a9d` |
| Versão anterior recuperável | `v0.9.9-demo` |
| Escopo | Fases 0 e 1 da auditoria de 26/11/2026 |
| Status | Implementado, compilado e testado em smoke test |
| Commit das alterações | **Não realizado** |
| Upload para itch.io | **Não realizado** |
| Senha do playtest | **Não alterada** |
| Fases 2–5 | **Não iniciadas** |

---

## 1. Objetivo

Implementar as correções de integridade e leitura de combate identificadas na auditoria pós-0.9.9, preservando:

- Pixel art espacial sci-fi.
- Jogador predominantemente ciano.
- Inimigos com cores próprias por classe.
- Núcleo vermelho.
- XP dourado.
- Fundo escuro `#05070F`.
- Paleta semântica da interface.
- Bloom contido.
- Silhuetas e proporções existentes.

A regra visual aplicada foi:

> O jogador é identificado pelo ciano; os inimigos pelas cores de suas classes; o branco representa impacto ou energia intensa, não a cor padrão de todos os efeitos.

---

## 2. Arquivos modificados

### Assets de projéteis e XP

- `Assets/Swarm/Art/Sprites/Projectiles/proj_player.png` + `.meta`
- `Assets/Swarm/Art/Sprites/Projectiles/proj_enemy.png` + `.meta`
- `Assets/Swarm/Art/Sprites/Projectiles/proj_boss.png` + `.meta`
- `Assets/Swarm/Art/Sprites/Projectiles/xp_orb.png` + `.meta`

### Pipeline de importação

- `Assets/Swarm/Editor/ArtImportPipeline.cs`

### Combate, cores e FX

- `Assets/Swarm/Scripts/Combat/CombatPalette.cs` **novo** + `.meta`
- `Assets/Swarm/Scripts/Combat/Projectile.cs`
- `Assets/Swarm/Scripts/Combat/EnemyProjectile.cs`
- `Assets/Swarm/Scripts/Combat/DeathBurst.cs`
- `Assets/Swarm/Scripts/Combat/Enemy.cs`
- `Assets/Swarm/Scripts/Combat/BossController.cs`
- `Assets/Swarm/Scripts/Combat/RangedEnemy.cs`
- `Assets/Swarm/Scripts/Combat/SpawnWarning.cs`
- `Assets/Swarm/Scripts/Pickup/XpOrb.cs`
- `Assets/Swarm/Scripts/Player/OrbitalWeapon.cs`
- `Assets/Swarm/Scripts/Player/Weapon.cs`

### Cena, build e versão

- `Assets/Swarm/Editor/SwarmSceneBuilder.cs`
- `Assets/Swarm/Scenes/Swarm.unity`
- `Assets/Swarm/Editor/BuildScript.cs`
- `ProjectSettings/ProjectSettings.asset`
- `README.md`

---

## 3. Fase 0 — Integridade dos assets

### 3.1 Diagnóstico dos PNGs

Os quatro arquivos apresentavam **bytes de filtro de scanline inválidos**. O padrão PNG aceita apenas valores `0–4`.

| Arquivo | Filtro inválido | Linha |
|---|---:|---:|
| `proj_player.png` | `127` | 4 |
| `proj_enemy.png` | `127` | 4 |
| `proj_boss.png` | `153` | 6 |
| `xp_orb.png` | `127` | 4 |

A análise confirmou:

- Assinatura PNG válida.
- Chunks `IHDR`, `IDAT` e `IEND` presentes.
- CRCs dos chunks válidos.
- Dimensões e interlace válidos.
- Compressão zlib válida.
- Stream descomprimido com o tamanho esperado.
- O problema estava restrito ao byte de filtro da scanline.

### 3.2 Correção

- Decodificação tolerante dos quatro arquivos.
- Preservação de dimensões, frames, transparência e pixels.
- Reexportação com encoder PNG padrão, usando filtro `0` em todas as scanlines.
- Comparação do decode antes/depois: pixels preservados.
- Validação com decodificador padrão: os quatro arquivos agora abrem normalmente.

As dimensões foram mantidas:

| Arquivo | Dimensão | Frames |
|---|---:|---:|
| `proj_player.png` | 16×8 | 2 |
| `proj_enemy.png` | 16×8 | 2 |
| `proj_boss.png` | 36×12 | 3 |
| `xp_orb.png` | 32×8 | 4 |

### 3.3 ArtImportPipeline

O pipeline agora valida:

- Assinatura PNG.
- Chunks obrigatórios: `IHDR`, `IDAT`, `IEND`.
- CRC de cada chunk.
- Dimensões contra o manifesto (`frameWidth`, `frames`, `sheetWidth`, `sheetHeight`).
- Bit depth, color type, compressão, filtro e interlace.
- Stream zlib/deflate.
- Tamanho esperado das scanlines.
- Filtros de scanline inválidos.
- Transparência, com aviso quando não há pixels transparentes.
- Arquivo ausente ou inválido.

Mensagens de erro incluem caminho, motivo e linha do filtro.

Também foram adicionados:

- Menu `SWARM > Arte > Validar PNGs`.
- Batch `ArtImportPipeline.ValidateAllBatch`.
- Falha com código 1 no batch de importação quando há erro de integridade.

### 3.4 Teste negativo do validador

O `proj_player.png` original inválido foi copiado temporariamente de volta e validado:

```text
[SWARM] PNG inválido: .../proj_player.png — filtro de scanline inválido na linha 4: 127
[SWARM] Validação de PNGs: 73 verificados, 1 erro(s).
```

O processo saiu com código 1. O arquivo corrigido foi restaurado em seguida.

---

## 4. Fase 1 — Identidade e leitura do combate

### 4.1 CombatPalette

Nova paleta semântica de combate:

| Uso | Cor |
|---|---|
| Jogador | `#4DFFE0` |
| Núcleo do jogador | `#EBF7FF` |
| Crítico do jogador | `#FF8C2E` |
| Muzzle do jogador | `#4DFFE0` |
| Projétil inimigo comum | `#FF4D8D` |
| Acento inimigo | `#FF8C2E` |
| Projétil do boss | `#FF2626` |
| Acento do boss | `#FFB84D` |
| XP | `#FFD647` |
| Impacto | `#EBF7FF` |
| Aviso/spawn | `#FFB84D` |

Cores por classe:

| Classe | Cor |
|---|---|
| Drone | Magenta `#FF4D8D` |
| Corredor | Laranja `#FF8C2E` |
| Tanque | Roxo `#BF33FF` |
| Divisor | Verde `#33FFB3` |
| Atirador | Azul `#59BFFF` |
| Núcleo | Vermelho `#FF2626` |

### 4.2 Projéteis

- **Jogador:** corpo ciano, núcleo branco, crítico laranja.
- **Inimigo comum:** corpo magenta, sem núcleo branco.
- **Boss:** corpo vermelho, núcleo âmbar.
- **Orbital:** corpo ciano, núcleo branco.
- **Muzzle:** ciano.

Foi adicionado um filho `Core` aos templates de:

- Projétil do jogador.
- Projétil do boss.
- Orbe orbital.

O núcleo é visual. Não altera collider, dano, velocidade, lifetime ou comportamento.

### 4.3 XP e dano crítico

- XP continua dourado (`#FFD647`).
- Crítico agora é laranja (`#FF8C2E`).
- Flash de dano crítico e hit spark crítico também usam laranja.
- XP deixa de competir visualmente com o crítico.
- Dano normal mantém impacto branco.

### 4.4 Explosões e morte

`DeathBurst` agora recebe a identidade cromática da classe:

- Explosão principal: cor da classe.
- Partículas: variação clara da cor da classe.
- Branco reservado para hit spark, núcleo do jogador e núcleo do boss.

Escalas por categoria:

| Categoria | Inimigos | Escala |
|---|---|---:|
| Pequenos | Drone, Corredor | `0.5` |
| Médios | Tanque, Divisor, Atirador | `0.9` |
| Especial | Núcleo | `2.4` |

### 4.5 Sequência de morte

A nova sequência é:

1. Hit spark do projétil.
2. Death sheet autoral, com a duração real dos frames.
3. Sprite desativado.
4. Explosão + partículas coloridas.
5. Core drop do boss, quando aplicável.
6. Despawn após o FX terminar.

Garantias:

- Collider desativado no início da morte.
- Inimigo não causa mais dano após morrer.
- `Enemy.All` remove o inimigo no início da morte.
- XP e kill continuam sendo registrados no início.
- A animação de morte tem tempo para aparecer.
- A explosão não começa em cima da death sheet.
- O objeto é devolvido ao pool após os efeitos.
- O boss permanece ativo durante a animação/FX e só então dispara o toast.

### 4.6 Camera shake

| Evento | Shake |
|---|---|
| Drone | Nenhum |
| Corredor | Nenhum |
| Atirador/Tanque/Divisor | `0.04 / 0.10` |
| Núcleo | `0.35 / 0.55` |
| Dano no jogador | Mantido |

O hitstop do boss foi movido para o momento da explosão final, em vez de ocorrer depois do despawn.

### 4.7 Spawn warning

A cor do aviso passou a usar o âmbar semântico (`#FFB84D`) em vez do vermelho hardcoded. O comportamento de spawn não foi alterado.

---

## 5. Versão e apresentação

- `BuildScript.bundleVersion`: `0.9.10`.
- `ProjectSettings.asset`: `bundleVersion: 0.9.10`.
- Menu principal: `DEMO 0.9.10 · COMBAT VISUAL POLISH`.
- `README.md` atualizado para 0.9.10 com changelog.

A versão 0.9.9 permanece recuperável pela tag `v0.9.9-demo` e pelo histórico Git.

---

## 6. Validação executada

### Integridade

| Teste | Resultado |
|---|---|
| Estrutura PNG dos quatro arquivos | Filtros inválidos confirmados e corrigidos |
| Decode padrão dos quatro arquivos | Abrem normalmente |
| Preservação de pixels/dimensões/frames/alpha | Confirmada |
| `ValidateAllBatch` com PNG original inválido | Falhou corretamente com filtro `127` |
| `ApplyAllBatch` final | 73 sprites, 0 erros |
| `ValidateAllBatch` final | 73 verificados, 0 erros |

### Build

| Teste | Resultado |
|---|---|
| `BuildAndValidateBatch` | 0 problemas de cena |
| Build Windows | OK, 105,1 MB |
| Warnings de compilação | Nenhum warning CS |
| Referências ausentes | Nenhuma detectada na validação de cena |

### Smoke test

| Item | Resultado |
|---|---|
| Menu principal | OK; versão 0.9.10 visível |
| Seleção de piloto | OK |
| Gameplay | OK |
| Disparos do jogador | OK; projétil ciano com núcleo claro |
| Morte de inimigo pequeno | OK; explosão magenta observada |
| Coleta de XP | OK; level up observado |
| Level up | OK; 3 cards |
| Exceptions no `Player.log` | Nenhuma |

### Não executado

- Projétil do Atirador em gameplay ao vivo.
- Projétil do boss em gameplay ao vivo.
- Morte de inimigo médio ao vivo.
- Luta completa contra o Núcleo.
- Stress de 300 inimigos.
- Profiler.

Esses pontos foram revisados por código e assets, mas não foram observados em runtime nesta rodada.

---

## 7. Problemas resolvidos

- Os quatro PNGs inválidos foram corrigidos.
- O pipeline detecta PNG inválido, CRC quebrado, filtro inválido e dimensões divergentes.
- Projéteis deixaram de ser todos brancos.
- Explosões deixaram de ser brancas e agora carregam a cor da classe.
- A explosão não começa mais junto com a death sheet.
- Drone e Corredor não tremem mais a câmera.
- XP e crítico agora têm cores distintas.
- O núcleo branco foi preservado no jogador e no boss.
- O hitstop do boss ocorre no impacto da explosão.
- A versão do projeto, build e menu foi atualizada para 0.9.10.

---

## 8. Limitações e pendências

- Entrada do boss, telegraphs distintos e arena são Fase 3 e não foram alterados.
- Grid, UI pixel-perfect, animações de move/idle e morte do jogador são Fases 2+ e não foram alterados.
- `fx_explosion` ainda tem alpha parcial; a Fase 0 cobriu apenas os quatro PNGs indicados.
- O `SpawnWarning` continua fora da tela na configuração atual; a Fase 1 alterou apenas sua cor.
- Não houve stress de 300 inimigos nem Profiler.
- As páginas documentais históricas continuam em 0.9.9; o `README` foi atualizado para 0.9.10.
- As alterações não foram commitadas.
- A build não foi enviada ao itch.io.
- A senha do playtest não foi alterada.
- As Fases 2, 3, 4 e 5 não foram iniciadas.

---

## 9. Arquivos de log da rodada

- `Logs/validate-invalid-test.log`
- `Logs/art-import-0910-final.log`
- `Logs/validate-0910-final.log`
- `Logs/build-scene-0910-rc2.log`
- `Logs/build-0910-rc3.log`

---

## 10. Conclusão

A versão 0.9.10 conclui as Fases 0 e 1:

- Assets de projéteis/XP íntegros e validados.
- Pipeline capaz de bloquear PNGs inválidos.
- Projéteis com identidade de time.
- Explosões com cor de classe e escala proporcional.
- Death sheet visível antes da explosão.
- Camera shake proporcional ao peso do inimigo.
- XP dourado separado do crítico laranja.
- Build Windows funcional e sem exceptions no smoke test.

O resultado preserva a identidade visual existente e melhora a leitura do combate sem alterar dano, velocidade, colisão, balanceamento, spawn ou upgrades.
