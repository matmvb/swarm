# 01 — Inventário do projeto

**Data:** 24/11/2026  
**Branch:** `main` · `e0c9fe7` · `origin/main` sincronizado  
**Unity:** `6000.6.1f1` (revisão `7efac9f6c10e`)  
**Plataforma:** Windows 10/11 x64

---

## 1. Repositório

| Item | Valor |
|---|---|
| Remoto | `https://github.com/matmvb/swarm` |
| Branch atual | `main` |
| Último commit | `e0c9fe7 feat: folha de conceito e brief de sprites para Piskel` |
| Commits à frente/atrás | `0 / 0` |
| Tags recentes | `v0.9.8-demo`, `v0.9.7-icon`, `v0.9.4-demo`, `v0.9.0-demo` |
| CI | **nenhum** workflow em `.github/` |
| Licença | **não há** `LICENSE` |

### Working tree

```text
 M Assets/Swarm/Scenes/Swarm.unity
?? Assets/Swarm/Art/Sprites.meta
?? Assets/Swarm/Art/Sprites/
?? docs/art/sprites-preview.html
?? docs/art/sprites-preview.png
```

- A cena tem alterações manuais de `m_MaxSize` em 5 componentes `Text`.
- 72 PNGs + 72 `.meta` + `manifest.json` novos, não commitados.
- O preview de sprites é um artefato de apoio, também não commitado.

---

## 2. Estrutura

```
Assets/
  Scenes/SampleScene.unity              (fora do build)
  Settings/                             URP, Renderer2D, volume, InputActions
  Swarm/
    Art/
      AppIcon.png
      Shapes/                           sprites procedurais (hex, diamond, etc.)
      Sprites/                          NOVO: 72 sprites em 5 categorias
      SpriteUnlit.mat
      white.png
    Data/
      Characters/                       3 ScriptableObjects
      Enemies/                          6 ScriptableObjects
      Upgrades/                         20 ScriptableObjects
    Editor/
      BuildScript.cs
      SwarmArt.cs
      SwarmSceneBuilder.cs
    Scenes/Swarm.unity                  cena de jogo
    Scripts/                            44 scripts de runtime
    Settings/NeonProfile.asset          Bloom + Vignette
Builds/
  Windows/                              build 0.9.8 (ignorada pelo Git)
  SWARM-windows.zip                     artefato de distribuição
docs/                                   28 Markdown + Pages + arte
```

---

## 3. Contagens

| Recurso | Quantidade |
|---|---:|
| Scripts C# | **47** (44 runtime + 3 editor) |
| Cenas Unity | **3** (`Swarm`, `SampleScene`, template URP) |
| ScriptableObjects | **34** (6 inimigos, 3 pilotos, 20 upgrades, 4 URP + 1 perfil) |
| Prefabs | **0** (templates são GameObjects da cena) |
| Materiais | **1** (`SpriteUnlit.mat`) |
| Shaders customizados | **0** |
| Assembly Definitions | **0** |
| Documentos Markdown | **28 existentes + 9 desta auditoria = 37** |
| Sprites novos | **72 PNGs** |
| Builds Windows | 1 (`Builds/Windows`) |
| Arquivos no ZIP | 202 |

---

## 4. Versão e pacotes

`ProjectSettings/ProjectVersion.txt`:

```text
m_EditorVersion: 6000.6.1f1
m_EditorVersionWithRevision: 6000.6.1f1 (7efac9f6c10e)
```

Instalado em:

```
C:\Program Files\Unity\Hub\Editor\6000.6.1f1
```

Pacotes principais (`Packages/manifest.json`):

| Pacote | Versão |
|---|---|
| URP | 17.6.0 |
| Input System | 1.19.0 |
| uGUI | 2.6.0 |
| 2D Animation | 16.0.0 |
| 2D SpriteShape | 16.0.0 |
| 2D Aseprite | 6.0.0 |
| Test Framework | 1.8.0 |
| Timeline | 6.6.0 |
| Visual Scripting | 1.9.11 |

**Divergência documental:** GDD e plano citam `6000.6.7f1`; o setup e o projeto usam
`6000.6.1f1`. O `03-SETUP.md` explica a diferença, mas GDD/plano não foram atualizados.

---

## 5. Configurações relevantes

| Configuração | Valor |
|---|---|
| Color Space | Linear |
| Active Input Handling | Input System novo (`activeInputHandler: 1`) |
| Quality atual | 5 (Ultra) |
| VSync | 1 no Ultra; 0 em Low/Medium |
| Fullscreen | `FullScreenWindow` 1920×1080 |
| Splash | Ligado |
| Scripting Backend | Mono2x (definido no BuildScript) |
| Cena no Build Settings | Apenas `Assets/Swarm/Scenes/Swarm.unity` |
| URP shader stripping | Post-processing/2D desligado (correto) |
| NeonProfile | Bloom + Vignette como sub-assets (correto) |

---

## 6. Scripts por domínio

### Core

`GameManager`, `ObjectPool`, `SaveSystem`, `RunLogger`, `Sfx`, `HitStop`,
`InputReader`, `ScreenshotCapture`.

### Player

`PlayerController`, `PlayerStats`, `Health`, `Weapon`, `PlayerVisual`,
`OrbitalWeapon`, `ThrusterTrail`, `PlayerRegen`, `CharacterData`, `CharacterSelect`.

### Combat

`Enemy`, `EnemyData`, `EnemySpawner`, `BossController`, `Projectile`,
`EnemyProjectile`, `RangedEnemy`, `SplitOnDeath`, `SpawnWarning`, `BurstParticle`,
`DeathBurst`, `MuzzleFlash`.

### Progression

`XpSystem`, `UpgradeSystem`, `UpgradeData`, `MetaProgression`.

### UI

`HudController`, `MainMenu`, `PauseMenu`, `OptionsMenu`, `GameOptions`,
`DebugOverlay`, `UIPalette`.

### Camera

`CameraFollow`, `BackgroundGrid`.

### Editor

`BuildScript`, `SwarmArt`, `SwarmSceneBuilder`.

---

## 7. ScriptableObjects

### Inimigos

| Asset | Nome | HP | Vel | Dano | XP | Peso | Escala |
|---|---|---:|---:|---:|---:|---:|---:|
| `Drone.asset` | Drone | 20 | 2.0 | 6 | 1 | 60 | 0.7 |
| `Corredor.asset` | Corredor | 12 | 3.6 | 4 | 1 | 30 | 0.55 |
| `Tanque.asset` | Tanque | 90 | 1.3 | 14 | 3 | 20 | 1.4 |
| `Divisor.asset` | Divisor | 45 | 1.6 | 8 | 2 | 15 | 1.0 |
| `Atirador.asset` | Atirador | 35 | 1.4 | 6 | 3 | 12 | 0.8 |
| `Nucleo.asset` | Núcleo | 1200 | 1.0 | 15 | 40 | 4 | 2.2 |

### Pilotos

`Tanque`, `Veloz`, `Atirador` — apenas multiplicadores e cor; **não têm campo de sprite**.

### Upgrades

20 assets: `MunicaoPesada`, `GatilhoRapido`, `TiroDuplo`, `Perfuracao`, `PernaLeve`,
`Blindagem`, `Ima`, `MiraFria`, `Alcance`, `ProjetilVeloz`, `Reparo`,
`NucleoInstavel`, `OrbeOrbital`, `OrbeDenso`, `Placas`, `Nanorrobos`, `ImaForte`,
`CalibrePesado`, `Execucao`, `CascaExtra`.

---

## 8. Arte procedural existente

`Assets/Swarm/Art/Shapes/`:

| Arquivo | Uso |
|---|---|
| `ship.png` | nave do jogador |
| `hex.png` | Drone |
| `diamond.png` | Corredor / projétil / orbe |
| `square.png` | Tanque |
| `ring.png` | Divisor |
| `circle.png` | Atirador / orbe orbital |
| `octagon.png` | Núcleo |
| `grid.png` | fundo |
| `white.png` | barras/UI |
| `SpriteUnlit.mat` | material único |

Os assets de `EnemyData` apontam para esses sprites procedurais, não para `Art/Sprites`.

---

## 9. Arte nova (`Assets/Swarm/Art/Sprites`) — WIP

**72 PNGs:**

| Pasta | Arquivos |
|---|---:|
| `Ships/` | 12 |
| `Enemies/` | 24 |
| `Projectiles/` | 6 |
| `FX/` | 5 |
| `UI/` | 25 |
| **Total** | **72** |

Também há `manifest.json` com 72 entradas, frame size, frames, sheet size, FPS,
pivô, PPU, filtro, compressão, mipmaps e mesh type.

### Situação da integração

- **0** GUIDs de sprites novos aparecem em `Assets/Swarm/Scenes/Swarm.unity`.
- **0** referências no código runtime/editor.
- **0** nomes de sprites novos no build (`Builds/Windows/SWARM_Data`).
- `CharacterData` não tem campo de sprite.
- `EnemyData.sprite` continua apontando para `Art/Shapes/*.png`.
- `SwarmSceneBuilder.AssignEnemySprites` só atribui se `sprite == null`; não substitui.
- `manifest.json` não é lido por nenhum script.

### Importação real dos 72 `.meta`

Todos os 72 estão com:

```text
spriteMode: 2            (Multiple)
spritePixelsToUnits: 100 (errado para o manifesto)
filterMode: 1            (Bilinear)
spriteMeshType: 1        (Tight)
spriteExtrude: 1
```

O manifesto pede:

```text
PPU = frame (8, 12, 16, 20, 24, 32, 64 ou 96)
Filter = Point
Compression = None
Mesh = FullRect
Extrude = 0
Pivot = Center
```

**Conclusão:** a arte existe e os tamanhos de folha batem com o manifesto, mas a
importação e a integração não foram feitas.

---

## 10. Cenas e build

### Cenas

| Cena | Situação |
|---|---|
| `Assets/Swarm/Scenes/Swarm.unity` | cena do jogo; no Build Settings |
| `Assets/Scenes/SampleScene.unity` | template não usado |
| `Assets/Settings/Scenes/URP2DSceneTemplate.unity` | template URP não usado |

### Build existente

| Item | Valor |
|---|---|
| Versão no build | `0.9.8` |
| `SWARM.exe` | 667.136 bytes |
| Dados | ~50 MB em `SWARM_Data` |
| ZIP | 40.595.947 bytes (~38,7 MiB) |
| Conteúdo do ZIP | exe, data, Mono, D3D12, UnityPlayer, crash handler |
| PDBs | 0 |
| Backup folder | 0 |
| `LEIA-ME.txt` | presente |
| Timestamp | 22/11/2026 17:58 |
| Cena atual | 24/11/2026 11:24 |
| Arte nova | 23/11/2026 20:15 |

**A build é anterior ao WIP de cena/arte.** O build não contém os sprites novos.

---

## 11. Git e histórico

- `main` sincronizada com `origin/main`.
- Nenhum merge pendente.
- Tags existentes: `v0.3.0-meta`, `v0.4.0-demo`, `v0.5.0-art`, `v0.6.0-juice`,
  `v0.7.0-options`, `v0.9.0-demo`, `v0.9.4-demo`, `v0.9.7-icon`, `v0.9.8-demo`.
- `v0.9.8-demo` aponta para `63b976e` (correção de Bloom/vinheta + release 0.9.8).
- A cena e a arte atuais **não estão em nenhum commit**.

---

## 12. Documentação

28 Markdown em `docs/` antes desta auditoria (mais os 9 desta pasta), incluindo:

- `01-GDD.md` — design, fórmulas, inimigos, upgrades, pilots, meta.
- `02-PLANO-DEV.md` — plano de 10 semanas; status histórico, não atual.
- `03-SETUP.md` — setup e versão 6000.6.1f1.
- `04-ARQUITETURA.md` — arquitetura.
- `05`–`09` — tutoriais.
- `10-PERFORMANCE-QA.md` — metas e checklist.
- `11-PUBLICACAO.md` — itch/Steam/marketing.
- `12`/`13` — playtest e resultados históricos.
- `17`–`26` — itch, UI, playtest, feedback, auditoria de página, arte.

Divergências principais:

- GDD/plano: Unity `6000.6.7f1`; projeto: `6000.6.1f1`.
- GDD: “a cada 20 segundos”; implementação: level up por XP.
- GDD: boss HP 1200; no spawn o HP é multiplicado pela dificuldade (2,75× às 5:00).
- Plano: checkboxes e “quadrados” históricos; não refletem 0.9.8.
- `docs/13`: resultados de build antiga/1.0; histórico, não atual.

---

## 13. Publicação

| Item | Status |
|---|---|
| GitHub Pages raiz | 200 |
| Feedback | 200 |
| Privacidade | 200 |
| Concept sheet | 200 |
| itch.io sem senha | 200 (tela de senha) |
| itch.io com senha | 200 (página real) |
| ZIP no itch | `SWARM-windows.zip`, 38 MB |
| Formspree | endpoint `xqpqgyzn`, CORS OK |
| Screenshots de gameplay | ausentes (só capa) |
| GIFs/trailer | ausentes |
| Teste externo | pendente |

---

## 14. Itens obsoletos ou não usados

- `Assets/Scenes/SampleScene.unity` — template.
- `Assets/Settings/Scenes/URP2DSceneTemplate.unity` — template.
- `Assets/Settings/InputSystem_Actions.inputactions` — não referenciado pelo `InputReader`.
- `SaveSystem.BestKills`, `BestTime` e `ResetAll` — gravados/definidos, mas não usados.
- `docs/art/sprites-preview.html/png` — preview não commitado.
- `Assets/Swarm/Art/Sprites/manifest.json` — documentação de arte, não consumida por código.

---

## 15. Resumo do inventário

O projeto tem um núcleo de código completo e uma cena válida, mas o estado atual está
**partido entre três camadas**:

1. código commitado (limpo, compila);
2. WIP não commitado (cena + 72 sprites);
3. build publicada antiga (0.9.8 de 22/11).

A prioridade do inventário é **unificar essas três camadas antes de qualquer nova feature**.
