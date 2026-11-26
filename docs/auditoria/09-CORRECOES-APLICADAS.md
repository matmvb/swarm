# 09 — Correções aplicadas pós-auditoria

**Data:** 26/11/2026  
**Baseline preservado:** `1fdf894` (WIP de arte/cena + relatórios 00–08)  
**Correções:** `e04d743`  
**Versão:** 0.9.9

> Os documentos 00–08 são o retrato **antes** das correções. Este documento registra
> o que foi feito depois da aprovação.

---

## 1. Preservação do WIP

Antes de alterar qualquer coisa, foi criado o commit `1fdf894` com:

- `Assets/Swarm/Scenes/Swarm.unity` (ajustes de UI);
- `Assets/Swarm/Art/Sprites` completo (72 PNGs + metas + manifest);
- `docs/art/sprites-preview.html/png`;
- `docs/auditoria/00`–`08`.

Nada do trabalho anterior foi perdido.

---

## 2. Importação da arte

Novo editor tool:

```text
Assets/Swarm/Editor/ArtImportPipeline.cs
```

Menu:

```text
SWARM > Arte > Aplicar import settings
```

Batch:

```text
Unity.exe -executeMethod ArtImportPipeline.ApplyAllBatch
```

O que ele faz:

- lê `Assets/Swarm/Art/Sprites/manifest.json`;
- aplica **PPU = frame** (8, 12, 16, 20, 24, 32, 64, 96);
- `Filter Mode = Point`;
- `Compression = Uncompressed`;
- `Mipmaps = off`;
- `Full Rect`;
- `Extrude = 0`;
- pivô central;
- fatia sprite sheets por célula (`frameWidth × frameHeight`), sem trim;
- reimporta os 72 sprites.

**Resultado:** 72/72 reimportados e vinculados.

---

## 3. Integração no jogo

### Inimigos

Os 6 `EnemyData` agora apontam para o primeiro frame do idle novo:

| Inimigo | Sprite |
|---|---|
| Drone | `enemy_esporo_idle` |
| Corredor | `enemy_dardo_idle` |
| Tanque | `enemy_casco_idle` |
| Divisor | `enemy_celula_idle` |
| Atirador | `enemy_olho_idle` |
| Núcleo | `enemy_nucleo_idle` |

`EnemyData.color` foi para **branco**, para o sprite manter a paleta própria.

### Naves

`CharacterData` ganhou:

```csharp
public Sprite shipSprite;
```

E os 3 pilotos receberam:

| Piloto | Sprite |
|---|---|
| TANQUE | `ship_baluarte_idle` |
| VELOZ | `ship_vetor_idle` |
| ATIRADOR | `ship_lanca_idle` |

`CharacterSelect.Apply` agora troca o sprite e usa `Color.white`; o fallback
procedural + cor do piloto continua existindo se o campo estiver vazio.

### Flash de dano

`Enemy` ganhou um `SpriteRenderer` filho `Flash`:

- overlay branco no hit normal;
- overlay amarelo no crítico;
- fade por alpha;
- desligado no pool/death.

Isso resolve o flash invisível em sprite colorido sem depender de shader novo.

---

## 4. Correções de fluxo

| Correção | Arquivo |
|---|---|
| Restart por nome da cena (não `buildIndex`) | `GameManager.cs` |
| Boss detectado por `BossController`, não por escala | `Enemy.cs` |
| `[M]` volta ao menu no game over | `HudController.cs` |
| `ESC` fecha opções sem vazar para o menu de baixo | `OptionsMenu`, `PauseMenu`, `MainMenu` |
| APIs obsoletas de ícone/build removidas | `BuildScript.cs` |
| APIs obsoletas do validador removidas | `SwarmSceneBuilder.cs` |
| Aviso obsoleto do `spritesheet` isolado | `ArtImportPipeline.cs` |

---

## 5. Segurança do playtest

A senha real foi removida de:

- `docs/20-PLAYTEST-EXTERNO.md`;
- `docs/21-ITCH-FORM-PLAYTEST.md`;
- `docs/24-VERIFICACAO-PAGES.md`.

Os documentos agora usam `SUA_SENHA`/`<senha-do-convite>` e dizem que a senha real
não é versionada.

**Ação manual pendente:** rotacionar a senha no itch.io. O valor antigo ainda pode
estar no histórico do Git; remover do arquivo não invalida a senha.

---

## 6. Versão e build

| Item | Valor |
|---|---|
| Versão | **0.9.9** |
| `BuildScript` | 0.9.9 |
| `ProjectSettings` | 0.9.9 |
| Menu principal | `DEMO 0.9.9` |
| Build | `Builds/Windows/SWARM.exe` |
| Tamanho | 103,9 MB |
| ZIP | `Builds/SWARM-windows.zip` · 39 MB |
| ZIP | 202 entradas · sem PDB · sem backup · README incluso |

O build foi gerado **limpo** (pasta `Builds/Windows` apagada antes), então os arquivos
são todos da 0.9.9.

---

## 7. Verificação

| Teste | Resultado |
|---|---|
| Compilação | OK |
| `ValidateSceneBatch` | **0 problemas** |
| Warnings de compilação | **0** |
| Sprites no build | nomes encontrados em `sharedassets0.assets` |
| Startup do build | abriu sem exceptions de script |
| Player.log | apenas aviso de cache D3D12; sem erro de jogo |
| ZIP | raiz correta, sem PDB/backup |

**Limitação:** não foi feita uma run completa automatizada. A validação de arte em
runtime foi por build + asset refs + startup; falta o smoke test visual de 1 run.

---

## 8. O que ainda falta

1. **Rotacionar a senha do itch** (ação externa).
2. **Subir a build 0.9.9 no itch** (ação externa).
3. Rodar uma **run completa** e confirmar nave/inimigos/boss na tela.
4. **Performance:** layers de colisão, atlas, prewarm, Profiler/stress.
5. **Testes automatizados/CI**.
6. **Screenshots/GIFs/trailer**.
7. Limpar cores/alpha dos sprites de projétil/FX/death que ficaram fora da paleta.
8. Decidir oficialmente o HP do boss (1200 base × 2,75 = 3300 no spawn).
9. Adicionar licença ao repositório.

---

## 9. Resultado

O projeto saiu de:

> “arte existe fora do jogo, import errado, build antiga, senha pública”

para:

> “arte integrada em naves/inimigos/boss, import correto, build 0.9.9 gerada,
> senha removida dos docs, fluxo de restart/game over/opções corrigido”.

A base agora está pronta para o próximo passo: **smoke test visual + build no itch**.
