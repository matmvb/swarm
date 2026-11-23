# Auditoria da página do GitHub — 23/11/2026

Commit da auditoria: `ad80b1f`

## Escopo

- Landing do GitHub Pages (`docs/index.html`)
- Formulário de feedback (`docs/feedback/index.html`)
- Política de privacidade, 404, SEO e compartilhamento
- README do repositório
- Verificação das URLs publicadas

## Antes → depois

| Área | Antes | Depois |
|---|---|---|
| Título/descrição | título curto, sem description | título descritivo + description + canonical |
| Compartilhamento | sem Open Graph/Twitter Card | OG e Twitter Card com imagem 1200×630 |
| Busca | sem JSON-LD | `VideoGame` com gênero, plataforma e oferta |
| Favicon | ausente | favicon 32 e apple-touch 180 |
| Landing | pouca informação e CTA único | hero com capa, features, passos, requisitos, imprensa e rodapé |
| Mobile | nav escondida | marca + botão Feedback fixos no topo |
| Acessibilidade | básica | skip link, foco visível, `prefers-reduced-motion`, alt e labels |
| Feedback | sem noindex e sem privacidade | `noindex`, consentimento, link de privacidade e painel de sucesso |
| Privacidade | ausente | página completa em `docs/privacidade.html` |
| Erros | 404 padrão do GitHub | 404 customizado com links |
| Indexação | sem robots/sitemap | `robots.txt` + `sitemap.xml` |
| README | descrevia um protótipo antigo | demo 0.9.8, links, features, stack, build e changelog |

## Verificação ao vivo

| URL | Status |
|---|---|
| `https://matmvb.github.io/swarm/` | 200 |
| `https://matmvb.github.io/swarm/feedback/` | 200 |
| `https://matmvb.github.io/swarm/privacidade.html` | 200 |
| `https://matmvb.github.io/swarm/nao-existe` | 404 com página customizada |
| `https://matmvb.github.io/swarm/robots.txt` | 200 |
| `https://matmvb.github.io/swarm/sitemap.xml` | 200 |
| `https://matmvb.github.io/swarm/press/og-1200x630.png` | 200 |
| `https://matmvb.github.io/swarm/press/favicon-32.png` | 200 |

Também conferido:

- metatags de SEO/OG/Twitter e JSON-LD válido;
- `noindex,nofollow` no feedback;
- IDs, labels e caminhos locais sem duplicatas ou links quebrados;
- fluxo de envio local e com endpoint;
- painel de sucesso e botão “Enviar outro feedback”;
- formulário enviando com `consent: true`;
- itch.io acessível com senha e download de 38 MB.

## Ajustes manuais no GitHub

Não dá para configurar isso por arquivo; faça em **Settings** do repositório:

1. **About** → description:
   `Bullet heaven 2D em Unity 6.6 — demo 0.9.8 em playtest.`
2. **Website**:
   `https://matmvb.github.io/swarm/`
3. **Topics**:
   `unity`, `game`, `bullet-heaven`, `survivors-like`, `roguelite`, `2d`, `indie`, `csharp`, `urp`, `windows`.
4. **Social preview** → suba `docs/press/og-1200x630.png`.

## Pendências de conteúdo

- [ ] 5 screenshots de gameplay (a página do itch tem só a capa).
- [ ] 3 GIFs de 6–10 s.
- [ ] Trailer de 30 s.
- [ ] Teste em outra máquina.
- [ ] Apagar as três submissões de teste no Formspree.

## Observações

- O link do itch na landing é o link público (mostra a senha). A senha **não** foi publicada na página.
- O feedback é público, mas `noindex` e com honeypot.
- A política de privacidade cobre GitHub Pages, Formspree, retenção de 90 dias e direitos LGPD.
