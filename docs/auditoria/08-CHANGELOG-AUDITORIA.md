# 08 — Changelog da auditoria

**Data:** 24/11/2026  
**Auditoria:** técnica, gameplay, QA, performance, Git e publicação  
**Modo:** somente leitura; nenhuma alteração em código, cena, assets, build, Git ou serviços externos.

---

## 1. Ações executadas

### Leitura e inventário

- `git branch -vv`, `git status`, `git log`, `git tag`, `git remote`.
- `ProjectVersion.txt`, `Packages/manifest.json`, `packages-lock.json`.
- Estrutura de `Assets/`, `Builds/`, `docs/`.
- Listagem de scripts, cenas, assets, materiais, prefabs e shaders.
- Leitura de 47 scripts C#.
- Leitura de `SwarmSceneBuilder.cs`, `SwarmArt.cs`, `BuildScript.cs`.
- Leitura de `01-GDD.md`, `02-PLANO-DEV.md`, `03-SETUP.md`, `10-PERFORMANCE-QA.md`,
  `13-RESULTADOS-SPRINT-01.md`, `17`, `24`, `25`, `26`.
- Leitura de `Builds/Windows` e do ZIP.

### Verificações automáticas

- Python para contagem de scripts/assets, dimensões de PNG, cores/alpha, IDs/duplicatas,
  referências de GUID e schema do manifesto.
- `grep` para hot paths, eventos, `GetComponent`, coroutines, `PlayerPrefs`, `new`.
- Leitura de logs (`Editor.log`, `AssetImportWorker*`, `Player.log`, CSVs).
- Leitura de `TagManager`, `Physics2DSettings`, `QualitySettings`, `TimeManager`,
  `GraphicsSettings`, `ProjectSettings`.
- HTTP read-only em Pages, itch e Formspree.

### Teste em batch

Comando executado:

```powershell
Unity.exe -batchmode -quit -nographics `
  -projectPath "C:\...\swarm" `
  -executeMethod SwarmSceneBuilder.ValidateSceneBatch `
  -logFile "Logs\audit-validate.log"
```

Resultado:

```text
[SWARM] Validação concluída: 0 problema(s).
EXIT=0
```

- Sem erros/warnings de compilação.
- Sem exceções.
- SHA-1 de source antes/depois: **0 diferenças**.

### Não executado

- `BuildAndValidateBatch` (regenera cena/assets; apagaria WIP).
- `BuildScript.BuildWindowsBatch` (sobrescreveria build/PlayerSettings).
- Play Mode/run completa.
- Testes automatizados (não existem).
- Stress/Profiler.
- Alterações externas (itch, Pages, Formspree).
- Commits/push.

---

## 2. Descobertas registradas

### Altas

1. **Arte nova não integrada:** 72 PNGs, 0 referências na cena/código/build.
2. **Import settings errados:** PPU 100, Bilinear, Multiple, Tight, Extrude 1.
3. **WIP não commitado:** cena + arte + preview fora do Git.
4. **Senha do itch pública:** senha real em docs públicos.
5. **Build stale:** 0.9.8 de 22/11; cena/arte de 23–24/11.

### Médias

- Ajustes manuais de UI (`m_MaxSize`) não reproduzidos pelo gerador.
- Boss identificado por escala; HP real 3300 vs GDD 1200.
- `Restart` por `buildIndex`.
- Sem layers/matriz de colisão.
- Sem Sprite Atlas.
- `PlayerVisual`/orbe O(N) por frame.
- Sem prewarm/caps.
- Sprites com cores/alpha fora da paleta.
- Game over sem menu.
- Sem testes/CI/Profiler.
- `CharacterData` sem sprite; tint/flash incompatíveis com arte colorida.
- `EnsureAsset`/`AssignEnemySprites` não atualizam assets existentes.
- Documentação com versões/20s/boss divergentes.
- Build manual/versão duplicada.
- Publicação sem screenshots/GIFs/trailer.
- Save sem teste runtime.
- UI sem fonte única de layout.
- `SwarmSceneBuilder` monolítico/destrutivo.
- Sem licença.

### Baixas

- Options sem ESC.
- DebugOverlay em build.
- SaveSystem com campos mortos.
- Sfx pitch compartilhado.
- Screenshot por segundo.
- Comentário “curva linear”.
- `IsOffensive` sem orbitais.
- Cenas/assets não usados.
- Boss sem animação de morte.
- Projétil direcional sem offset.
- Gamepad só move.
- Um dano por contato.
- Ultra/MSAA para pixel art.
- 7 GUIDs de Renderer2D.
- Preview não commitado.

---

## 3. Decisões da auditoria

| Decisão | Motivo |
|---|---|
| Não rodar `BuildAndValidateBatch` | regeneraria a cena e apagaria ajustes manuais |
| Não rodar build Windows | evitar sobrescrever artefato/PlayerSettings |
| Não rodar Play Mode | sem build atual e para não abrir tela cheia |
| Não criar testes | exigiria alterar source; fora do escopo |
| Não commitar | regra explícita do pedido |
| Não alterar Pages/itch | regra explícita |
| Criar apenas `docs/auditoria/` | entregável autorizado |

---

## 4. Evidências principais

| Evidência | Onde |
|---|---|
| 0 problemas de cena | `Logs/audit-validate.log` |
| Build 0.9.8 | `Builds/Windows/SWARM_Data/globalgamemanagers` |
| ZIP limpo | `Builds/SWARM-windows.zip` |
| Sprites não referenciados | `Swarm.unity` + `EnemyData` + `manifest.json` |
| Import settings | `Assets/Swarm/Art/Sprites/**/*.meta` |
| WIP | `git status` |
| Senha pública | `docs/20`, `docs/21`, `docs/24` |
| Boss HP | `Enemy.cs:64-66`, `BossController` |
| Restart | `GameManager.cs:107-112` |
| Física | `TagManager.asset`, `Physics2DSettings.asset` |
| Sem testes/CI | `find Assets`, `.github` |
| Sem profiler | `ProfilerCaptures/` |
| Player.log 0.9.8 | `%LocalLow%/Scoty/SWARM/Player.log` |
| CSV | `%LocalLow%/Scoty/SWARM/swarm-runs.csv` |
| Pages/itch | HTTP 200 |

---

## 5. Estado após a auditoria

- Nenhum arquivo de código, cena, asset, build ou configuração foi alterado.
- Apenas `docs/auditoria/` foi criado.
- O WIP de cena/arte continua **não commitado**.
- A build 0.9.8 continua **stale**.
- A senha do itch continua **pública**.
- O playtest continua no ar.

---

## 6. Próximo gate

Aguardando aprovação do autor para:

1. preservar o WIP em branch/commit;
2. corrigir import settings e integrar a arte;
3. rotacionar a senha e limpar docs;
4. gerar build atual e rodar smoke test;
5. iniciar P1/P2 do roadmap.

Nenhuma correção deve começar antes do item 1.
