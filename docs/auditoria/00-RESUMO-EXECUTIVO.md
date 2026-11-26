# 00 — Resumo executivo

**Projeto:** SWARM  
**Data:** 24/11/2026  
**Modo:** auditoria somente leitura  
**Branch:** `main` · commit `e0c9fe7`  
**Unity:** `6000.6.1f1` (`ProjectVersion.txt`)

---

## Situação geral

O SWARM está em um estado de **vertical slice avançado / pré-beta**, não de release.

O núcleo de gameplay existe, compila e a cena principal passa na validação de referências.
A build Windows 0.9.8 existe e está limpa, mas **foi gerada antes das mudanças atuais**
de cena e arte. O maior risco neste momento não é “o jogo não roda”: é **o estado real
do projeto estar à frente do que está commitado, buildado e testado**.

| Dimensão | Situação |
|---|---|
| Código | Bom para protótipo; compila sem erro |
| Cena principal | Validada: 0 problemas de referência |
| Gameplay | Implementado; 0.9.8 não tem run completa registrada |
| Arte nova | 72 PNGs produzidos, **não integrados** e com importação errada |
| Build | 0.9.8 existe, limpa, mas **stale** em relação ao working tree |
| Testes | Sem testes automatizados; sem CI |
| Performance | Sem Profiler/stress test; metas não comprovadas |
| Git | `main` sincronizada, mas com WIP não commitado |
| Publicação | Pages/itch no ar; senha do itch exposta em docs públicos |
| Documentação | Boa cobertura, com divergências de versão e plano histórico |

---

## O que foi comprovado

- Unity **6000.6.1f1** instalada e configurada.
- `SwarmSceneBuilder.ValidateSceneBatch` executado em batch: **0 problemas**.
- Compilação sem `error CS`, `warning CS`, `NullReferenceException` ou exceções no log.
- Nenhum `m_Script: {fileID: 0}` na cena/assets.
- Build 0.9.8 presente, versão correta, ZIP sem PDB, sem pasta de backup e com `SWARM.exe` na raiz.
- `Player.log` de 0.9.8 **sem** os avisos de shader stripped que existiam na 0.9.5/0.9.7.
- GitHub Pages, feedback, privacidade e concept sheet respondendo 200.
- itch.io respondendo 200 com senha; download `SWARM-windows.zip` de 38 MB.
- Formspree com CORS liberado; POSTs de teste anteriores retornaram `{"ok":true}`.

## O que não foi comprovado

- Uma run completa na build 0.9.8 atual.
- Funcionamento da arte nova dentro do jogo.
- Persistência de save/meta em runtime.
- 60 fps com 300 inimigos / 100 projéteis.
- Build gerada a partir do working tree atual.
- Teste em outra máquina.
- Fluxo completo de level up, boss, morte e restart na versão atual.

---

## Principais descobertas

| ID | Gravidade | Descoberta |
|---|---|---|
| H-01 | ALTO | 72 sprites novos existem, mas **0 referências na cena**; o jogo continua usando as formas procedurais. |
| H-02 | ALTO | Os `.meta` dos 72 sprites estão com **PPU 100, Bilinear, Multiple, Tight, Extrude 1**, divergindo do `manifest.json` (PPU por frame, Point, FullRect, Extrude 0). |
| H-03 | ALTO | Cena e arte estão **não commitadas**; a build 0.9.8 é de 22/11 e não contém esse WIP. |
| H-04 | ALTO | A senha real do itch está em documentos públicos do repositório/Pages. |
| H-05 | ALTO | A build publicada está **stale** em relação à cena/arte atuais; não há run 0.9.8 completa. |
| M-01 | MÉDIO | A cena tem ajustes manuais de `m_MaxSize` que o gerador não reproduz; rodar o gerador pode perdê-los. |
| M-02 | MÉDIO | Boss é identificado por `data.scale > 1.4f`; além disso, o HP real no spawn é 1200 × 2,75 = **3300**, enquanto o GDD lista 1200. |
| M-03 | MÉDIO | `GameManager.Restart` usa `buildIndex`; se a cena ativa não estiver no Build Settings, o índice é -1. |
| M-04 | MÉDIO | Não há layers/matriz de colisão; todos os pares de física estão ativos. |
| M-05 | MÉDIO | Não há Sprite Atlas; 72 texturas novas podem elevar draw calls. |
| M-06 | MÉDIO | `PlayerVisual` faz `FindNearest` a cada frame e `OrbitalWeapon` faz O(orbs × inimigos) a cada frame. |
| M-07 | MÉDIO | Pool não tem pré-aquecimento; o primeiro combate pode gerar hitch. |
| M-08 | MÉDIO | Vários sprites novos têm cores fora da paleta e pixels semitransparentes. |
| M-09 | MÉDIO | Game Over só oferece `[R]`; não há retorno ao menu. |
| M-10 | MÉDIO | Sem testes automatizados, sem CI e sem dados de Profiler. |

---

## Maturidade

| Área | Nível |
|---|---|
| Loop jogável | 8/10 |
| Estabilidade de código | 7/10 |
| Integração de arte | 2/10 |
| Testes e QA | 3/10 |
| Performance comprovada | 2/10 |
| Build/release | 5/10 |
| Documentação | 7/10 |
| **Maturidade geral** | **5,5/10 — vertical slice avançado** |

---

## Cinco prioridades

1. **Preservar o WIP:** criar branch/commit para cena + 72 sprites + preview; hoje eles não estão no Git.
2. **Corrigir a importação e integrar a arte:** PPU por frame, Point, FullRect, Extrude 0; atribuir sprites em `EnemyData`, `CharacterData` e player.
3. **Proteger o playtest:** rotacionar a senha do itch e remover a senha real dos docs públicos.
4. **Gerar uma build do estado atual e rodar uma run completa 0.9.8:** menu → piloto → level up → boss → morte/vitória → restart.
5. **Fechar o básico de engenharia:** teste automatizado mínimo, CI de compilação/validação, camadas de colisão e um stress test com Profiler.

---

## Próxima tarefa recomendada

**Não integrar a arte ainda.** Primeiro:

```text
1. Criar branch de preservação do WIP.
2. Commitar cena + Assets/Swarm/Art/Sprites + manifest + preview.
3. Corrigir os import settings dos sprites em um script de editor separado.
4. Só então atribuir os sprites e rebuildar.
```

Isso evita perder o trabalho atual e evita descobrir tarde que a arte entrou com escala/filtro errados.
