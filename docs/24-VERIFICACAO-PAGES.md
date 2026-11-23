# Verificação — Pages, feedback e itch

Data: 23/11/2026
Último commit verificado: `df18970`

## GitHub Pages

| URL | Status | Observação |
|---|---|---|
| `https://matmvb.github.io/swarm/` | 200 | landing atualizada, sem link quebrado para o itch |
| `https://matmvb.github.io/swarm/feedback/` | 200 | formulário completo, endpoint configurado |
| `https://matmvb.github.io/swarm/feedback` | 200 | abre a mesma página |

- O conteúdo servido corresponde ao commit atual.
- `docs/.nojekyll` está no repositório.
- A landing tem o botão **Enviar feedback** e as instruções de teste.
- O link do jogo não fica na landing; ele vai no convite do itch.
- Testado em desktop e em largura de 504 px (o headless do Chrome não desce abaixo disso).

## Formspree

Endpoint configurado na página:

```text
https://formspree.io/f/xqpqgyzn
```

Testes:

| Teste | Resultado |
|---|---|
| `OPTIONS` com origem `matmvb.github.io` | 200, `access-control-allow-origin: *` |
| `POST` JSON de teste 1 | 200, `{"next":"/thanks","ok":true}` |
| `POST` JSON de teste 2 | 200, `{"next":"/thanks","ok":true}` |

Foram criadas duas submissões de teste:

```text
TESTE AUTOMATICO
TESTE AUTOMATICO 2
```

Apague as duas no painel do Formspree.

> Confira a caixa de entrada/spam: o Formspree deve enviar e-mail de notificação.
> Se não chegar, ative as notificações nas configurações do formulário.

## itch.io

| URL | Status |
|---|---|
| `https://matmvb.itch.io/swarm` | 200, mas mostra a tela de senha |
| `https://matmvb.itch.io/swarm?password=swarmtest` | 200, página real |

O projeto está **publicado no modo Restrito** e funcionando com a senha `swarmtest`.

Conteúdo confirmado na página com senha:

- título **SWARM**;
- botão **Download**;
- arquivo **SWARM-windows.zip**, 38 MB;
- descrição completa e controles;
- gênero **Action / Shooter**;
- tags (2D, Arcade, auto-shooter, bullet-heaven, neon, roguelite, short-session, singleplayer, survivor-like);
- AI Disclosure: **AI Assisted / Code / Text**;
- uma imagem de capa/screenshot;
- atualizado em 23/11/2026.

### Link para enviar aos testadores

```text
https://matmvb.itch.io/swarm?password=swarmtest
```

O link sem `?password=swarmtest` mostra a tela de senha. O link com a senha abre direto.

## Pendências

- [x] Publicar o projeto no itch (Restrito + senha).
- [x] Testar o link do itch com senha.
- [ ] Apagar as submissões **TESTE AUTOMATICO** e **TESTE AUTOMATICO 2** no Formspree.
- [ ] Confirmar o e-mail de notificação do Formspree.
- [ ] Enviar o link do feedback para os testadores.
- [ ] Acompanhar as respostas e triar com `docs/feedback/triage.csv`.
