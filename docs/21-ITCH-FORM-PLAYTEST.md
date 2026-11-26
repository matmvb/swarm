# Ficha do itch.io — playtest 0.9.8

Use esta ficha para preencher a tela **Criar um novo projeto** do itch.io.
Ela complementa `docs/20-PLAYTEST-EXTERNO.md`.

---

## 1. Campos do formulário

| Campo | Valor |
|---|---|
| **Título** | `SWARM` |
| **URL do projeto** | `https://matmvb.itch.io/swarm` |
| **Breve descrição ou slogan** | `Bullet heaven neon: sobreviva 10 minutos, escolha upgrades e derrote o Núcleo.` |
| **Classificação** | `Jogos` |
| **Tipo de projeto** | `Para download` |
| **Status de lançamento** | `Em desenvolvimento` |
| **Preços** | `Sem pagamentos` (para o teste) |
| **Carregamentos** | `Builds/SWARM-windows.zip` |
| **Capa** | `docs/press/cover-630x500.png` |
| **Vídeo ou trailer** | deixar vazio por enquanto |
| **Fotos** | 3–5 screenshots (mínimo 1) |
| **Gênero** | `Ação` |
| **Tags** | ver lista abaixo |
| **AI generation disclosure** | `Sim` → marcar `Code` e `Text & Dialog` |
| **App store links** | deixar vazio |
| **Nome personalizado** | deixar vazio |
| **Comunidade** | `Comentários` |
| **Visibilidade e acesso** | `Restrito` (não `Rascunho`) |
| **Senha** | `SUA_SENHA` |

### Por que "Sem pagamentos" no teste

O aviso amarelo no topo aparece porque você ainda não configurou pagamentos.
Com `Sem pagamentos`, o download fica livre e o aviso desaparece. Depois, se quiser
doações, configure uma conta de pagamento e volte para `$0 ou doar`.

### Por que "Restrito" e não "Rascunho"

Em projeto **para download**, o link secreto do rascunho não libera o download.
O modo correto para testadores é **Restrito + senha**. Quem tiver a URL com a senha
consegue baixar sem criar conta no itch.io.

---

## 2. Upload

1. Clique em **Fazer upload de arquivos**.
2. Envie `Builds/SWARM-windows.zip` (39 MB).
3. Marque o arquivo como **Windows**.
4. Deixe desmarcado **"This file will be played in the browser"**.
5. Não envie a pasta `Builds/Windows` solta; envie o ZIP.

O ZIP já contém `LEIA-ME.txt` na raiz com controles, SmartScreen, volume e caminho dos logs.

---

## 3. Capa e screenshots

- Capa: `docs/press/cover-630x500.png` (630×500).
- Screenshots: capture com `F12` no jogo. Os arquivos ficam em:

```
%USERPROFILE%\AppData\LocalLow\Scoty\SWARM\shots
```

Sugestão de 5:

1. Escolha de piloto com a loja de Núcleos.
2. Horda cheia com o jogador no meio.
3. Level up com as opções.
4. Boss com o telegrafo.
5. Tela de vitória ou fim de run.

Se ainda não tiver nenhuma, adicione pelo menos 1 para a página não ficar vazia.
Screenshots são opcionais no itch, mas fazem diferença.

---

## 4. Descrição para colar

Copie o bloco abaixo para o campo **Descrição**. Formate os títulos e listas com a
barra do editor do itch se necessário.

```text
SWARM é um bullet heaven 2D onde você não aperta "atirar" — o sistema atira por você. Sua decisão é para onde ir e qual upgrade escolher.

Cada run dura até 10 minutos. Ao subir de nível, o jogo congela e oferece 3 upgrades. Às 5:00 o Núcleo nasce, limpa a horda e transforma a run em um duelo. Sobreviva até 10:00 para vencer.

O QUE TEM NA DEMO
• 6 inimigos com papéis diferentes: horda, corredor, tanque, divisor, atirador e boss
• Boss com rajada radial + investida e telegrafo legível
• 20 upgrades com pré-requisitos, efeitos duplos e limites
• 3 pilotos: Tanque, Veloz e Atirador
• 2 armas: tiro automático e orbes orbitais
• Meta-progressão: gaste Núcleos em bônus permanentes
• Trilha e efeitos gerados por código, sem assets externos

CONTROLES
WASD / setas — mover
ENTER — jogar
1 / 2 / 3 — escolher piloto ou upgrade
4 / 5 / 6 — comprar melhorias permanentes
O — opções
ESC — pausar / continuar
R — reiniciar
F12 — screenshot
F1 — overlay de debug (opcional)

ISSO É UM PROTÓTIPO
Estou testando ritmo, clareza e sensação de poder. Se algo parecer injusto, confuso ou repetitivo, escreva nos comentários — é exatamente esse retorno que estou buscando.

PROBLEMAS CONHECIDOS
• O Windows pode avisar que o aplicativo não é reconhecido. Clique em "Mais informações" > "Executar assim mesmo".
• Se o jogo abrir mudo, aperte O e depois + para aumentar o volume.

Demo 0.9.8 — Windows. Extraia o ZIP inteiro e execute SWARM.exe.

Transparência: código e textos foram produzidos com assistência de LLM; arte, música e efeitos são procedurais/gerados por código.
```

---

## 5. Gênero e tags

**Gênero:** `Ação`. Se não existir, use `Arcade`.

**Tags** (máximo 10, copie uma por uma):

```text
survivors-like
bullet-heaven
roguelite
arcade
2d
neon
singleplayer
short-session
shooter
auto-shooter
```

---

## 6. AI disclosure

O itch.io pede para marcar se o projeto contém material produzido por IA generativa.
A ficha oferece categorias como **Graphics**, **Sound**, **Text & Dialog** e **Code**.

Recomendação para este projeto:

- **Sim**, o projeto contém código e textos produzidos com assistência de LLM.
- Marcar **Code**.
- Marcar **Text & Dialog** (textos de interface e descrição).
- **Não** marcar **Graphics**: sprites, capa e ícone são procedurais, gerados por script próprio.
- **Não** marcar **Sound**: música e SFX são sintetizados por código, sem modelo generativo.

A descrição já inclui uma linha de transparência. Se você preferir não marcar,
entenda que a política do itch lista **Code** como categoria e que a divulgação
incorreta pode afetar a indexação da página.

---

## 7. Visibilidade e senha

1. Em **Visibilidade e acesso**, selecione **Restrito**.
2. Role a página até as opções de acesso restrito.
3. Ative **"Também permitir uma senha para ver a página"** (o texto pode variar).
4. Defina a senha `SUA_SENHA`.
5. Salve.
6. Copie a URL secreta que o itch mostrar (ela pode já incluir `?password=SUA_SENHA`).
7. Abra essa URL em uma **janela anônima** e confirme que:
   - a página abre;
   - o botão de download aparece;
   - o ZIP baixa e extrai;
   - o `SWARM.exe` abre.

> Se a opção de senha não aparecer, procure **"Restricted access settings"** no fim
> da página de edição. Sem senha, o modo Restrito pode exigir download key e conta.

---

## 8. Depois de salvar

1. Copie a URL restrita (com senha).
2. Teste deslogado ou em aba anônima.
3. Envie a mensagem de convite de `docs/20-PLAYTEST-EXTERNO.md`.
4. Use o Google Forms com as 10 perguntas de lá.
5. Não compartilhe a URL em redes públicas.
6. Quando a rodada acabar, troque a senha ou volte para **Rascunho**.

---

## 9. Erros comuns

| Erro | Consequência |
|---|---|
| Deixar em **Rascunho** | testador vê a página, mas não baixa o ZIP |
| Marcar **Público** antes da hora | página aparece em buscas e no perfil |
| Enviar a pasta em vez do ZIP | faltam DLLs e o jogo não abre |
| Não marcar o arquivo como **Windows** | itch não mostra a plataforma correta |
| Esquecer a senha | testador não acessa a página restrita |
| Não testar em aba anônima | você pode estar vendo por estar logado |
| Deixar `$0 ou doar` sem pagamento | o botão de doação pode falhar |
