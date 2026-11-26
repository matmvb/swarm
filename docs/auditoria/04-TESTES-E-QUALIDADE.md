# 04 — Testes e qualidade

**Data:** 24/11/2026  
**Ambiente:** Windows, Unity `6000.6.1f1`, projeto em `C:\Users\scoty\OneDrive\Documentos\Default Project\swarm`  
**Modo:** testes não destrutivos.

---

## 1. Condições do ambiente

| Item | Valor |
|---|---|
| Unity aberta durante a auditoria | **Não** |
| `Temp/UnityLockfile` | Ausente |
| Processo `Unity.exe` | Nenhum |
| Batch usado | `-batchmode -quit -nographics` |
| Fonte modificada pelos testes | **Não** (SHA-1 antes/depois = 0 diferenças) |

> Não foi executado `BuildAndValidateBatch`, porque ele **regenera** a cena e assets
> e poderia apagar o WIP manual de `Swarm.unity`. Foi usado apenas `ValidateSceneBatch`.

---

## 2. Testes executados

### 2.1 Compilação + validação da cena

Comando:

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

- Sem `error CS`.
- Sem `warning CS`.
- Sem `NullReferenceException`.
- Sem `MissingReferenceException`.
- Sem exceções no log.

**Limite:** o validador confere referências obrigatórias, componentes e templates.
Ele **não** valida import settings de sprites, integração de arte, layout de UI,
persistência, performance ou comportamento em Play Mode.

### 2.2 Integridade da cena e assets

| Teste | Resultado |
|---|---|
| `m_Script: {fileID: 0}` em cena/assets | 0 |
| Cena principal no Build Settings | Sim |
| Templates de inimigo | 6 |
| Template de projétil | 1 |
| Template de orbe | 1 |
| Template de aviso | 1 |
| Template de partícula | 1 |
| Template de projétil inimigo | 1 |
| Referências obrigatórias do HUD/menus | 0 problemas |
| GUIDs de assets ausentes | 7 em `Renderer2D.asset` (ver abaixo) |

**Renderer2D:** 7 GUIDs de debug/probe/falloff não encontrados no cache de pacotes.
São recursos internos do URP 2D; não impediram compilação, validação ou build.
Risco baixo, mas vale conferir se luzes 2D forem usadas no futuro.

### 2.3 Snapshot de source

Antes e depois do batch:

```text
find Assets ProjectSettings Packages docs -type f | sha1sum
```

Resultado:

```text
DIFF_LINES=0
```

**Nenhum arquivo de source foi alterado pelos testes.** O batch só escreveu em
`Library/` e `Logs/`.

### 2.4 Inventário de sprites novos

| Teste | Resultado |
|---|---|
| PNGs encontrados | 72 |
| Dimensões das folhas vs `manifest.json` | 72/72 corretas |
| Entradas no manifesto | 72 |
| Sprites referenciados na cena | **0** |
| Sprites referenciados no código | **0** |
| Nomes no build | **0** |
| `spritePixelsToUnits` | 100 em todos (manifesto espera 8–96) |
| `filterMode` | 1 (Bilinear) em todos (manifesto espera Point) |
| `spriteMode` | 2 (Multiple) em todos |
| `spriteMeshType` | 1 (Tight) |
| `spriteExtrude` | 1 |

**Conclusão:** os PNGs existem e têm os tamanhos certos, mas **não estão integrados
nem configurados** para o estilo pixel art.

### 2.5 Qualidade de pixels dos sprites novos

Foi feita leitura de pixels dos 72 PNGs.

Exemplos de anomalia:

| Asset | Cores únicas | Alphas não 0/255 | Observação |
|---|---:|---:|---|
| `proj_player.png` | 16 | 5 | cores fora da paleta, canais deslocados |
| `proj_enemy.png` | 18 | 5 | idem |
| `proj_boss.png` | 17 | 5 | idem |
| `orbital.png` | 14 | 5 | idem |
| `xp_orb.png` | 16 | 5 | idem |
| `core_pickup.png` | 14 | 5 | idem |
| `fx_explosion.png` | 26 | 7 | gradação/anti-alias |
| `fx_muzzle.png` | 19 | 5 | idem |
| `fx_thruster.png` | 17 | 5 | idem |
| `enemy_casco_death.png` | 18 | 0 | muitas cores |
| `ship_baluarte_death.png` | 38 | 0 | muitas cores |
| `enemy_esporo_idle.png` | 4 | 0 | **limpo** |
| `enemy_casco_hit.png` | 2 | 0 | **limpo** |

**Conclusão:** parte dos sprites está limpa; projéteis/FX e várias mortes têm cores
fora da paleta e/ou alpha. Isso precisa ser corrigido antes de integrar. A hipótese
mais provável é exportação com antialias/blend ou bug de canal no gerador.

### 2.6 Build existente

Não foi gerada uma nova build (para não sobrescrever o artefato nem alterar PlayerSettings).
A build existente foi auditada:

| Item | Resultado |
|---|---|
| Versão no `globalgamemanagers` | `0.9.8` |
| `SWARM.exe` | presente |
| ZIP | 202 entradas, 103,4 MB descomprimidos |
| `SWARM.exe` na raiz | sim |
| PDB | 0 |
| Pasta `BackUpThisFolder` | ausente |
| `LEIA-ME.txt` | presente |
| Timestamp da build | 22/11/2026 17:58 |
| Timestamp da cena atual | 24/11/2026 11:24 |
| Timestamp da arte | 23/11/2026 20:15 |

**A build não representa o working tree atual.**

### 2.7 Runtime existente

`Player.log` mais recente (0.9.8):

- engine 6000.6.1f1;
- D3D12 / RTX 3050;
- `[SWARM] Piloto escolhido: VELOZ`;
- sem `shader ... stripped`;
- sem exceptions/errors.

Isso confirma que a correção de Bloom/vinheta da 0.9.8 funcionou no build.
**Não** confirma uma run completa: o log termina após a escolha do piloto.

CSV (`swarm-runs.csv`) tem 3 linhas:

| Versão | Duração | Vitória | Boss |
|---|---:|---|---:|
| 0.5.0 | 7:01 | não | 73,8 s |
| 0.9.1 | 3:06 | não | — |
| 0.9.5 | 10:00 | sim | 15,9 s |

**Não há linha de 0.9.8.**

### 2.8 Publicação

| URL | HTTP | Observação |
|---|---:|---|
| Pages raiz | 200 | landing |
| Feedback | 200 | formulário |
| Privacidade | 200 | política |
| Concept sheet | 200 | PNG |
| itch sem senha | 200 | tela de senha |
| itch com senha | 200 | página real |
| Formspree OPTIONS | 200 | CORS |

**Segurança:** a senha real está em `docs/20`, `docs/21` e `docs/24`,
que são públicos. Isso anula o propósito do modo Restrito.

**Testes de Formspree:** POSTs anteriores retornaram 200/`{"ok":true}`. Existem três
submissões de teste pendentes (`TESTE AUTOMATICO`, `TESTE AUTOMATICO 2` e uma anônima
de 23/11 21:23 UTC). Não foi feito novo POST nesta auditoria para não criar mais lixo.

---

## 3. Testes não executados

| Teste | Motivo | Status |
|---|---|---|
| Run completa 0.9.8 | Não havia build atual; evitar abrir jogo/tela cheia | ❌ não executado |
| Play Mode automatizado | Sem testes/asmdef; exigiria criar código | ❌ não executado |
| Save entre sessões | Exigiria executar o jogo | ❌ não executado |
| Level up/boss/morte/restart | Exigiria Play Mode atual | ❌ não executado |
| Build Windows nova | Evitar sobrescrever build/PlayerSettings | ❌ não executado |
| Stress 300 inimigos + Profiler | Sem harness/Profiler capture | ❌ não executado |
| Teste em outra máquina | Sem acesso | ❌ não executado |
| Teste de acessibilidade/teclado | Requer runtime | ❌ não executado |

**Não declarei aprovação de runtime para o que não foi executado.**

---

## 4. Matriz de sistemas

| Sistema | Implementação | Validação | Situação |
|---|---|---|---|
| Movimento | Sim | Parcial | Código + cena; sem Play Mode atual |
| Combate | Sim | Parcial | Código + build antiga; sem run 0.9.8 |
| Inimigos | Sim | Parcial | 6 templates validados; comportamento não revalidado |
| Chefes | Sim | Parcial | Código; CSV 0.9.5 com 15,9 s |
| XP e upgrades | Sim | Parcial | Código; sem run 0.9.8 |
| Meta-progressão | Sim | Parcial | Código; sem teste de persistência |
| Interface | Sim | Parcial | Validador de cena; sem teste visual atual |
| Save | Sim | Não | Sem teste runtime |
| Performance | Parcial | Não | Sem Profiler/stress |
| Build | Sim | Parcial | Build 0.9.8 existe, mas stale |

---

## 5. Recomendações de QA

1. Criar uma **build do WIP atual** em branch separada e rodar uma run completa.
2. Adicionar um **teste EditMode** para:
   - curva de XP;
   - `UpgradeSystem.Roll` (pré-requisito, limite, cura, primeira ofensiva);
   - custo/compra de meta;
   - `ObjectPool` (spawn/despawn duplo).
3. Adicionar um **teste PlayMode** de fumaça:
   - escolher piloto;
   - matar 50 inimigos;
   - subir 5 níveis;
   - invocar boss;
   - matar jogador;
   - reiniciar.
4. Criar um **harness de stress** com 300 inimigos e 100 projéteis, medido no Profiler.
5. Só publicar nova build após o smoke test e a validação de importação da arte.
