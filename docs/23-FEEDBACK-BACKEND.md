# Backend do feedback — como receber as respostas

A página `docs/feedback/index.html` já tem o botão **Enviar feedback** e um
ponto de configuração:

```js
const FEEDBACK_ENDPOINT = '';
```

Se ficar vazio, a página funciona no **modo local** (copiar/baixar).
Se você colar um endpoint, ela faz `POST` JSON e o serviço grava a resposta.

```
Testador → página HTML → POST JSON → serviço → painel/e-mail/planilha
```

---

## 1. Recomendação: Formspree

Melhor custo/benefício para 5–8 testadores: não precisa de servidor, funciona
em GitHub Pages/Netlify e envia e-mail + painel.

### Passo a passo

1. Crie uma conta em `formspree.io`.
2. **New form** → nome `SWARM — Feedback`.
3. Confirme o e-mail da conta.
4. Copie o endpoint, no formato:

```text
https://formspree.io/f/abcdwxyz
```

5. Abra `docs/feedback/index.html` e procure:

```js
const FEEDBACK_ENDPOINT = '';
```

6. Cole o endpoint:

```js
const FEEDBACK_ENDPOINT = 'https://formspree.io/f/abcdwxyz';
```

7. Salve e publique a página.
8. Abra em aba anônima, preencha e clique em **Enviar feedback**.
9. Confira:
   - a mensagem “Feedback enviado. Obrigado!”;
   - a resposta no painel do Formspree;
   - o e-mail de notificação.

### O que a página envia

```json
{
  "_subject": "SWARM — Feedback de playtest",
  "_gotcha": "",
  "source": "swarm-feedback-page",
  "data": "2026-11-23T15:32:04.629Z",
  "build": "0.9.8",
  "name": "Teste",
  "machine": "",
  "opened": "Sim",
  "runs": "2",
  "progress": "Boss (5:00)",
  "autofire": "Menos de 10 s",
  "difficulty": "3",
  "clarity": "3",
  "again": "3",
  "confused": "",
  "change": "",
  "liked": "",
  "bug": "Não",
  "bugText": "",
  "consent": true,
  "quote": "Sim",
  "extra": ""
}
```

O campo `_gotcha` é um honeypot antispam: fica invisível e deve chegar vazio.
O `_subject` é usado pelo Formspree como assunto do e-mail.

### Teste rápido por terminal

```bash
curl -X POST 'https://formspree.io/f/SEU_ID' \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json' \
  -d '{"name":"Teste","build":"0.9.8","opened":"Sim","_subject":"Teste SWARM"}'
```

Resposta esperada: algo como `{"ok":true}`.

### Onde ver as respostas

- **Painel:** Formspree → Forms → SWARM — Feedback → Submissions.
- **E-mail:** a conta recebe uma notificação por envio.
- **Exportar:** baixe CSV pelo painel e cole em `docs/feedback/triage.csv`.
- **Limite:** confira o plano free atual (por volta de 50 envios/mês).
  Para 5–8 testadores, é suficiente.

### Vantagens e limites

| Vantagem | Limite |
|---|---|
| Zero backend | plano free tem limite mensal |
| Funciona em GitHub Pages/Netlify | endpoint é público (protegido por honeypot) |
| E-mail + painel + CSV | customização avançada é paga |
| Integra com qualquer HTML | não é dono da infraestrutura |

---

## 2. Alternativa: Netlify Forms

Se você hospedar a página no **Netlify**, pode usar o serviço de formulários deles.
Nesse caso, a submissão não usa `fetch`; o próprio Netlify intercepta o `<form>`.

### O que mudaria na página

O `<form>` precisaria de:

```html
<form name="swarm-feedback" method="POST" data-netlify="true" netlify-honeypot="_gotcha">
  <input type="hidden" name="form-name" value="swarm-feedback">
  ...
</form>
```

E o botão **Enviar feedback** viraria `type="submit"`, sem o `fetch`.

### Vantagens

- 100 envios/mês no plano free (confira o plano atual).
- Painel do Netlify + notificações por e-mail.
- Sem conta em outro serviço.

### Limite

- Só funciona hospedado no Netlify.
- Precisa adaptar o HTML (hoje ele está pronto para Formspree/JSON).

> Se você preferir Netlify Forms, eu adapto a página e o fluxo de sucesso.

---

## 3. Alternativa: Google Forms (zero código)

Use o roteiro de `docs/22-FEEDBACK-FORM.md` e pronto:

- respostas em uma planilha do Google;
- notificação por e-mail;
- link público ou restrito;
- nenhuma hospedagem de página.

É a opção mais rápida se você não quer editar HTML nem criar conta no Formspree.
A desvantagem é que a página não tem o visual do jogo.

---

## 4. Alternativa: Supabase (banco próprio)

Para quem quer os dados em um banco e não se importa com mais setup:

1. Crie um projeto no Supabase.
2. Crie a tabela `feedback` com colunas iguais ao JSON acima.
3. Ative **Row Level Security** com política **insert-only** para o papel `anon`.
4. Na página, troque o `fetch` para:

```js
fetch('https://SEU_PROJETO.supabase.co/rest/v1/feedback', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'apikey': 'SUA_ANON_KEY',
    'Authorization': 'Bearer SUA_ANON_KEY',
    'Prefer': 'return=minimal'
  },
  body: JSON.stringify(payload)
});
```

5. Leia as respostas no Table Editor ou via SQL.
6. Para e-mail, crie um trigger/Edge Function.

**Cuidados:**

- A `anon key` é pública; use RLS insert-only e nunca permita `select` anônimo.
- Adicione captcha/rate limit se a página ficar pública.
- Para 5–8 testadores, é overkill.

---

## 5. Segurança e privacidade

- **Nunca** coloque service role key, senha de SMTP ou token privado no HTML.
- O endpoint do Formspree é público por design; o honeypot e o filtro deles
  reduzem spam.
- Colete o mínimo: nome é opcional, e-mail não é pedido.
- Avise no formulário que as respostas serão usadas para melhorar o jogo.
- LGPD: guarde só o necessário, apague depois da análise e não publique respostas
  com dados pessoais.
- Se usar Supabase, RLS insert-only e nunca `select` para `anon`.
- Se a página ficar pública, considere Cloudflare Turnstile/captcha.

---

## 6. Problemas comuns

| Sintoma | Causa provável | Solução |
|---|---|---|
| “Endpoint não configurado” | `FEEDBACK_ENDPOINT` vazio | cole o endpoint |
| Erro de CORS no console | endpoint sem CORS | use Formspree ou um backend same-origin |
| HTTP 422 | campo/form inválido | confira o ID e os nomes dos campos |
| HTTP 403 | formulário não confirmado | confirme o e-mail no Formspree |
| Envio OK, sem e-mail | notificação desligada/spam | veja o painel e a caixa de spam |
| Limite atingido | plano free | espere o mês ou use Google Forms |
| Funciona local, não hospedado | caminho/arquivo faltando | publique a pasta inteira `docs/feedback` |

---

## 7. Checklist

- [ ] Conta no Formspree criada e e-mail confirmado.
- [ ] Endpoint colado em `FEEDBACK_ENDPOINT`.
- [ ] Página publicada (GitHub Pages ou Netlify).
- [ ] Teste em aba anônima.
- [ ] Resposta aparece no painel.
- [ ] E-mail de notificação chega.
- [ ] `_gotcha` vazio nas submissões.
- [ ] Consentimento obrigatório funcionando.
- [ ] CSV de triagem pronto.
- [ ] Link enviado aos testadores.
