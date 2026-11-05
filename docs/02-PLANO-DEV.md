# Plano de Desenvolvimento — SWARM

Premissa: **1 pessoa, ~15 h/semana**. Cada sprint = 1 semana. Total até a demo: **~10 semanas**.

Regra que vale mais que o cronograma: **não avance de sprint com o anterior "mais ou menos".**
Um loop divertido e feio vence um jogo bonito e chato.

---

## Visão geral

| Sprint | Semana | Objetivo | Entregável |
|---|---|---|---|
| 0 | 1 | Ambiente + protótipo rodando | Jogo feio jogável |
| 1 | 2 | Loop núcleo divertido | "Quero jogar de novo" |
| 2 | 3 | XP + upgrades | Build aparece |
| 3 | 4 | Juice + balanceamento | **Vertical slice** |
| 4 | 5–6 | Conteúdo (boss, armas, inimigos) | Demo interna |
| 5 | 7 | Meta-progressão + save | Retenção |
| 6 | 8 | Performance + QA | 60 fps estável |
| 7 | 9 | Build + Steam page + playtest | Demo pública |
| 8 | 10 | Polimento + trailer | Pronto para wishlist |

---

## Sprint 0 — Ambiente (dias 1–3)

**Meta:** ver quadrados se movendo na tela.

- [ ] Instalar Unity **6000.6.7f1** no Windows (ver `03-SETUP.md`)
- [ ] Criar projeto 2D (URP), instalar Input System
- [ ] Copiar `Assets/Swarm`, rodar `SWARM > Montar Protótipo Completo`
- [ ] Play: mover, atirar, matar, level up, morrer, reiniciar
- [ ] `git init` + primeiro commit

**DoD:** o ciclo completo roda sem erro no Console e sem tocar em nenhum arquivo.

**Armadilha:** se algo não compilar, resolva **antes** de escrever código novo.
Console limpo é pré-requisito, não luxo.

---

## Sprint 1 — Loop núcleo (semana 2)

**Meta:** a pergunta "já acabou?" some.

- [ ] Ajustar `moveSpeed`, `fireRate`, `range` até o movimento parecer bom
- [ ] Ajustar velocidade/HP dos inimigos até a horda pressionar sem ser injusta
- [ ] Implementar morte do jogador com clareza (shake + som + painel)
- [ ] Testar 10 runs seguidas e anotar o minuto de morte (`F1` + CSV automático)
- [ ] Corrigir o que mais incomodou

**DoD:** 10 runs com duração entre 2 e 6 min, e você quer jogar a 11ª.

**Truque:** jogue sem upgrades (desligue o `XpSystem`) por 20 min. Se ainda for
divertido desviar e atirar, o núcleo está bom. Se não, nenhum upgrade salva.

---

## Sprint 2 — Progressão (semana 3)

- [ ] Curva de XP: medir level ups por run (alvo 12–20)
- [ ] 12 upgrades preenchidos e balanceados
- [ ] Tela de level up legível em 1 segundo (3 linhas, sem parágrafo)
- [ ] Fila de level ups (matar boss e subir 2 níveis não pode quebrar)
- [ ] Feedback ao escolher (som + HUD atualiza na hora)

**DoD:** ao morrer, você consegue dizer em uma frase qual era sua build.

---

## Sprint 3 — Juice e vertical slice (semana 4)

- [ ] Flash de dano, shake calibrado, SFX com volume/pitch variados
- [ ] Números de dano ou barras de vida nos inimigos (escolha 1)
- [ ] Partículas simples de morte (Sprint 4 se faltar tempo)
- [ ] Balancear os 10 minutos inteiros
- [ ] Gravar um GIF de 10 s e mostrar para 3 pessoas

**DoD:** alguém que não é você pede para jogar de novo.

---

## Sprint 4 — Conteúdo (semanas 5–6)

- [ ] Boss Núcleo aos 5:00 (HP alto, lento, drop de XP grande)
- [ ] 2ª arma (ex.: orbe orbital) e sistema de armas
- [ ] 6 inimigos no total, 20 upgrades
- [ ] 3 personagens com stats diferentes
- [ ] Tela de seleção de personagem

**DoD:** 3 builds distintas e reconhecíveis.

---

## Sprint 5 — Meta-progressão (semana 7)

- [ ] Tela de Núcleos com 3 upgrades permanentes
- [ ] Save robusto (PlayerPrefs → JSON em `persistentDataPath` se crescer)
- [ ] Reset de save
- [ ] Primeira run "nova" com bônus permanentes

**DoD:** fechar o jogo e reabrir mantém tudo.

---

## Sprint 6 — Performance e QA (semana 8)

- [ ] Profiler: 300 inimigos + 100 projéteis a 60 fps
- [ ] Pooling em tudo que nasce/morre
- [ ] Remover alocações por frame (LINQ, `GetComponent`, strings)
- [ ] Teste em máquina fraca / resolução 1280x720
- [ ] Checklist de `10-PERFORMANCE-QA.md` inteiro

**DoD:** 0 GC spikes visíveis no Profiler e 0 warnings no Console.

---

## Sprint 7 — Publicação (semana 9)

- [ ] Build Windows x64
- [ ] Página no itch.io (grátis) para playtest
- [ ] Página Steam "em breve" + trailer de 30 s
- [ ] 10 playtesters externos + formulário de 5 perguntas
- [ ] Corrigir os 3 problemas mais citados

**DoD:** link público funcionando e 10 respostas coletadas.

---

## Sprint 8 — Polimento (semana 10)

- [ ] Tutorial sem texto (a primeira tela ensina a mover)
- [ ] Opções: volume, tela cheia, qualidade
- [ ] Ajuste final de dificuldade
- [ ] Trailer final + 5 GIFs para redes
- [ ] Demo congelada (não mexer mais)

---

## Rotina semanal

| Dia | Foco |
|---|---|
| Seg | Planejar sprint (30 min) e escolher 3 tarefas |
| Ter–Qui | Produção, 2 h por dia, sem redes sociais |
| Sex | **Playtest**: 3 runs seguidas + anotar |
| Sáb | Corrigir o que o playtest apontou |
| Dom | Devlog curto (print/GIF) + descanso |

**Regra dos 20 minutos:** travou em algo por 20 min? Anota, pula, volta depois.
Bloqueio não é desafio, é desperdício de sprint.

---

## Riscos

| Risco | Prob. | Impacto | Mitigação |
|---|---|---|---|
| Escopo inflar | Alta | Alto | MVP congelado; ideias novas vão para `ideias.md` |
| Performance cair | Média | Alto | Pooling desde o dia 1; testar com 300 inimigos no Sprint 2 |
| Arte travar o projeto | Alta | Médio | Quadrados até o Sprint 4; contratar/asset pack depois |
| Burnout | Média | Alto | 1 dia off/semana; sprint de 5 dias, não 7 |
| Ninguém jogar | Média | Alto | GIF semanal público desde o Sprint 3 |
| Unity quebrar ao atualizar | Baixa | Médio | Travar versão no `ProjectVersion.txt`; atualizar só entre sprints |

---

## Definition of Done (vale para toda tarefa)

1. Roda sem erro/warning no Console.
2. Testado em Play mode por 3 runs.
3. Números de tuning em um só lugar (não hardcoded no meio da lógica).
4. Commit com mensagem clara (`feat: spawner com curva de dificuldade`).
5. Se mudou o feel, atualizou o GDD.
