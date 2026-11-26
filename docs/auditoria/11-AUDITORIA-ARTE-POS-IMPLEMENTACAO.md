# SWARM 0.9.9 — Auditoria artística pós-implementação

**Projeto:** SWARM  
**Versão:** 0.9.9  
**Commit de referência:** `b75100e`  
**Data:** 26/11/2026  
**Escopo:** auditoria técnica-artística dos assets, animações, combate, boss, cenário e interface após as correções da 0.9.9.  
**Modo:** a auditoria foi realizada em somente leitura; este documento registra os achados. Nenhum asset, cena, código ou configuração foi alterado durante a análise.

**Legenda de evidência:**  
`[A]` encontrado nos arquivos · `[V]` observado em captura · `[I]` inferido de código/cena.

---

## Método e limitações

- Inspeção dos **73 PNGs** de `Assets/Swarm/Art/Sprites`, do `manifest.json`, dos dados de inimigos/pilotos/upgrades, da cena, dos scripts de apresentação, do material, da fonte e do perfil de pós-processamento.
- Geração de contatos visuais dos sprites, animações, ícones e sheets do boss; medição de cores, alfa, bounding boxes e diferenças entre frames.
- A build 0.9.9 foi aberta com as flags de QA e foram observados **menu, seleção de piloto, gameplay e level up**.
- As capturas de janela saíram em **1536×864**; por DPI, parte da borda direita do layout 1920 não aparece. O HUD direito foi conferido na cena, não apenas na imagem.
- **O boss não foi capturado ao vivo**: ele spawna aos 300 s e a run automática sem movimento provavelmente termina antes. A avaliação do Núcleo vem dos sheets, da cena e do código.
- Não houve stress de 300 inimigos nem Profiler. As conclusões de densidade são qualitativas e de código.

---

## A. Diagnóstico da versão 0.9.9

### Nota geral: **7,5/10**

A 0.9.9 deu um salto real. A identidade “pixel art espacial neon” está consolidada, os inimigos têm paleta e silhuetas coerentes, os cards e a fonte pixel funcionam, e o boss tem sheets próprios. O problema agora não é falta de direção: é **execução de runtime e acabamento**.

| Camada | Nota | Diagnóstico |
|---|---:|---|
| Sprites base | 8,5 | Inimigos e boss têm silhueta, paleta e leitura boas. |
| Animações | 6,0 | O sistema existe, mas boa parte dos idles é estática e os `move` estão sem uso. |
| Combate/FX | 5,5 | FX integrados, porém brancos demais, com alpha parcial e escalas que cobrem a arte. |
| Boss | 7,0 | Arte boa; entrada, telegraphs e morte ainda não fazem o clímax. |
| Cenário | 5,5 | Grid funcional, mas grosso, suave e sem profundidade. |
| Interface | 7,5 | Cards fortes; faltam pixel-perfect, barra de XP e molduras. |
| Consistência | 6,5 | Inimigos seguem a paleta; jogador, projéteis, logo e UI fogem dela. |

**Tese central:** o conteúdo autoral já existe. O que separa a 0.9.9 de uma 1.0 profissional é **fazer a arte aparecer** — animações, cores de time, sequência de morte, entrada do boss e UI pixel-perfect.

---

## B. Pontos fortes

1. **Paleta dos inimigos é exemplar.**  
   Esporo `#FF4D8D`, Dardo `#FF8C2E`, Casco `#BF33FF`, Célula `#33FFB3`, Olho `#59BFFF` e Núcleo `#FF2626` usam a paleta oficial com poucos highlights. Isso deve ser preservado.

2. **Silhuetas cumprem função.**  
   Drone é anel/olho pequeno; Dardo é seta; Tanque é placa; Divisor é célula; Atirador é losango/olho; Núcleo é anel quebrado. A diferenciação funciona mesmo sem cor.

3. **Boss tem arte própria de verdade.**  
   Idle, telegraph, attack, hit e death existem e são animados. O sheet de morte em 8 frames e o telegraph de 4 frames são o melhor material do pacote.

4. **Dardo e Olho têm comportamento visual correto.**  
   O Dardo gira para a direção do movimento e o Olho troca para `enemy_olho_charge`. São os dois inimigos com leitura de intenção mais clara.

5. **Level up e seleção de piloto estão muito melhores.**  
   Cards com ícone, categoria, tecla e descrição; 20 ícones ligados; fonte pixel; hierarquia clara. O level up deixou de ser parede de texto.

6. **A paleta semântica da UI funciona.**  
   Ciano para ofensivo, dourado para defesa, âmbar para utilidade; perigo vermelho; XP dourado. A categoria é legível.

7. **O conceito de fundo em duas camadas é bom.**  
   Parallax, grid distante e tint vermelho no duelo são decisões corretas. O problema é a execução técnica do grid, não a ideia.

8. **O Bloom foi contido.**  
   `0.48 / 0.82 / 0.52` é muito mais adequado que o perfil anterior. O caminho é esse; o problema é que os próprios sprites já são brancos demais.

9. **Build e validação estão estáveis.**  
   Build 0.9.9 OK, ~105,2 MB, validação de cena 0 problemas e smoke test sem exceptions.

---

## C. Problemas encontrados

### C1. CRÍTICO — quatro PNGs de combate são inválidos como PNG

`[A]` Os arquivos abaixo têm **bytes de filtro de scanline inválidos** (`127` ou `153`; o padrão PNG aceita apenas `0–4`):

- `Assets/Swarm/Art/Sprites/Projectiles/proj_player.png`
- `Assets/Swarm/Art/Sprites/Projectiles/proj_enemy.png`
- `Assets/Swarm/Art/Sprites/Projectiles/proj_boss.png`
- `Assets/Swarm/Art/Sprites/Projectiles/xp_orb.png`

Decodificadores padrão rejeitam esses arquivos. A Unity importa e renderiza, mas o arquivo não é confiável: reimportação, compressão, atlas ou ferramenta externa podem falhar ou produzir outra imagem. Pior: são exatamente os projéteis e o XP, elementos centrais.

**Impacto:** crítico para pipeline e potencialmente para a aparência final.  
**Ação:** reexportar os quatro sheets com encoder PNG padrão, sem filtros inválidos, e adicionar validação de integridade ao `ArtImportPipeline`.

---

### C2. CRÍTICO — a linguagem de cor do combate é branca demais

`[A]` Medição do primeiro frame:

| Asset | Pixels quase brancos | Pixels com alpha parcial |
|---|---:|---:|
| `proj_player` | 59,6% | 43 |
| `proj_enemy` | 57,4% | 43 |
| `proj_boss` | 63,9% | 100 |
| `xp_orb` | 55,3% | 87 |
| `orbital` | 85,2% | 172 |
| `fx_explosion` | 70,3% | 1.144 |

Os projéteis de jogador, inimigo e boss têm **a mesma leitura branca**; a diferença é quase só tamanho. O `EnemyProjectile` não aplica tint de time, e `DeathBurst.SpawnExplosion` **ignora a cor do inimigo** e usa branco. Isso contraria o brief:

> “Brilho vem do Bloom, não de pixels semitransparentes.”  
> “Nada de antialias, gradiente ou ruído de 1 px.”

**Impacto:** alto em leitura de combate e identidade.  
**Ação:** reexportar/tintar por time:
- jogador: ciano + núcleo branco;
- inimigo comum: magenta/laranja + centro escuro;
- boss: vermelho/âmbar + núcleo branco;
- explosão: cor do inimigo no miolo, branco só na borda;
- reduzir alpha parcial a 0/255 nos projéteis.

---

### C3. ALTO — o boss pode atacar antes de aparecer na tela

`[I]` Em `EnemySpawner.SpawnBoss`, o Núcleo nasce a `spawnRadius + 2 = 15` unidades. A câmera tem `orthographicSize = 6`; em 16:9, a meia-largura é ~10,67 e a meia-diagonal ~12,24. O boss anda a `moveSpeed = 1`, mas o primeiro burst acontece em `firstBurstDelay = 2.5 s`.

Dependendo da direção do spawn, o Núcleo ainda está **fora da tela** quando dispara a primeira rajada. O zoom de entrada (`ZoomPunch(0.7)`) reduz ainda mais o campo visível. O jogador vê a barra do boss aparecer, a horda sumir e projéteis virem de fora sem ver o inimigo.

Além disso, não há aviso de entrada do boss; `SpawnWarning` só é usado para inimigos comuns.

**Impacto:** alto — quebra o clímax e a justiça percebida.  
**Ação:** spawnar mais perto (9–10,5), ou fazer uma entrada rápida/invulnerável; atrasar o primeiro ataque até o boss estar visível; adicionar um aviso de borda de tela.

---

### C4. ALTO — a explosão de morte cobre a animação de morte e o shake é constante

`[A/I]` Em `Enemy.Die`:

```csharp
float explosionScale = isBoss ? 3.4f : Mathf.Clamp(transform.localScale.x * 1.8f, 0.8f, 2.2f);
```

Escalas em tela a 1080p:

| Inimigo | Sprite na tela | Explosão |
|---|---:|---:|
| Drone | ~38 px | mínimo ~72 px |
| Tanque | ~85 px | ~154 px |
| Boss | ~198 px | ~306 px |

A explosão é criada **no início** da morte e dura `0.4 s`; as sheets de morte duram `0,33–0,67 s`. Ou seja: a explosão branca cobre justamente a animação autoral que foi produzida. O `SpawnExplosion` também ignora a cor do inimigo.

Além disso, **toda morte comum** chama `CameraFollow.Shake(0.03f, 0.08f)`. Com dezenas de mortes por segundo, a câmera nunca assenta; isso piora o shimmer do pixel art e a leitura.

**Impacto:** alto — o trabalho de death sheet não aparece e o combate fica instável.  
**Ação:** morte em três tempos: hit spark → death sheet → explosão no fim/atrás; escalar FX por tier; remover shake de Drone/Corredor; reservar shake/hitstop para Tanque/Divisor/Boss.

---

### C5. ALTO — a horda é estática na maior parte do tempo

`[A]` Comparação pixel a pixel:

| Sheet | Frames | Diferença entre frames |
|---|---:|---:|
| `enemy_esporo_idle` | 2 | **0** |
| `enemy_dardo_idle` | 2 | **0** |
| `enemy_casco_idle` | 2 | **0** |
| `enemy_celula_idle` | 2 | **0** |
| `enemy_olho_idle` | 2 | **0** |
| `enemy_fragmento_idle` | 2 | **0** |
| `enemy_esporo_move` | 2 | 72 |
| `enemy_dardo_move` | 4 | 100 |
| `enemy_nucleo_idle` | 4 | 236/191/344 |

O sistema `SpriteAnimator` funciona, mas os idles de quase todos os inimigos são **frames duplicados**. Os únicos `move` que realmente têm animação — Esporo e Dardo — **nunca são tocados**: `Enemy.ConfigureIdle()` usa apenas `idleFrames`, e `moveFrames` não aparece em nenhum consumidor.

**Impacto:** alto — a horda desliza como tokens, não como criaturas.  
**Ação:** usar `moveFrames` quando houver velocidade; criar pulso/bob de 1 px para Tanque/Divisor/Olho; reexportar os idles duplicados.

---

### C6. ALTO — o jogador não tem hit/death visuais e os pilotos não têm acento

`[A]` `CharacterData.hitFrames` e `deathFrames` são preenchidos, mas `PlayerVisual.SetShip()` só configura `idleFrames`. `ship_*_hit` e `ship_*_death` estão no projeto e **não são usados**. Na morte, `Health.Died` chama `GameManager.EndRun` imediatamente; não há nave explodindo, sheet de morte nem pausa dramática.

Além disso, o brief pede **ciano + acento do piloto** (laranja/verde/amarelo), mas:

- Baluarte tem 48 px de laranja — funciona.
- Vetor tem 20 px de `#A6FFDA`, que **não é** o verde `#33FFB3`; não lê como acento.
- Lança tem apenas 8 px de amarelo no motor; cockpit/canhão não são amarelos.
- O corpo dominante das três naves é `#94FFEE` (326/164/104 px), um ciano pálido **fora da paleta oficial**; o ciano oficial `#4DFFE0` tem menos presença.
- `CharacterData.color` (usado na borda dos cards) não é aplicado à nave; o card de Veloz é verde, mas a nave é ciano.

**Impacto:** alto — o jogador fica pálido e próximo do branco dos FX; os pilotos não se diferenciam em jogo.  
**Ação:** corpo em `#4DFFE0`, highlight `#A6FFF2`; acento do piloto no cockpit/motor; usar o mesmo acento no card e no thruster; adicionar ground ring/outline ciano sutil.

---

### C7. ALTO — o grid ainda é o elemento mais “provisório” da tela

`[A/V]` `SwarmArt.Grid()` cria um tile 128×128 com linha de 1 px a cada 32 px, **PPU 8** e `FilterMode.Bilinear`.

- 1 px de arte em PPU 8 = `0,125` unidades.
- A 1080p (~90 px/u), a linha tem ~**11 px** de espessura teórica.
- O filtro bilinear suaviza a borda.
- O resultado visual é um grid grosso, macio e dominante, que compete com projéteis e com a nave.

O grid é o único elemento grande do gameplay que não é pixel art 1×. O conceito de duas camadas/parallax é bom; a execução técnica não.

**Impacto:** alto — o cenário parece protótipo e reduz a leitura.  
**Ação:** regenerar o tile com linha de 1 px em PPU 64/128, `Point`, alpha 5–10%; manter duas camadas e o tint do boss.

---

### C8. ALTO — a UI não é pixel-perfect

`[A/V]` A fonte e os ícones são pixel art, mas a apresentação usa escalas fracionárias:

| Elemento | Origem | Exibição | Escala |
|---|---:|---:|---:|
| `ui_pilot_*` | 64×64 | 96×96 | **1,5×** |
| `ui_core` | 16×16 | 36×36 | **2,25×** |
| `ui_boss` | 24×24 | 30×30 | **1,25×** |
| `ui_logo_swarm` | 355×127 | 600×212 | **1,69×** |
| `ui_upgrade_*` | 24×24 | 96×96 | 4× (correto) |

Além disso:

- `PressStart2P-Regular.ttf.meta` usa `fontRenderingMode: 0` (**Smooth**), não `HintedRaster`.
- Tamanhos da Press Start 2P incluem 28, 22, 46, 60, 30, 18 e 20 — a maioria **não é múltipla de 8** (a base da fonte).
- `CanvasScaler.matchWidthOrHeight = 0.5` gera escala 0,666 em 720p e 1,333 em 1440p.
- `SpriteUnlit.mat` tem `PixelSnap: 0`.
- `CameraFollow.pixelSnap = 40`, mas a arte tem ~2,25 px de tela por pixel de arte em 1080p; o snap não resolve o shimmer.

**Impacto:** alto no acabamento — a UI parece “quase pixel”, mas com bordas irregulares e ícones borrados.  
**Ação:** `HintedRaster`; tamanhos 16/24/32/40/48; escalas inteiras para ícones/pilotos/logo; `Canvas.pixelPerfect`; considerar Pixel Perfect Camera/render target interno.

---

### C9. MÉDIO/ALTO — a barra de XP é invisível quando está vazia e o HUD não tem moldura

`[A/V]` `MakeBar` cria fundo preto com alpha 0.55 e fill dourado, sem borda, label ou segmentos. A barra de XP tem **1200×10 px** na base. Com XP 0, o fundo quase desaparece no grid/fundo; o jogador não vê que existe uma progressão. A 720p, ela fica com ~6,7 px de altura.

O HUD também flutua diretamente sobre o grid, sem uma faixa/placa de contraste. Em hordas densas, os textos e barras competem com inimigos e FX.

**Impacto:** médio/alto — o jogador perde o feedback de XP e a HUD parece improvisada.  
**Ação:** barra 16–20 px com moldura 1 px, label “XP” e ticks; faixa escura/gradiente atrás do HUD; mesmo tratamento para HP e boss bar.

---

### C10. MÉDIO — nomes de pilotos colidem com nomes de inimigos e o Divisor não usa o fragmento

`[A]` Os pilotos são `TANQUE`, `VELOZ` e `ATIRADOR`. Os inimigos incluem `Tanque` e `Atirador`. O jogador vê “ATIRADOR” na seleção e “Atirador” como inimigo azul; vê “TANQUE” como piloto e “Tanque” como inimigo roxo. Isso prejudica a identidade e a comunicação.

O `Divisor` deveria liberar fragmentos, mas `SplitOnDeath` usa `enemyTemplates[0]` — o Drone — e `enemy_fragmento_idle.png` está **sem uso**. O brief previa fragmentos 12×12.

**Impacto:** médio — confusão de nomes e uma promessa visual não cumprida.  
**Ação:** renomear pilotos para **BALUARTE / VETOR / LANÇA** (ou callsigns); criar template de fragmento com `enemy_fragmento_idle`.

---

### C11. MÉDIO — a atmosfera de menu/seleção/level up é um vazio preto

`[V]` O menu principal tem um bom wordmark, mas o fundo é preto absoluto. A seleção e o level up também são preto absoluto com cards. Funciona, mas não parece o mesmo universo do gameplay.

O logo é um recorte da capa com **330 cores** e **14.932 pixels de alpha parcial**; exibido a 1,69×, ele é o elemento menos “pixel 1×” da UI. Não é ruim, mas está fora do padrão.

**Impacto:** médio — a primeira impressão é limpa, porém chapada.  
**Ação:** starfield/grid muito sutil atrás dos menus; scrim para legibilidade; logo pixel real em 1×/2×; molduras/cantos técnicos nos painéis.

---

### C12. MÉDIO — dívida de produção: não há fontes editáveis

`[A]` `docs/art/source/` contém apenas `LEIA-ME.txt`; não há `.piskel`, `.aseprite` ou fontes equivalentes dos 73 PNGs. Corrigir os quatro PNGs inválidos, os idles duplicados ou os acentos das naves exigirá redesenho ou engenharia reversa.

**Impacto:** médio/alto para a 1.0 — qualquer refinamento futuro fica caro e arriscado.  
**Ação:** versionar as fontes editáveis e adicionar validação de PNG (filtros, zlib, dimensões, alpha) ao pipeline.

---

## D. Oportunidades de polimento

### D1. Sprites e identidade

- **Corpo do jogador em ciano saturado** (`#4DFFE0`), highlight `#A6FFF2`, acento do piloto no cockpit/motor.
- **Vetor:** verde `#33FFB3` no cockpit/engine, não `#A6FFDA`.
- **Lança:** amarelo `#FFE066` no cockpit/canhão, não só 8 px no motor.
- **Ground ring** ciano de baixa alpha sob o jogador; usar `fx_spawn`/ring pixel, não o anel procedural AA.
- **Boss:** reforçar o núcleo/olho branco e os shards; o anel ainda lê como “inimigo grande”.
- **Fragmento:** usar `enemy_fragmento_idle` no `SplitOnDeath`.

### D2. Animações

- Tocar `moveFrames` quando a velocidade for relevante; `idle` quando parado.
- Corrigir os idles duplicados: olho do Esporo, vents do Casco, lóbulos da Célula, piscada do Olho.
- Dardo: usar os 4 frames de `move`; a rotação já está correta.
- Tanque/Divisor/Olho: bob/pulso de 1 px por código ou shader, sem novos frames.
- Jogador: `hit` por 0,08 s e `death` antes do game over.
- Boss: telegraph e attack com frames distintos por ataque; a morte deve ser a estrela.

### D3. Combate/FX

- Tint por time em projéteis e explosões.
- Separar XP de crítico: hoje `CriticalColor` é `(1, .78, .28)` e o XP é `(1, .84, .30)` — quase iguais.
- Hit spark menor para Drone/Corredor; explosão média para Tanque/Divisor; boss à parte.
- Remover shake de mortes comuns; usar hitstop só em eventos pesados.
- Adicionar um pequeno sparkle de coleta no XP, sem partículas brancas.
- Limitar FX simultâneos por tipo; overdraw é o risco.

### D4. Boss

- Entrada: aviso de borda, zoom, grid pulse, boss entrando de fato; primeiro ataque só depois de visível.
- Telegraphs: rajada = anel expandindo laranja; investida = linha/chevron vermelho na direção.
- Arena: grid vermelho pulsando, vinheta mais quente, limite circular sutil.
- Morte: death sheet primeiro; explosão no fim; hitstop no impacto; core drop visível voando para o HUD.
- Barra: pips de fase, nome, ícone fora da barra.

### D5. Cenário

- Grid 1 px/Point, duas camadas, alpha 5–10%.
- Starfield 1 px em 3 profundidades; parallax 0,15/0,4/0,7.
- Nebulosa dithered de 3–4 cores, alpha muito baixo; nada de gradiente suave.
- Poeira/partículas ambientais lentas; nenhuma estrela brilhante perto de projéteis.
- Arena do boss com pulso vermelho e limite; sem encher a tela.

### D6. UI

- `HintedRaster` + tamanhos 8n; `Canvas.pixelPerfect`.
- Escalas inteiras: piloto 64→128, core 16→32/48, boss 24→24/48, logo 355→710.
- Barra de XP 16–20 px, label e moldura; HP/boss com ticks e 1 px frame.
- Faixa escura/gradiente atrás do HUD.
- Painéis com moldura 1 px/cantos técnicos; grid/scanline 3–5% atrás de menu/seleção/level up.
- Card de piloto com acento do piloto no retrato, não só na borda.
- Corrigir ícone de Perfuração (flecha atravessando escudo), unificar line/fill.
- Game over com cards/ícones, não só texto.

### D7. Nomes e comunicação

- BALUARTE / VETOR / LANÇA; remover colisão com Tanque/Atirador.
- “KILLS” vs “NÍVEL”: decidir se o HUD é inglês arcade ou português; hoje mistura.
- Tagline “SURVIVE 10 MINUTES” em inglês; aceitável como marca, mas decidir.

---

## E. Conceitos de evolução visual

### E1. “Ciano é o jogador; acento é o piloto”

O corpo da nave volta ao ciano saturado; o acento do piloto aparece no cockpit, engine e thruster. Baluarte laranja, Vetor verde, Lança amarelo. O card do piloto usa o mesmo acento. Em horda, o jogador é o único bloco ciano sólido; o resto são polígonos quentes.

### E2. “Cada time tem uma cor; branco é impacto”

Projétil do jogador: ciano com núcleo branco. Inimigo: magenta/laranja com centro escuro. Boss: vermelho/âmbar. XP: verde ou dourado com forma distinta. Explosão: cor do inimigo no miolo, branco na borda. O branco deixa de ser a cor padrão e vira evento.

### E3. “Morte em três tempos”

1. Hit spark curto e pequeno.
2. Death sheet visível, com 1 px de movimento e partículas coloridas.
3. Explosão curta no fim, atrás/da cor do inimigo.

Drone não treme a câmera; Tanque treme pouco; boss treme muito. O jogador vê a morte que a arte desenhou.

### E4. “O Núcleo entra como chefe”

Aviso na borda, zoom, grid pulsando, barra enchendo, boss entrando. Rajada e investida têm cores/formas de telegraph diferentes. A morte quebra o anel, implode o núcleo, dá hitstop no impacto e solta o core visível.

### E5. “Três profundidades, nenhum protagonismo”

Grid fino, estrelas 1 px, nebulosa dithered, poeira. Tudo abaixo de 10% de contraste. O fundo dá profundidade, mas o olho continua nos projéteis e no jogador.

### E6. “UI de instrumento, não de formulário”

Barras com moldura/ticks, labels curtas, painéis com cantos técnicos, fonte Press Start 2P em tamanhos inteiros, ícones em escalas inteiras. A UI parece um HUD de nave, não uma tela de debug.

---

## F. Priorização

| # | Melhoria | Impacto visual | Importância p/ jogo | Complexidade | Risco de descaracterizar |
|---|---|---|---:|---|---|
| 1 | Corrigir os 4 PNGs inválidos + validação | 5 | 5 | Baixa | Baixo |
| 2 | Cor/tint de projéteis, XP e explosões | 5 | 5 | Média | Baixo |
| 3 | Rebalancear morte, hit FX e camera shake | 5 | 4 | Média | Baixo |
| 4 | Animar horda + hit/death do jogador | 5 | 3 | Média | Baixo |
| 5 | Entrada/telegraphs/morte do Núcleo | 5 | 4 | Média/Alta | Médio |
| 6 | Identidade do jogador (ciano + acento + XP/crit) | 5 | 4 | Baixa/Média | Baixo |
| 7 | Grid + starfield/parallax/arena | 5 | 3 | Média | Baixo/Médio |
| 8 | Pixel-perfect da UI (fonte, escalas, Canvas) | 4 | 3 | Média | Baixo |
| 9 | HUD/XP bar/molduras/menus | 4 | 4 | Baixa | Baixo |
| 10 | Nomes, fragmentos, ícones | 3 | 3 | Baixa | Baixo |
| 11 | Fontes editáveis/pipeline | 3 | 2 | Baixa | Baixo |
| 12 | Game over/opções com cards | 3 | 2 | Baixa/Média | Baixo |

---

## G. Plano de evolução para 1.0

### Fase 0 — Integridade (1–2 dias)
- Reexportar `proj_player`, `proj_enemy`, `proj_boss`, `xp_orb`.
- Adicionar validação de PNG ao pipeline.
- Versionar fontes editáveis em `docs/art/source`.
- **Critério:** todos os PNGs abrem em decodificador padrão; build sem assets corrompidos.

### Fase 1 — Leitura de combate (3–4 dias)
- Tint de time em projéteis/explosões.
- Separar XP de crítico.
- Escalas de hit/explosion por tier.
- Remover shake de mortes comuns.
- Sequência hit → death sheet → explosão.
- **Critério:** em 30–50 inimigos, dá para identificar jogador, inimigo, boss e XP sem HUD.

### Fase 2 — Animação e identidade (3–4 dias)
- `moveFrames` em uso; idles duplicados corrigidos.
- Hit/death do jogador.
- Acentos dos pilotos e corpo ciano.
- Fragmento do Divisor; renomear pilotos.
- **Critério:** a horda se move, o jogador tem morte e os três pilotos são reconhecíveis.

### Fase 3 — Núcleo (3 dias)
- Entrada visível/aviso; primeiro ataque só depois de aparecer.
- Telegraph distinto para rajada/investida.
- Arena e morte com hitstop/explosão/core drop.
- **Critério:** o clímax é o momento mais forte da run, não o mais confuso.

### Fase 4 — Cenário e UI (4–5 dias)
- Grid Point/fino; starfield/parallax/nebulosa.
- Fonte HintedRaster/tamanhos 8n; escalas inteiras.
- XP bar/moldura/faixa de HUD; painéis e menus.
- **Critério:** 720p/1080p/1440p/4K com UI nítida e fundo sem competir.

### Fase 5 — QA visual (2–3 dias)
- 300 inimigos, boss, 30/60/120 s.
- Colorblind/grayscale.
- Capturas/GIFs de menu, gameplay, level up e boss.
- **Critério:** legibilidade em horda, sem shimmer, sem FX cobrindo projéteis.

---

## H. Top 10 melhorias

1. **Reexportar os quatro PNGs inválidos** (`proj_player`, `proj_enemy`, `proj_boss`, `xp_orb`) e validar o pipeline. É a correção de maior impacto e menor esforço.
2. **Fechar a linguagem de cor do combate:** tint de time em projéteis/explosões; reduzir branco; separar XP de crítico. Hoje os três times são brancos.
3. **Rebalancear FX de morte e camera shake:** death sheet visível, explosão menor/tintada/atrasada, shake só em heavy/boss.
4. **Fazer a horda animar de verdade:** usar `moveFrames`, corrigir idles duplicados, adicionar bob/pulso; hit/death do jogador; fragmento do Divisor.
5. **Redesenhar a apresentação do Núcleo:** entrada visível, primeiro ataque depois de aparecer, telegraphs diferentes para rajada e investida.
6. **Recuperar a identidade do jogador:** corpo `#4DFFE0`, acento do piloto, thruster/cockpit coloridos, ground ring, XP vs crítico distinto.
7. **Corrigir o grid e criar profundidade:** 1 px/Point, alpha baixo, starfield 1 px, nebulosa dithered, arena do boss.
8. **Pixel-perfect da UI:** `HintedRaster`, tamanhos 8n, escalas inteiras de ícones/pilotos/logo, `Canvas.pixelPerfect`, Pixel Perfect Camera se possível.
9. **Refazer HUD e menus:** XP bar com frame/label/ticks, faixa escura atrás do HUD, molduras nos painéis, fundo sutil no menu/seleção/level up.
10. **Resolver nomes e semântica:** Baluarte/Vetor/Lança, fragmento real do Divisor, ícone de Perfuração, game over com cards.

---

## Critério final — o que preservar e o que não fazer

**Preservar:**
- Paleta e silhuetas dos seis inimigos.
- Sheets do Núcleo.
- Cards, ícones e Press Start 2P.
- Paleta semântica da UI.
- Dardo direcional e charge do Olho.
- Bloom contido, grid em camadas, parallax.
- A regra “jogador ciano; inimigos quentes”.

**Não fazer:**
- Não adicionar mais branco/partículas; o problema é excesso, não falta.
- Não aumentar o Bloom para esconder alpha.
- Não trocar os inimigos por outra paleta.
- Não transformar o fundo em protagonista.
- Não usar escalas fracionárias em pixel art.
- Não animar 300 inimigos com `Update` individual; usar shader/animador compartilhado.
- Não cobrir projéteis com explosões.
- Não fazer o boss atacar fora da tela.

**Conclusão:** a 0.9.9 tem identidade e conteúdo. O próximo salto não é “mais arte”; é **integridade de assets, semântica de cor, coreografia de morte, entrada do boss e pixel-perfect**. Com esses ajustes, a 1.0 pode parecer intencional e profissional sem perder a personalidade “Sinal Morto”.
