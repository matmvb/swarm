# Verificação — Pages, feedback e itch

Data: 23/11/2026
Último commit verificado: `1a4d558`

## GitHub Pages

| URL | Status | Observação |
|---|---|---|
| `https://matmvb.github.io/swarm/` | 200 | landing atualizada, sem link quebrado para o itch |
| `https://matmvb.github.io/swarm/feedback/` | 200 | formulário completo, endpoint configurado |
| `https://matmvb.github.io/swarm/feedback` | 200 | redireciona/abre a mesma página |

- O conteúdo servido corresponde ao commit atual.
- `docs/.nojekyll` está no repositório.
- A landing tem o botão **Enviar feedback** e as instruções de teste.
- O link do jogo não fica mais na landing; ele vai no convite do itch.

## Formspree

Endpoint configurado na página:

```text
https://formspree.io/f/xqpqgyzn
```

Testes:

| Teste | Resultado |
|---|---|
| `OPTIONS` com origem `matmvb.github.io` | 200, `access-control-allow-origin: *` |
| `POST` JSON de teste | 200, `{"next":"/thanks","ok":true}` |
| Payload | `consent: true`, `_subject`, `source` e campos do formulário |

Foi criada uma submissão de teste com o nome **TESTE AUTOMATICO**. Apague no painel do Formspree.

> Confira também a caixa de entrada/spam: o Formspree deve enviar um e-mail de notificação. Se não chegar, ative as notificações nas configurações do formulário.

## itch.io

| URL | Status |
|---|---|
| `https://matmvb.itch.io/swarm` | **404** |
| `https://matmvb.itch.io/swarm?password=swarmtest` | **404** |
| `https://matmvb.itch.io/` | 200, perfil sem jogos listados |

O projeto **não está publicado/visível**. Provavelmente ainda está em **Rascunho** ou Restrito sem o link secreto correto.

Para liberar:

1. No itch, abra o projeto SWARM.
2. Em **Visibilidade e acesso**, escolha **Restrito**.
3. Ative a senha e defina uma senha de teste.
4. Salve e copie o link secreto mostrado pelo itch.
5. Abra o link em uma aba anônima e confirme que o download aparece.
6. Só então envie o link aos testadores.

Se o projeto for público, use **Público**. Se estiver em Rascunho, o link público continuará 404 e o download não funciona para visitantes.

## Pendências

- [ ] Publicar o projeto no itch (Restrito + senha ou Público).
- [ ] Testar o link do itch em aba anônima.
- [ ] Apagar a submissão **TESTE AUTOMATICO** no Formspree.
- [ ] Confirmar o e-mail de notificação do Formspree.
- [ ] Enviar o link do feedback para os testadores.
- [ ] Acompanhar as respostas e triar com `docs/feedback/triage.csv`.
