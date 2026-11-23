# Página de feedback — playtest 0.9.8

Duas opções:

| Opção | Melhor para | Coleta |
|---|---|---|
| **Google Forms** | testers remotos, respostas automáticas | planilha do Google |
| **Página HTML** (`docs/feedback/index.html`) | visual do jogo, hospedagem própria, offline | testador copia/baixa e envia |

**Recomendação:** Google Forms para a rodada oficial e a página HTML como link
alternativo ou para quem preferir responder sem login.

---

## Opção A — Google Forms

### 1. Criar

1. Abra `forms.google.com` → **Em branco**.
2. Título: `SWARM — Feedback de playtest`.
3. Descrição:

```text
Obrigado por testar a demo do SWARM! São 3 minutos.
Responda depois de jogar pelo menos uma run.
Não colete dados pessoais; o formulário é anônimo.
```

4. Tema: cor de destaque `#4DFFE0` (ciano), fundo escuro se disponível.
5. Configurações:
   - **Não coletar e-mails**.
   - **Não limitar a 1 resposta** (a pessoa pode testar mais de uma build).
   - **Mostrar barra de progresso**.
   - Embaralhar perguntas: desligado.
   - Ative **notificações de novas respostas** por e-mail.
   - Em **Respostas**, clique em **Vincular à planilha** para gerar o CSV.

### 2. Perguntas

Crie na ordem abaixo. Use **Seção** para separar os blocos.

#### Seção 1 — Sobre você

| # | Pergunta | Tipo | Obrigatória | Opções |
|---|---|---|---|---|
| 1 | Nome ou apelido | Resposta curta | Não | — |
| 2 | Build testada | Resposta curta | Sim | `0.9.8` |
| 3 | Windows e placa de vídeo | Resposta curta | Não | — |

#### Seção 2 — Primeira impressão

| # | Pergunta | Tipo | Obrigatória | Opções |
|---|---|---|---|---|
| 4 | Você conseguiu abrir e jogar? | Múltipla escolha | Sim | Sim / Não / Com ajuda |
| 5 | Quantas runs você jogou? | Múltipla escolha | Sim | 1 / 2 / 3+ / Não joguei |
| 6 | Até onde você chegou? | Múltipla escolha | Sim | Antes de 3 min / 3–5 min / Boss (5:00) / Vitória (10:00) / Não joguei |
| 7 | Em quanto tempo entendeu que o tiro é automático? | Múltipla escolha | Sim | Menos de 10 s / 10–30 s / Mais de 30 s / Não entendi |

#### Seção 3 — Ritmo e clareza

| # | Pergunta | Tipo | Obrigatória | Escala |
|---|---|---|---|---|
| 8 | Dificuldade | Escala linear | Sim | 1 = fácil · 5 = injusto |
| 9 | Clareza do level up | Escala linear | Sim | 1 = confuso · 5 = óbvio |
| 10 | Jogaria de novo agora? | Escala linear | Sim | 1 = não · 5 = sim |

#### Seção 4 — O que você sentiu

| # | Pergunta | Tipo | Obrigatória |
|---|---|---|---|
| 11 | O que mais te confundiu ou incomodou? | Parágrafo | Não |
| 12 | O que você mudaria primeiro? | Parágrafo | Não |
| 13 | O que você mais gostou? | Parágrafo | Não |

#### Seção 5 — Bugs

| # | Pergunta | Tipo | Obrigatória | Opções |
|---|---|---|---|---|
| 14 | Encontrou algum bug, travamento ou tela preta? | Múltipla escolha | Sim | Não / Sim |

Em **“Ir para a seção com base na resposta”**:

- `Não` → Seção 6.
- `Sim` → Seção 5b.

#### Seção 5b — Detalhes do bug

| # | Pergunta | Tipo | Obrigatória |
|---|---|---|---|
| 15 | Descreva o bug e o momento | Parágrafo | Sim |

Descrição da seção:

```text
Se conseguir, envie também o Player.log e o swarm-runs.csv de:
%USERPROFILE%\AppData\LocalLow\Scoty\SWARM\
```

Depois da seção 5b, continue para a Seção 6.

#### Seção 6 — Para terminar

| # | Pergunta | Tipo | Obrigatória | Opções |
|---|---|---|---|---|
| 16 | Consentimento | Caixas de seleção | Sim | `Concordo em enviar estas respostas para melhorar o jogo.` |
| 17 | Podemos citar seu feedback (sem seu nome) em um devlog? | Múltipla escolha | Sim | Sim / Não |
| 18 | Comentário livre | Parágrafo | Não | — |

> Versão curta (2 min): mantenha as perguntas 4–10, 14, 16 e 17. O resto é opcional.

### 3. Mensagem de confirmação

```text
Obrigado! Seu feedback foi registrado.

Se você marcou um bug, envie o Player.log e o swarm-runs.csv para o dev
no Discord/WhatsApp/e-mail combinado. Eles ficam em:
%USERPROFILE%\AppData\LocalLow\Scoty\SWARM\

Se quiser acompanhar a próxima build, entre no canal de playtest.
```

### 4. Link

Clique em **Enviar** → copie o link curto. Se quiser, encurte com `bit.ly`
ou `tinyurl` e use sempre o mesmo link nas mensagens.

> Não ative “coletar e-mails” nem “limitar a 1 resposta” nesta rodada.
> O teste é anônimo e a pessoa pode responder de novo após uma build nova.

---

## Opção B — Página HTML pronta

Arquivo:

```
docs/feedback/index.html
```

Ela tem o mesmo conteúdo do formulário, no visual do jogo. Funciona em dois modos:

| Modo | Como funciona |
|---|---|
| **Local** (padrão) | O testador preenche, clica em **Copiar respostas** ou **Baixar .txt** e envia para o dev. |
| **Envio automático** | Com um endpoint configurado, o botão **Enviar feedback** grava a resposta no painel do serviço. |

O envio automático está pronto para **Formspree** (ou qualquer endpoint JSON).
O passo a passo está em `docs/23-FEEDBACK-BACKEND.md`.

### Como ativar o envio

1. Crie um formulário no Formspree e copie o endpoint.
2. Abra `docs/feedback/index.html`.
3. Procure:

```js
const FEEDBACK_ENDPOINT = '';
```

4. Cole o endpoint:

```js
const FEEDBACK_ENDPOINT = 'https://formspree.io/f/SEU_ID';
```

5. Salve, publique e teste em aba anônima.
6. Confirme a resposta no painel do Formspree.

### Como hospedar

**GitHub Pages (se o repositório for público):**

1. Suba o arquivo no repositório.
2. No GitHub: **Settings → Pages**.
3. Em **Source**, escolha a branch `main` e a pasta `/docs`.
4. A URL fica:

```
https://matmvb.github.io/swarm/feedback/
```

**Netlify Drop (sem git):**

1. Abra `app.netlify.com/drop`.
2. Arraste a pasta `docs/feedback`.
3. Copie a URL gerada.

**Local:** abra `docs/feedback/index.html` no navegador. Funciona offline.

### Limitações

- Sem endpoint, não há planilha automática; o testador precisa enviar o texto.
- O botão **Copiar** pode pedir permissão do navegador; o fallback seleciona o texto.
- Em `file://`, o botão de e-mail pode abrir o app padrão; o copy continua funcionando.
- Com endpoint, o envio depende do serviço (Formspree tem limite no plano free).

---

## Convite para os testadores

Mensagem curta:

> Obrigado por testar o SWARM! Quando terminar sua run, responde esse formulário de 3 minutos:
>
> **[LINK DO FORMULÁRIO]**
>
> É anônimo. Se encontrar bug, o formulário explica como enviar o log. Valeu!

Se estiver usando a página HTML:

> ... responde essa página e clica em **Enviar feedback**. Se o envio automático não estiver ativo, clica em **Copiar respostas** e cola aqui no chat.

---

## Planilha de triagem

Depois de 3 dias, exporte as respostas para uma planilha com estas colunas:

| Coluna | Exemplo |
|---|---|
| data | 2026-11-25 |
| tester | “anônimo 3” |
| build | 0.9.8 |
| abriu | Sim |
| runs | 2 |
| progresso | Boss (5:00) |
| tiro_automatico | Menos de 10 s |
| dificuldade | 4 |
| clareza | 2 |
| jogaria_de_novo | 5 |
| bug | Não |
| citar | Sim |
| consentimento | Sim |
| confundiu | “não entendi o upgrade de orbe” |
| mudaria | “mais contraste no dano” |
| gostou | “caos da horda” |
| comentario | — |
| tags | confusão, balance |
| acao | ajustar texto do upgrade |

Arquivo pronto: `docs/feedback/triage.csv`.

### Regras de triagem

- **Bloqueador** (não abre, crash, tela preta): corrigir antes de tudo.
- **Confusão** (não entendeu upgrade, boss, dano): ajustar UI/texto.
- **Balance** (morreu cedo/late demais): mexer em 1–2 números.
- **Elogio**: manter.
- **Pedido de conteúdo**: anotar, não implementar agora.

Só aja no que apareceu **2+ vezes** ou no que é bloqueador.

---

## Checklist

- [ ] Google Forms criado e testado em aba anônima.
- [ ] Link do formulário copiado.
- [ ] Página HTML revisada (se for usar).
- [ ] Hospedagem da página HTML definida.
- [ ] Mensagem de convite com link e prazo.
- [ ] Planilha de respostas pronta.
- [ ] `triage.csv` copiado para a planilha.
- [ ] Prazo de 3 dias comunicado.
- [ ] Respostas triadas após o prazo.
