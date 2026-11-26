# Correções de arte aplicadas — SWARM 0.9.9

**Escopo:** implementação das correções da auditoria de direção de arte.
**Base:** `7b10fff` + este passe de arte.
**Versão:** 0.9.9 (passe de arte, mesma demo).

---

## 1. O que foi corrigido

### Escala e leitura

- Escalas dos inimigos normalizadas para a mesma densidade visual da nave:
  - Drone `0.70 → 0.42`
  - Corredor `0.55 → 0.48`
  - Tanque `1.40 → 0.95`
  - Divisor `1.00 → 0.60`
  - Atirador `0.80 → 0.58`
  - Núcleo `2.20` (mantido)
- Jogador continua em `0.80`; a horda agora lê como horda, não como clones do jogador.
- Câmera com snap de pixel (`CameraFollow.pixelSnap = 40`) e fundo `#05070F`.

### Arte integrada

- Todas as naves usam `idle`, `hit`, `death` e `thruster` do `Art/Sprites/Ships`.
- Todos os inimigos usam `idle`, `move`, `hit` e `death` do `Art/Sprites/Enemies`.
- Núcleo usa `idle`, `telegraph`, `attack` e `death`.
- Dardo gira o visual para a direção do movimento.
- Olho troca para `enemy_olho_charge` antes de atirar.
- Boss usa telegraph dedicado e sheet de ataque; a morte toca 8 frames antes de despawnar.

### Projéteis, orbes e FX

- Projétil do jogador: `proj_player`.
- Projétil do Atirador: `proj_enemy` (separado do boss).
- Projétil do Núcleo: `proj_boss`.
- Orbital: `orbital`.
- XP: `xp_orb`, com tint dourado e escala menor.
- Muzzle: `fx_muzzle`.
- Impacto: `fx_hit`.
- Morte: `fx_explosion` + partículas.
- Spawn: `fx_spawn`.
- Propulsor: `fx_thruster`/`ship_*_thruster`, agora sprite persistente em vez de partículas brancas.
- Boss dropa um efeito visual de `core_pickup` ao morrer.

### Interface

- Level up virou **3 cards** com ícone, título, descrição, categoria e tecla.
- Todos os 20 `ui_upgrade_*` estão ligados aos `UpgradeData`.
- Seleção de piloto virou **3 cards** com `ui_pilot_*`.
- `ui_boss` entrou na barra do boss.
- `ui_core` entrou no cabeçalho da seleção de piloto.
- Menu principal usa o wordmark `ui_logo_swarm` derivado da capa.
- Fonte pixel **Press Start 2P** para títulos, HUD e botões; texto longo em fonte de corpo.
- Painéis de menu/level up opacos para isolar a leitura do gameplay.

### Paleta e atmosfera

- `UIPalette` unificada:
  - perigo/HP/boss: `#FF3D4D`
  - aviso: `#FFB84D`
  - XP: `#FFD647`
- Categorias de upgrade: ofensivo ciano, defesa dourado, utilidade âmbar.
- Bloom reduzido: `intensity 0.48`, `threshold 0.82`, `scatter 0.52`.
- Fundo em duas camadas de grid com parallax e tint vermelho durante o duelo do Núcleo.
- Alphas do grid reduzidos para não competir com projéteis.

---

## 2. Arquivos principais

| Área | Arquivos |
|---|---|
| Animação | `Scripts/Visual/SpriteAnimator.cs`, `Scripts/Visual/FxOneShot.cs` |
| Inimigos | `Scripts/Combat/Enemy.cs`, `EnemyData.cs`, `BossController.cs`, `RangedEnemy.cs` |
| Projéteis/FX | `Projectile.cs`, `EnemyProjectile.cs`, `DeathBurst.cs`, `MuzzleFlash.cs`, `SpawnWarning.cs` |
| Jogador | `PlayerVisual.cs`, `ThrusterTrail.cs`, `CharacterData.cs`, `CharacterSelect.cs`, `OrbitalWeapon.cs` |
| UI | `HudController.cs`, `UpgradeCardUI.cs`, `CharacterCardUI.cs`, `UIPalette.cs`, `MainMenu.cs` |
| Cena | `Editor/SwarmSceneBuilder.cs`, `Scenes/Swarm.unity` |
| Dados | `Data/Enemies/*`, `Data/Characters/*`, `Data/Upgrades/*` |
| Arte | `Art/Sprites/manifest.json`, `Art/Sprites/UI/ui_logo_swarm.png`, `Art/Fonts/PressStart2P-Regular.ttf` |

---

## 3. Verificação

- `ArtImportPipeline.ApplyAllBatch`: **73 sprites** importados.
- `SwarmSceneBuilder.BuildAndValidateBatch`: **0 problemas**.
- Build Windows: **OK**, ~105,2 MB.
- Smoke test em janela: menu, gameplay e level up renderizados; sem exceptions no `Player.log`.
- Capturas usadas na verificação: `Logs/shot-menu3.png`, `shot-game10.png`, `shot-levelup.png` (arquivos locais de QA).

### Flags de QA

A build aceita flags para testes visuais e de trailer:

```text
SWARM.exe -autostart              # pula menu e escolhe o primeiro piloto
SWARM.exe -showselect             # pula o menu e para na seleção de piloto
SWARM.exe -autostart -showlevelup # abre o preview dos 3 cards
```

Não alteram o fluxo normal sem as flags.

---

## 4. O que ainda falta

1. `enemy_fragmento_idle` ainda não tem um template próprio de fragmento; o Divisor usa o Drone como fragmento.
2. `core_pickup` é só efeito visual no boss; não existe sistema de coleta de núcleos no mapa.
3. As sheets `ship_*_death` ainda não tocam na morte do jogador (o game over encerra antes).
4. Não houve stress de 300 inimigos nem Profiler neste passe.
5. A build 0.9.9 ainda precisa ser enviada ao itch.io; a página continua na 0.9.8.
6. A senha antiga do playtest precisa ser rotacionada fora do Git.

---

## 5. Critério de aceite deste passe

- Jogador identificável em 1 s no meio da horda.
- Nenhum projétil/FX procedural antigo no gameplay.
- Level up sem parede de texto.
- Menu principal e seleção de piloto com arte da mesma família dos sprites.
- Nenhum sprite de gameplay com densidade de pixel quase 2× maior que outro.
