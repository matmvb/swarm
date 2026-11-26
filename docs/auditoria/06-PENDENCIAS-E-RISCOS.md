# 06 — Pendências e riscos priorizados

**Nenhum bug CRÍTICO de execução foi reproduzido.**  
Há **5 riscos ALTOS** de processo/entrega e uma dívida técnica relevante.

---

## 1. Bloqueadores de entrega

| ID | Gravidade | Área | Evidência | Consequência | Correção | Dependências |
|---|---|---|---|---|---|---|
| H-01 | ALTO | Arte/integração | 72 sprites em `Assets/Swarm/Art/Sprites`, **0 GUIDs na cena**, 0 refs no código | O jogo continua com formas procedurais; a arte autoral não existe no build | Atribuir sprites em `EnemyData`, `CharacterData`, player, projéteis, FX, UI | H-02, H-03 |
| H-02 | ALTO | Pipeline de arte | 72 `.meta` com `PPU=100`, `filterMode=1` (Bilinear), `spriteMode=2`, `meshType=1`, `extrude=1`; manifesto pede PPU por frame, Point, FullRect, 0 | Sprites borrados, minúsculos e com mesh errado | Criar editor script/import postprocessor para aplicar o manifesto; reimportar | H-03 |
| H-03 | ALTO | Git/WIP | `git status`: cena + 72 PNGs/metas + preview não commitados | Trabalho pode ser perdido ao trocar branch/limpar; build não reproduz | Criar branch/commit de preservação | Nenhuma |
| H-04 | ALTO | Segurança/playtest | `docs/20`, `docs/21`, `docs/24` contêm a senha real; repo/Pages públicos | Qualquer pessoa acessa o itch restrito; password é público | Rotacionar senha no itch; remover senha real dos docs; usar placeholder | Nenhuma |
| H-05 | ALTO | Build/release | Build 0.9.8 de 22/11; cena de 24/11; arte de 23/11; build não contém nomes dos sprites | Testers recebem build antiga; source/build divergem | Gerar build do WIP após H-01/H-02 e validar | H-01, H-02, H-03 |

---

## 2. Riscos médios

| ID | Gravidade | Área | Evidência | Consequência | Correção | Dependências |
|---|---|---|---|---|---|---|
| M-01 | MÉDIO | Cena/gerador | `Swarm.unity` tem `m_MaxSize` manual 46/60/72; `MakeText` não define maxSize; `BuildAndValidateBatch` regenera | Ajustes somem ao rodar o gerador; layout imprevisível | Mover ajustes para `MakeText`/dados; ou parar de regenerar | Nenhuma |
| M-02 | MÉDIO | Boss/design | `Enemy.cs:154` usa `data.scale > 1.4f`; `Enemy.cs:64-66` aplica dificuldade ao boss; 1200 × 2,75 = 3300 | Boss pode ficar 2,75× mais forte que o GDD; escala nova pode quebrar o tipo | Usar `BossController`/flag explícita; documentar HP base vs spawn | Decisão de design |
| M-03 | MÉDIO | Restart | `GameManager.cs:107-112` usa `buildIndex` | Cena fora do Build Settings → `LoadScene(-1)` | `LoadScene(SceneManager.GetActiveScene().name)` ou validar buildIndex | Nenhuma |
| M-04 | MÉDIO | Física/perf | `TagManager` sem layers; matriz toda `ff`; projéteis triggers | Pares inúteis testados; custo cresce com horda | Layers + matriz + `CompareTag`/LayerMask | Profiler baseline |
| M-05 | MÉDIO | Render/perf | Nenhum Sprite Atlas; 72 texturas novas + 9 procedurais | Draw calls altos; risco de GPU-bound | Atlas de gameplay; padding 2; compressão None | H-01/H-02 |
| M-06 | MÉDIO | CPU | `PlayerVisual.LateUpdate` O(N); `OrbitalWeapon` O(orbs×N) | Custo cresce com 300 inimigos; possível frame spike | Target provider compartilhado; tick de orbes 0,1 s | Profiler baseline |
| M-07 | MÉDIO | Pool/GC | `ObjectPool` sem prewarm; sem cap de projéteis/orbes | Hitch no primeiro combate; picos de memória | Prewarm; caps; reciclagem | Teste de stress |
| M-08 | MÉDIO | Arte | `proj_player` 16 cores/5 alphas; `fx_explosion` 26/7; `casco_death` 18 cores; `baluarte_death` 38 | Visual inconsistente, alpha indesejado, tint imprevisível | Reexportar/limpar paleta e alpha; validar com script | H-02 |
| M-09 | MÉDIO | UX | `HudController.cs:250-273` só mostra `[R]` | Jogador não volta ao menu após run | Adicionar `[M]` → `GameManager.Restart(false)` | Nenhuma |
| M-10 | MÉDIO | QA | Sem testes, sem asmdef, sem CI; `test-framework` instalado | Regressões como colisor/pool podem voltar | Testes EditMode/PlayMode + CI | Nenhuma |
| M-11 | MÉDIO | Performance | `ProfilerCaptures` vazio; nenhum dado 300/100/100 | Meta de 60 fps não comprovada | Stress test e captura | Harness |
| M-12 | MÉDIO | Arte/pilotos | `CharacterData` sem sprite; `CharacterSelect.Apply` só faz tint; `Enemy` flash por `Lerp` | 3 naves novas não usadas; tint global; flash branco invisível em sprite colorido | Campo `shipSprite`; overlay de flash; tint branco | H-01/H-02 |
| M-13 | MÉDIO | Gerador/assets | `EnsureAsset` só configura asset novo; `AssignEnemySprites` só se `sprite==null` | Campos/artes novos não chegam aos assets existentes | Migração/atualização explícita | H-01 |
| M-14 | MÉDIO | Documentação | GDD/plano 6000.6.7f1; projeto 6000.6.1f1; GDD “20 s”; boss 1200 vs 3300; plano histórico | Decisões e tuning ficam ambíguos | Atualizar GDD/plano ou marcar como histórico | Nenhuma |
| M-15 | MÉDIO | Build | Versão 0.9.8 em `BuildScript.cs` e `ProjectSettings.asset`; build manual | Versão divergente/erro humano | Fonte única de versão; CI | Nenhuma |
| M-16 | MÉDIO | Publicação | itch só com capa; sem screenshots de gameplay, GIFs, trailer | Página pobre para público; conversão baixa | Capturar 5 prints/GIFs/trailer | H-01/H-05 |
| M-17 | MÉDIO | Save | Sem teste runtime de save/meta entre sessões | Risco de perda de progresso não detectado | Teste PlayMode/persistência | Nenhuma |
| M-18 | MÉDIO | UI | `MakeText` não define `maxSize`/overflow; cena manual difere | Textos podem clipar/ser perdidos | Centralizar layout no gerador | M-01 |
| M-19 | MÉDIO | Editor | `SwarmSceneBuilder` ~1.600 linhas; `BuildAndValidateBatch` destrutivo | Rerun apaga WIP; manutenção difícil | Separar validação de geração; script de import | H-03 |
| M-20 | MÉDIO | Legal | Sem `LICENSE` no repo público | Uso/contribuição indefinidos; risco comercial | Definir licença (código/assets) | Decisão do autor |

---

## 3. Riscos baixos

| ID | Gravidade | Área | Evidência | Consequência | Correção |
|---|---|---|---|---|---|
| L-01 | BAIXO | UX | `OptionsMenu` só fecha com `O` | `ESC` não fecha opções | Aceitar `Back` |
| L-02 | BAIXO | Debug | `DebugOverlay` roda em build | Jogador vê FPS/stats | `#if UNITY_EDITOR` ou flag |
| L-03 | BAIXO | Save | `BestKills`, `BestTime`, `ResetAll` não usados | Código morto | Remover ou exibir recordes |
| L-04 | BAIXO | Áudio | `Sfx` muda `pitch` de um único `AudioSource` | One-shots sobrepostos desafinam | Pool de sources |
| L-05 | BAIXO | Screenshot | Nome por segundo | Duas capturas no mesmo segundo sobrescrevem | Incluir ms/GUID |
| L-06 | BAIXO | Código | `XpSystem` diz “curva linear” | Comentário stale | Corrigir comentário |
| L-07 | BAIXO | Upgrades | `IsOffensive` ignora Orbitais | Primeira escolha pode não ter dano direto | Incluir Orbitals/OrbDamage |
| L-08 | BAIXO | Assets | `SampleScene`, template URP, InputActions não usados | Ruído no projeto | Remover com autorização |
| L-09 | BAIXO | Boss | `Enemy.Die` despawna boss imediatamente | Sem animação de morte do boss | Tocar death sprite/rotina |
| L-10 | BAIXO | Projétil | `Projectile.Init` gira sem offset -90 | Arte direcional pode apontar errado | Ajustar offset ou manter diamante |
| L-11 | BAIXO | Input | Gamepad só move | Menus/upgrades exigem teclado | Mapear botões |
| L-12 | BAIXO | Combate | Um inimigo dano por 0,4 s | Múltiplos contatos não escalam | Validar design |
| L-13 | BAIXO | Qualidade | Ultra/MSAA 2 para pixel art | Custo desnecessário | Preset 2D |
| L-14 | BAIXO | URP | 7 GUIDs de `Renderer2D` não achados | Debug/probe/falloff ausente | Verificar se 2D lights serão usadas |
| L-15 | BAIXO | Git | `docs/art/sprites-preview.*` não commitado | Preview local apenas | Commitar ou remover |

---

## 4. Matriz de risco

| Risco | Probabilidade | Impacto | Prioridade |
|---|---|---|---|
| Arte não integrada | Alta | Alto | **P0** |
| Import settings errados | Alta | Alto | **P0** |
| WIP perdido | Média | Alto | **P0** |
| Senha pública | Alta | Alto | **P0** |
| Build stale | Alta | Alto | **P0** |
| Boss HP divergente | Média | Médio | P1 |
| Restart -1 | Baixa | Médio | P1 |
| Física sem layers | Alta | Médio | P2 |
| Sem atlas | Alta | Médio | P2 |
| Sem testes/CI | Alta | Médio | P1 |
| Sem Profiler | Alta | Médio | P2 |
| Layout de UI perdido | Média | Médio | P1 |
| Documentação divergente | Alta | Baixo/médio | P1 |
| Sem licença | Média | Médio (legal) | P2 |

---

## 5. O que **não** é problema

- `Enemy.OnEnable` reativa colisor; o bug crítico do pool está corrigido.
- `PlayerVisual` usa a mesma prioridade da arma; a nave aponta para o alvo certo.
- `HudController.Choose` ignora tecla acima do número de opções.
- `ObjectPool.Despawn` evita double-enqueue.
- `SpawnWarning` não spawna durante o boss.
- URP sem strip de post-processing; Bloom/Vignette no build.
- Cena sem scripts nulos.
- Eventos principais desinscritos.
- HUD com throttle e sem strings por frame.
- Sem LINQ/FindObjectOfType no runtime.

---

## 6. Conclusão

O projeto **não está em crise de código**. Está em crise de **estado**:

- o código está à frente da build;
- a arte está à frente da integração;
- o WIP está à frente do Git;
- o playtest está à frente da segurança.

Resolver os cinco ALTOS acima é mais importante do que adicionar qualquer inimigo,
upgrade ou mapa.
