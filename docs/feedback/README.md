# Feedback — SWARM

Esta pasta contém a página estática de feedback do playtest.

- `index.html` — formulário visual. Por padrão funciona no modo local: o testador preenche, copia ou baixa o texto e envia.
- Com um endpoint configurado, o botão **Enviar feedback** grava a resposta no painel do serviço (Formspree).
- `triage.csv` — cabeçalho da planilha de triagem.

Guia completo: `docs/22-FEEDBACK-FORM.md`.
Backend e envio: `docs/23-FEEDBACK-BACKEND.md`.

## Ativar envio automático

1. Crie um formulário no Formspree.
2. Copie o endpoint (`https://formspree.io/f/...`).
3. Em `index.html`, procure:

```js
const FEEDBACK_ENDPOINT = '';
```

4. Cole o endpoint entre as aspas.
5. Publique e teste em aba anônima.

## Publicar

**GitHub Pages:** Settings → Pages → branch `main` → pasta `/docs`.
URL: `https://matmvb.github.io/swarm/feedback/`

**Netlify Drop:** arraste esta pasta em `app.netlify.com/drop`.

**Local:** abra `index.html` no navegador.

A página não usa cookies nem analytics. No modo local, nada é enviado até o
testador copiar ou baixar o texto.
