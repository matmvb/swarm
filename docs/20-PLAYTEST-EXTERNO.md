# Playtest externo — 0.9.8

**Objetivo:** colocar a demo na mão de 5–8 pessoas, sem explicar as regras, e descobrir onde elas travam, se divertem ou desistem.

**Formato:** 1–2 runs por pessoa (10–20 min), feedback em até 3 dias.

**O que você quer aprender nesta rodada:**

1. A pessoa entende que o tiro é automático?
2. Ela entende o objetivo (sobreviver 10 min / matar o boss)?
3. O level up é claro e rápido?
4. A dificuldade está justa até o boss?
5. Ela quer jogar de novo?

---

## 1. Canal de distribuição

### Recomendado: itch.io com página **Restricted**

Você já tem a página pronta em `docs/17-PAGINA-ITCH.md`. Suba como **Restricted** (não pública), com senha, e envie o link secreto. Vantagens:

- download versionado;
- estatísticas de acesso;
- testers não precisam de conta;
- você troca a build sem mudar o link.

### Alternativa: link direto (Drive/Dropbox)

Para 1–3 pessoas de confiança. Suba o ZIP e compartilhe como “qualquer pessoa com o link”. Não use anexo no Discord: o ZIP tem 39 MB e o limite gratuito é menor.

> **Não torne a página pública ainda.** Faltam screenshots, GIFs e teste em outra máquina.

---

## 2. Preparar o pacote

O ZIP de teste é:

```
Builds/SWARM-windows.zip   (0.9.8, ~39 MB)
```

Ele já contém `LEIA-ME.txt` na raiz. Para regerar depois de uma mudança:

1. Copie `docs/LEIA-ME-PLAYTEST.txt` para `Builds/LEIA-ME.txt`.
2. Refaça o ZIP incluindo `LEIA-ME.txt`.
3. Teste extraindo em uma pasta limpa e abrindo `SWARM.exe`.

> **Nunca** envie a pasta `Builds/Windows` solta. O tester precisa extrair o ZIP inteiro, senão faltam DLLs.

### Aviso para colocar na mensagem

> “O Windows pode mostrar um aviso de ‘aplicativo não reconhecido’ porque o executável não é assinado. Clique em **Mais informações → Executar assim mesmo**.”

---

## 3. Subir no itch.io (passo a passo)

1. Entre em `itch.io` → **Dashboard** → **Create new project**.
2. Preencha a ficha de `docs/17-PAGINA-ITCH.md`:
   - Title: **SWARM**
   - Classification: **Game**
   - Kind of project: **Downloadable**
   - Pricing: **$0 or donate**
   - Release status: **In development**
   - Platforms: **Windows**
3. Em **Uploads**, envie `Builds/SWARM-windows.zip`.
   - Marque o arquivo como **Windows**.
   - Deixe **desmarcado** “This file will be played in the browser”.
4. Adicione a capa `docs/press/cover-630x500.png`.
5. Adicione pelo menos 1 screenshot. Ideal: 3–5.
6. Cole a descrição, os controles e as tags.
7. Em **Visibility**, escolha **Restricted**.
8. Defina uma senha simples, por exemplo `swarmtest`.
9. Salve e copie a URL secreta.
10. Abra a URL em uma janela anônima, baixe e confirme que o ZIP abre.

### Versão no itch

Para a primeira rodada, use o campo de versão `0.9.8`. Quando corrigir algo, suba um arquivo novo e mude a versão para `0.9.9` (ou `0.9.8b`). Não sobrescreva o arquivo antigo sem avisar os testers.

### Opcional: butler (para iterações futuras)

```bash
butler push Builds/SWARM-windows.zip usuario/swarm:windows --userversion 0.9.8
```

---

## 4. Convidar as pessoas

**Quantidade:** 5–8. Ideal:

- 3–4 que gostam de survivors/roguelite/arcade;
- 2–3 que quase não jogam esse gênero;
- 1 que já testou uma versão anterior, se existir.

**Não explique as regras antes.** O teste é para descobrir se o jogo se explica sozinho.

### Mensagem pronta (WhatsApp/Discord/e-mail)

> E aí! Estou testando a demo do **SWARM**, um bullet heaven 2D estilo Vampire Survivors + Space Invaders. A run dura até 10 min.
>
> Queria que você jogasse **1 run inteira, sem eu explicar nada**, e depois respondesse um formulário curto (3 min). Se morrer antes de 3 min, joga mais uma.
>
> É Windows e não precisa instalar: baixa o ZIP, extrai e abre `SWARM.exe`.
>
> Link: **[COLE A URL RESTRITA AQUI]**
> Senha: **swarmtest**
>
> Dois avisos:
> 1. O Windows pode dizer que o app não é reconhecido (não é assinado). Clique em **Mais informações → Executar assim mesmo**.
> 2. Se abrir mudo, aperte **O** e depois **+**.
>
> Prazo: **[DATA]**. O formulário está aqui: **[COLE O LINK DO FORMULÁRIO]**.
>
> Valeu demais! 🙏

**Peça para não ler o `LEIA-ME` antes da primeira run.** Ele está no ZIP para consulta se travar, mas queremos observar a descoberta.

---

## 5. Formulário de feedback

Use Google Forms, Typeform ou Tally. Deixe **anônimo**; peça e-mail só se a pessoa quiser receber atualizações.

### Estrutura sugerida (10 perguntas)

| # | Pergunta | Tipo |
|---|---|---|
| 1 | Você conseguiu abrir e jogar? | Sim / Não / Com ajuda |
| 2 | Seu Windows e placa de vídeo (se souber) | Texto curto |
| 3 | Quantas runs você jogou? | 1 / 2 / 3+ |
| 4 | Até onde você chegou? | Morri antes de 3 min / 3–5 min / boss / vitória |
| 5 | Em quanto tempo você entendeu que o tiro é automático? | < 10 s / 10–30 s / demorei / não entendi |
| 6 | O que você achou da dificuldade? | 1 (fácil) a 5 (injusto) |
| 7 | O level up era claro? | 1 (confuso) a 5 (óbvio) |
| 8 | Você jogaria de novo agora? | 1 (não) a 5 (sim) |
| 9 | O que mais te confundiu ou incomodou? | Texto longo |
| 10 | O que você mudaria primeiro? | Texto longo |

Perguntas extras para bugs:

- “Aconteceu algum erro, travamento ou tela preta? Descreva o momento.”
- “Se tiver print ou o arquivo `Player.log`, arraste aqui.”

---

## 6. O que observar (mesmo à distância)

Peça na mensagem que a pessoa **não leia nada antes**. Durante o teste, se estiver junto, observe:

| Sinal | O que significa |
|---|---|
| Mexeu o mouse procurando mira | não entendeu que o tiro é automático |
| Ficou parado no menu | não viu o ENTER |
| Leu as 3 opções por mais de 4 s | upgrade com texto denso |
| Não percebeu que levou dano | feedback de dano fraco |
| Tentou apertar botão para atirar | mesmo problema da mira |
| Saiu antes de 3 min | ritmo inicial ou dificuldade |
| Reiniciou sozinho | o loop prendeu |

Para teste remoto, transforme esses sinais em perguntas do formulário.

---

## 7. Coletar logs e prints

Só peça os arquivos se houver bug. Os caminhos são:

```
%USERPROFILE%\AppData\LocalLow\Scoty\SWARM\Player.log
%USERPROFILE%\AppData\LocalLow\Scoty\SWARM\swarm-runs.csv
%USERPROFILE%\AppData\LocalLow\Scoty\SWARM\shots
```

Peça também:

- versão do Windows;
- modelo da placa de vídeo, se souber;
- o que estava acontecendo no momento.

> O CSV tem uma linha por run e ajuda a comparar duração, nível, personagem e tempo de boss.

---

## 8. Triagem do feedback

Depois de 3 dias, classifique cada resposta:

| Categoria | Exemplo | Ação |
|---|---|---|
| **Bloqueador** | não abre, crash, tela preta | corrigir antes de qualquer coisa |
| **Confusão** | não entendeu upgrade, não viu o boss | ajustar UI/texto |
| **Balance** | morreu em 2 min, boss fácil | mexer em 1–2 números |
| **Elogio** | “gostei do caos” | manter |
| **Pedido** | “queria mais armas” | anotar para depois |

**Regra:** só mexa no que apareceu **2+ vezes** ou no que é bloqueador. Uma pessoa pedindo uma arma nova não é sinal para adicionar conteúdo.

---

## 9. Critérios de sucesso desta rodada

- [ ] 5+ pessoas conseguiram abrir e jogar.
- [ ] Nenhum crash bloqueador.
- [ ] 60%+ chegaram ao boss (5:00).
- [ ] Mediana de duração entre 5 e 10 min.
- [ ] 6 de 8 responderam “jogaria de novo” com 4 ou 5.
- [ ] Ninguém ficou preso no menu ou no level up.

Se passar, a próxima rodada é **página pública + screenshots + trailer**. Se não passar, corrija os 2 problemas mais repetidos e repita com 3–5 pessoas.

---

## 10. Checklist rápido

- [ ] ZIP testado em pasta limpa.
- [ ] `LEIA-ME.txt` dentro do ZIP.
- [ ] Página Restricted + senha.
- [ ] Link testado deslogado.
- [ ] Formulário pronto e anônimo.
- [ ] Mensagem com prazo e link do formulário.
- [ ] 5–8 testadores convidados.
- [ ] Caminho dos logs na mensagem.
- [ ] Feedback triado em 3 dias.
- [ ] Build corrigida e versão nova, se necessário.
