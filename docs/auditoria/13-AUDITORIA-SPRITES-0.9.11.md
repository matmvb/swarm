# SWARM 0.9.11 — Auditoria de Sprites e Identidade Visual

**Projeto:** SWARM  
**Versão de referência:** 0.9.10 — Combat Visual Polish  
**Versão planejada:** 0.9.11 — Sprite Redesign & Visual Identity  
**Branch:** `main`  
**Commit local:** `f661a9d`  
**Remote:** `origin/main` em `7b10fff`  
**Relação:** local 2 commits à frente, 0 atrás, com alterações não commitadas da 0.9.10 preservadas  
**Modo desta rodada:** somente análise e planejamento  
**Alterações no projeto:** nenhuma  

---

## Metodologia e proteção do projeto

Antes da auditoria foram verificados:

- Branch atual.
- `git status`.
- Commit atual.
- Versão do projeto.
- Relação com o remote.
- Alterações locais não commitadas.

A versão local foi tratada como fonte de verdade. Não houve:

- Checkout de outra branch.
- Reset.
- Clean.
- Descarte de alterações.
- Commit.
- Push.
- Publicação de build.

A auditoria foi estática, baseada em:

- PNGs originais.
- Arquivos `.meta`.
- `manifest.json`.
- `ArtImportPipeline.cs`.
- ScriptableObjects de inimigos, personagens e upgrades.
- Cena e scripts de apresentação.
- Folha de conceito interna `docs/art/concept-sheet.png`.
- Folhas temporárias de comparação.

Foram geradas imagens temporárias fora da produção em:

- `/tmp/opencode/swarm_0911/ships_current.png`
- `/tmp/opencode/swarm_0911/enemies_current.png`
- `/tmp/opencode/swarm_0911/nucleo_current.png`
- `/tmp/opencode/swarm_0911/ships_current_vs_concept.png`

Não foi executada uma build nova nesta rodada. As conclusões visuais vêm da inspeção dos assets e das folhas de comparação, não de uma sessão completa de gameplay.

---

# 1. Resumo executivo

O SWARM já possui uma identidade visual reconhecível. Os inimigos são a parte mais madura do pacote: usam a paleta oficial com precisão, têm silhuetas distintas e contornos limpos. O Núcleo tem presença e animações próprias.

A base entregue na 0.9.10 — projéteis coloridos por time, explosões por classe, morte em sequência, camera shake proporcional e pipeline de importação validado — deve ser preservada.

O problema está concentrado nas **naves do jogador** e na **animação da horda**:

- As três naves compartilham o mesmo corpo ciano pálido e têm acentos de piloto inconsistentes.
- O Baluarte lê como uma nave larga, mas não como uma nave pesada e blindada.
- O Vetor tem boa silhueta de flecha, mas as asas escuras somem no fundo e o verde do piloto quase não aparece.
- O Lança tem silhueta de agulha, mas não tem canhão/emissor ofensivo visível; o amarelo é só 8 px no motor.
- Vários frames tocam a borda do canvas, sem a margem de 1 px exigida pelo brief.
- Quase todos os idles de inimigos têm frames duplicados.
- Os sheets de `move` de Drone e Dardo existem, mas não são usados.
- O canvas atual de 32×32 para naves é adequado à escala de gameplay; 48/64 só fariam sentido com mudança de escala de exibição ou pipeline pixel-perfect.

**Diagnóstico:** não é necessário aumentar a resolução nem refazer todos os assets. É necessário redesenhar parcialmente Baluarte/Lança/Corredor, refinar Vetor/Drone/Tanque/Divisor/Olho/Núcleo e corrigir margens, acentos e animações.

---

# 2. Nota geral de qualidade artística

## **7,2/10**

| Camada | Nota | Comentário |
|---|---:|---|
| Naves | 6,4 | Silhuetas boas; acentos, blindagem, margens e identidade de piloto precisam de trabalho. |
| Inimigos | 8,0 | Paleta e silhuetas fortes; Corredor é o mais fraco. |
| Boss | 7,4 | Anel e núcleo funcionam; falta detalhe interno e padding nos frames de ataque. |
| Animações | 5,8 | Idles duplicados, `move` sem uso e hit/death do jogador ausentes. |
| Pixel craft | 7,2 | Alpha e contornos limpos nos personagens; corpo da nave fora da paleta e margens insuficientes. |
| Integração Unity | 8,0 | Importação base correta; há riscos em platform overrides, arrays e âncoras. |
| Identidade geral | 7,5 | Direção clara e coerente; execução desigual entre naves e inimigos. |

**Potencial:** 9,0 sem mudar a direção “Sinal Morto”.

---

# 3. Avaliação individual das naves

## 3.1 BALUARTE — pesada

| Métrica | Valor |
|---|---|
| Frame | 32×32 |
| BBox idle | 26×29 px |
| Área desenhada | 461 px de 1.024 (45%) |
| Cores | 7 |
| Corpo dominante | `#94FFEE` |
| Acento | laranja `#FF8C2E` — 48 px |
| Margem mínima | 0 px em um dos frames |
| Tela em 1080p | ~72 px; arte ~58×65 px |

**Leitura:** a silhueta larga e os pods laterais comunicam “nave grande”, mas o corpo é um teardrop arredondado, mais simpático que blindado. Falta uma estrutura de placas, vents ou uma frente mais robusta. O cockpit é grande e legível; os motores existem, mas são pequenos.

**Veredito:** redesenho parcial, mantendo a silhueta-mestre larga. Aumentar a sensação de blindagem e estabilidade sem aumentar o canvas.

**Direção:**

- Frente mais reta/chanfrada, menos “gota”.
- Duas camadas de placas de blindagem no casco.
- Pods laterais maiores e mais integrados.
- Dois motores traseiros visíveis, com acento laranja.
- Cockpit ligeiramente menor para dar peso ao casco.
- 1 px de margem real; arte máxima de 30×30 dentro do frame 32.

---

## 3.2 VETOR — veloz

| Métrica | Valor |
|---|---|
| Frame | 32×32 |
| BBox idle | 29×29 px |
| Área desenhada | 343 px (33,5%) |
| Cores | 7 |
| Acento | `#A6FFDA`, 20 px — não é o verde oficial `#33FFB3` |
| Margem mínima | 0 px em um dos frames |
| Tela em 1080p | ~72 px; arte ~65×65 px |

**Leitura:** a silhueta é uma flecha clara e a direção de movimento é excelente. As asas são grandes, mas usam `#1A8F86` escuro e quase desaparecem no fundo. O motor é mint, não verde; o piloto “Veloz” não tem identidade cromática forte.

**Veredito:** refinamento, não redesenho completo. A silhueta está correta.

**Direção:**

- Manter a flecha e as asas varridas.
- Clarear a borda das asas ou adicionar 1 px de verde `#33FFB3`.
- Motor verde, com 2 px de brilho.
- Cockpit menor e mais fino, para reforçar velocidade.
- 1 px de margem; não tocar a borda superior/inferior.

---

## 3.3 LANÇA — ofensiva

| Métrica | Valor |
|---|---|
| Frame | 32×32 |
| BBox idle | 27×31 px |
| Área desenhada | 283 px (27,6%) |
| Cores | 7 |
| Acento | amarelo `#FFE066`, apenas 8 px no motor |
| Margem mínima | 0 px; topo toca a borda |
| Tela em 1080p | ~72 px; arte ~61×70 px |

**Leitura:** é a nave mais fina e comprida, mas a “ofensiva” não está visível. Não há canhão, emissor ou boca de fogo amarela; o cockpit é branco/escuro; o amarelo aparece só no motor. Ela lê como uma agulha, não como uma nave de ataque.

**Veredito:** redesenho parcial, preservando a silhueta longa.

**Direção:**

- Canhão frontal de 2–3 px, integrado ao nariz.
- Emissor/núcleo amarelo no cockpit ou na boca do canhão.
- Aletas laterais mais agressivas, apontadas para frente.
- Corpo central mais estreito que o Baluarte e mais comprido que o Vetor.
- 1 px de margem; nariz não pode tocar o topo do frame.

---

## 3.4 Identidade compartilhada das naves

As três naves devem continuar parecendo da mesma frota:

- Corpo ciano `#4DFFE0`.
- Highlight `#A6FFF2`.
- Sombra `#1A8F86`.
- Contorno `#02040A`.
- Cockpit/core com 1–2 px claros.
- Acento do piloto no cockpit, motor ou núcleo.
- Mesma lógica de luz: topo/esquerda claro, base/direita escuro.
- Sem anti-aliasing, sem gradiente, sem ruído de 1 px.
- 1 px de margem transparente em todos os frames.
- 5–7 cores por sprite.

**Problema atual:** o corpo dominante é `#94FFEE`, um ciano pálido fora da paleta oficial, e o `#4DFFE0` tem menos presença. Isso deixa a nave menos saturada e mais próxima do branco dos efeitos. O Baluarte tem 48 px de laranja; o Vetor tem 20 px de mint; o Lança tem 8 px de amarelo. A identidade de piloto existe, mas é desigual.

---

# 4. Avaliação individual dos inimigos

| Inimigo | Frame | BBox | Área | Cores | Leitura | Veredito |
|---|---:|---:|---:|---:|---|---|
| Drone/Esporo | 16×16 | 13×15 | 149 px (58%) | 4 | Anel compacto, olho central; forte | Refinar |
| Corredor/Dardo | 20×20 | 17×13 | 98 px (24,5%) | 3 | Flecha muito vazia; pouco corpo | Redesenhar |
| Tanque/Casco | 32×32 | 25×29 | 545 px (53%) | 3 | Escudo/máscara pesada; forte | Refinar |
| Divisor/Célula | 24×24 | 20×22 | 320 px (55%) | 4 | Célula com dois olhos e costura; excelente | Preservar |
| Atirador/Olho | 24×24 | 22×21 | 233 px (40%) | 3 | Diamante/olho com canos; forte | Refinar |
| Núcleo | 96×96 | 93×93 | 2.738 px (30%) | 4 | Anel quebrado + núcleo; forte | Refinar |
| Fragmento | 12×12 | 11×11 | 80 px (55%) | 3 | Pequeno, sem uso atual | Integrar/redesenhar |

## Drone/Esporo

**Pontos fortes:** é o inimigo mais compacto e legível. O anel magenta com olho central funciona bem em horda. A paleta é exata: `#FF4D8D`, `#FF9BBE`, `#A81F52`, `#02040A`.

**Problemas:** o idle tem dois frames idênticos; não há pulsação de olho nem movimento. O frame toca a borda inferior. A forma é quase circular; o conceito é hexagonal, o que daria mais identidade.

**Direção:** manter o anel/olho; hexagonalizar levemente o contorno; adicionar 2 pequenos espinhos/rotores; 1 px de margem; 2 frames de idle com olho pulsando 1 px.

---

## Corredor/Dardo

**Pontos fortes:** a flecha laranja comunica velocidade; a direção é clara; a rotação em jogo ajuda.

**Problemas:** ocupa apenas 24,5% do frame; o corpo é muito fino e vazio. O bbox toca a borda direita (x=19). O idle tem frames idênticos; o sheet `move` tem 4 frames, mas não é usado. Em gameplay, ele pode parecer um risco/garrancha em vez de nave.

**Direção:** redesenhar o corpo com mais massa central, cauda dupla mais definida e motor laranja; manter a seta; usar os 4 frames de `move`; 1 px de margem.

---

## Tanque/Casco

**Pontos fortes:** é o inimigo mais “pesado” visualmente; o escudo/máscara roxo é forte; 53% de ocupação; contorno limpo.

**Problemas:** usa só 3 cores; falta profundidade de blindagem. O idle tem frames idênticos. O topo tem um recorte escuro que pode parecer dano. A forma é boa, mas poderia ter mais camadas de placa e vents.

**Direção:** preservar a silhueta de escudo; adicionar 1–2 placas e vents; usar 5 cores no máximo; 2 frames de idle com vents pulsando.

---

## Divisor/Célula

**Pontos fortes:** é o inimigo mais bem resolvido. Dois olhos, costura central, lóbulos e paleta verde exata. Lê imediatamente como “célula que divide”.

**Problemas:** idle com frames idênticos; em gameplay, os olhos não piscam. O fragmento correspondente não é usado; o Divisor ainda gera Drones.

**Direção:** preservar o desenho; adicionar pulso de 1 px nos lóbulos/olhos; criar/ligar o fragmento 12×12; 1 px de margem.

---

## Atirador/Olho

**Pontos fortes:** o losango azul com olho central e canos laterais é distinto; a paleta azul é exata; a animação de charge existe.

**Problemas:** idle com frames idênticos; o olho poderia piscar; os canos são pequenos; o charge tem 3 frames, mas o contraste da pupila pode ser maior.

**Direção:** preservar o losango; reforçar canos e lente; usar o charge existente; 2 frames de idle com piscada.

---

## Núcleo

**Pontos fortes:** anel vermelho quebrado, núcleo central e shards; presença clara; idle de 4 frames realmente animado; telegraph/attack/death próprios.

**Problemas:** o anel ocupa 30% do frame e o interior é muito vazio; falta uma camada interna ou shards mais evidentes. Os frames de `attack` tocam a borda 0..95, sem margem. O telegraph e o attack são visualmente próximos; o jogador pode não distinguir rajada de investida.

**Direção:** adicionar anel interno/shards; manter o núcleo branco; reservar 2–4 px de margem no frame 96 ou redesenhar o ataque dentro de 92×92; diferenciar telegraph de rajada e investida.

---

# 5. Diagnóstico das animações

## Idle

| Asset | Frames | Diferença real | Diagnóstico |
|---|---:|---:|---|
| `ship_baluarte_idle` | 2 | 45 px | Sutil; funciona, mas pode ter 4 frames de núcleo/motor |
| `ship_vetor_idle` | 2 | 22 px | Sutil; asas quase estáticas |
| `ship_lanca_idle` | 2 | 15 px | Quase estático |
| `enemy_esporo_idle` | 2 | **0** | Totalmente estático |
| `enemy_dardo_idle` | 2 | **0** | Totalmente estático |
| `enemy_casco_idle` | 2 | **0** | Totalmente estático |
| `enemy_celula_idle` | 2 | **0** | Totalmente estático |
| `enemy_olho_idle` | 2 | **0** | Totalmente estático |
| `enemy_fragmento_idle` | 2 | **0** | Totalmente estático |
| `enemy_nucleo_idle` | 4 | 236/191/344 | Animado e bom |

**Conclusão:** o sistema de animação funciona, mas os assets de idle de quase todos os inimigos são frames duplicados. A horda desliza como tokens.

## Move

- `enemy_esporo_move` tem 2 frames com diferença 72, mas não é usado.
- `enemy_dardo_move` tem 4 frames com diferença 100, mas não é usado.
- Tanque, Divisor e Olho não têm move sheet; podem usar bob/pulso de 1 px via código.
- Naves não têm move sheet; o thruster de 3 frames já comunica movimento.

## Hit

- Todos os inimigos têm 1 frame branco de hit.
- `Enemy` usa esse frame por ~0,09 s e aplica overlay branco/amarelo.
- O frame branco apaga a silhueta por um instante; em hordas, isso pode ser aceitável, mas é agressivo.
- As naves têm `hit` de 1 frame branco, mas não usam.

## Death

- Inimigos: 4 frames (Drone/Dardo/Divisor/Olho), 6 (Tanque) e 8 (Núcleo).
- A sequência 0.9.10 já toca a death sheet antes da explosão; isso deve ser preservado.
- As naves têm `death` de 6 frames, mas não usam; o jogador morre sem animação.
- Os frames de morte das naves têm 36–39 cores; se redesenhados, limitar a paleta.

## Boss

- Idle 4, telegraph 4, attack 4, hit 1, death 8.
- A arte é boa, mas o mesmo telegraph serve para rajada e investida.
- `attack` toca as bordas; falta padding.
- A morte em 8 frames é forte e deve ser preservada.

## Recomendação de frames

| Categoria | Idle | Move | Hit | Death |
|---|---|---|---|---|
| Naves | 4 @ 6 fps | thruster 3–4 @ 12 fps | 1 @ 12 fps / 0,05–0,07 s | 6 @ 12 fps |
| Drone | 2 @ 6 fps | 2 @ 8 fps | 1 @ 12 fps | 4 @ 12 fps |
| Corredor | 2 @ 8 fps | 4 @ 10 fps | 1 @ 12 fps | 4 @ 12 fps |
| Tanque | 2 @ 4–6 fps | bob de 1 px | 1 @ 12 fps | 6 @ 10–12 fps |
| Divisor | 2 @ 6 fps | bob de 1 px | 1 @ 12 fps | 4 @ 12 fps |
| Olho | 2 @ 6 fps | hover de 1 px | 1 @ 12 fps | 4 @ 12 fps |
| Núcleo | 4 @ 6 fps | — | 1 @ 12 fps | 8 @ 10–12 fps |

Cada frame precisa ter função: núcleo pulsando, motor variando, olho piscando, lóbulo respirando, chama encurtando. Não adicionar frames apenas para aumentar contagem.

---

# 6. Comparação entre 32×32, 48×48 e 64×64

A câmera tem `orthographicSize = 6`; em 1080p, isso dá ~90 px por unidade.

| Resolução da nave | Tamanho na tela a 0,8 u | Pixels de tela por pixel de arte | Leitura |
|---|---:|---:|---|
| 32×32 | 72 px | **2,25** | Pixels grandes, coerentes com a horda |
| 48×48 | 72 px | **1,50** | Mais detalhe, mas pixels irregulares e menos “pixel art” |
| 64×64 | 72 px | **1,125** | Detalhe alto, mas quase subpixel; não recomendado |

Para manter 2,25 px de tela por pixel de arte com resoluções maiores, seria necessário aumentar o tamanho da nave:

| Resolução | Tamanho de mundo necessário | Tamanho na tela |
|---|---:|---:|
| 32×32 | 0,80 u | 72 px |
| 48×48 | 1,20 u | 108 px |
| 64×64 | 1,60 u | 144 px |

## Comparação com os inimigos

| Inimigo | Frame | Escala | Tela | Pixels de tela/arte |
|---|---:|---:|---:|---:|
| Drone | 16 | 0,42 | 37,8 px | 2,36 |
| Corredor | 20 | 0,48 | 43,2 px | 2,16 |
| Tanque | 32 | 0,95 | 85,5 px | 2,67 |
| Divisor | 24 | 0,60 | 54 px | 2,25 |
| Atirador | 24 | 0,58 | 52,2 px | 2,18 |
| Núcleo | 96 | 2,20 | 198 px | 2,06 |

A densidade atual está entre 2,06 e 2,67 pixels de tela por pixel de arte. É uma faixa boa. Um sprite de nave em 48 reduziria para 1,5 e ficaria visivelmente mais fino que o resto da horda.

## Recomendação

**Manter 32×32 para as naves, 16/20/24/32 para os inimigos e 96×96 para o Núcleo.**

O ganho deve vir de:

- Silhueta.
- Blindagem.
- Acentos.
- Margem.
- Cockpit/motor.
- Animação.

Não de mais pixels.

**Exceção possível:** 48×48 para naves somente se elas forem exibidas a 1,2 u (108 px) e o projeto aceitar revisar âncoras de muzzle/thruster, collider visual e sensação de escala. 64×64 não é recomendado nesta câmera.

---

# 7. Resolução recomendada por categoria

| Categoria | Resolução | Justificativa |
|---|---:|---|
| Naves | **32×32** | 2,25 px/arte; mantém densidade, permite 1 px de margem e 5–7 cores |
| Drone | **16×16** | Menor unidade da horda; 2,36 px/arte |
| Corredor | **20×20** | Flecha horizontal; 2,16 px/arte |
| Tanque | **32×32** | Massa e blindagem; 2,67 px/arte |
| Divisor | **24×24** | Célula e dois olhos; 2,25 px/arte |
| Atirador | **24×24** | Losango/olho; 2,18 px/arte |
| Núcleo | **96×96** | 2,06 px/arte; 198 px na tela |
| Fragmento | **12×12** | Menor que o Drone; usar no split |
| Thruster | **8×16** | Já integrado; 3 frames |
| Portraits | **64×64** | Hoje exibidos a 96 (1,5×); ideal 64 ou 128 |

**Regra:** a menor resolução que permita a qualidade. Para este projeto, é 32 para naves e 16–32 para inimigos.

---

# 8. Guia de identidade visual do SWARM

## Linguagem

**Pixel art espacial neon, geométrica e dura, com leitura arcade e contraste alto.**

## Paleta

| Papel | Hex |
|---|---|
| Void | `#05070F` |
| Painel | `#0B1020` |
| Contorno | `#02040A` |
| Ciano | `#4DFFE0` |
| Ciano claro | `#A6FFF2` |
| Ciano escuro | `#1A8F86` |
| Magenta | `#FF4D8D` |
| Magenta claro | `#FF9BBE` |
| Laranja | `#FF8C2E` |
| Roxo | `#BF33FF` |
| Verde | `#33FFB3` |
| Azul | `#59BFFF` |
| Vermelho | `#FF2626` |
| Amarelo | `#FFE066` |
| Branco | `#EBF7FF` |
| Muted | `#8FA3B8` |

## Regras

1. Jogador: ciano + acento do piloto.
2. Inimigos: uma cor dominante por classe + 1 highlight + 1 sombra.
3. Núcleo: vermelho + núcleo branco + acento âmbar.
4. Contorno de 1 px em `#02040A`.
5. 5–7 cores por sprite de personagem.
6. 1 px de margem transparente.
7. Núcleo/olho com 1–2 px claros.
8. Luz no topo/esquerda; sombra na base/direita.
9. Sem anti-aliasing, gradiente ou ruído de 1 px.
10. Branco só para núcleo, flash e impacto.
11. Nada de detalhe que desapareça a 1×.
12. A silhueta deve funcionar em preto puro.

## Acentos do piloto

| Piloto | Acento | Onde |
|---|---|---|
| Baluarte | Laranja `#FF8C2E` | Pods, motores, cockpit |
| Vetor | Verde `#33FFB3` | Borda das asas, motor |
| Lança | Amarelo `#FFE066` | Cockpit, canhão, emissor |

Hoje:

- Baluarte tem 48 px de laranja.
- Vetor tem 20 px de `#A6FFDA`, não o verde oficial.
- Lança tem 8 px de amarelo apenas no motor.
- O corpo dominante é `#94FFEE`, fora da paleta.

O redesign deve corrigir isso.

---

# 9. Problemas de integração com Unity

## 9.1 Importação atual

Os `.meta` atuais estão corretos no essencial:

- `spriteMode: 2` (Multiple).
- `spritePixelsToUnits`: 32 para naves, 16 para Drone, etc.
- `filterMode: 0` (Point).
- `textureCompression: 0` no DefaultTexturePlatform.
- `mipmaps` desligado.
- `alphaIsTransparency: 1`.
- `spriteMeshType: 0` (FullRect).
- `spriteExtrude: 0`.
- Pivot central.
- `maxTextureSize: 2048`.
- `sRGBTexture: 1`.
- `nPOTScale: 0` (None).

O `ArtImportPipeline` 0.9.10 valida PNG, CRC, chunks, zlib, filtros, dimensões e transparência. Isso deve ser preservado.

## 9.2 Risco de compressão em Standalone/WebGL

Os `.meta` têm entradas de plataforma:

- `Standalone`: `textureCompression: 1`, `overridden: 0`
- `WebGL`: `textureCompression: 1`, `overridden: 0`

Com `overridden: 0`, o override não está ativo; o DefaultTexturePlatform (`textureCompression: 0`) tende a valer. Mas o valor gravado é compressão. Se um override for ativado, o pixel art pode ficar borrado.

**Recomendação:** o pipeline deve aplicar explicitamente `SetPlatformTextureSettings` para Standalone e WebGL com:

- `textureCompression = Uncompressed`
- `filterMode = Point`
- `maxTextureSize = 2048`
- `overridden = true`

E validar com `GetPlatformTextureSettings`.

## 9.3 Dependências de frames e sprites

Se o redesign mudar frame size ou frame count:

- Atualizar `manifest.json`: `frameWidth`, `frameHeight`, `frames`, `sheetWidth`, `sheetHeight`, `ppu`, `fps`.
- Atualizar os `.meta` slices.
- Atualizar `EnemyData.idleFrames/moveFrames/hitFrames/deathFrames`.
- Atualizar `CharacterData.idleFrames/hitFrames/deathFrames/thrusterFrames/portrait`.
- Atualizar `ui_pilot_*` e os cards.
- Rebuildar a cena via `SwarmSceneBuilder` ou atualizar os SpriteRenderers.

**Ponto crítico:** `ArtImportPipeline` atualiza apenas `EnemyData.sprite` e `CharacterData.shipSprite`. As arrays de frames são preenchidas pelo `SwarmSceneBuilder`. Se o artista reimportar os sprites e não rodar o builder, as arrays podem continuar apontando para sprites antigos. O pipeline deve ser estendido para atualizar arrays ou o fluxo deve exigir builder.

## 9.4 PPU e escala

O projeto usa **PPU = tamanho do frame**. Isso mantém o tamanho de mundo estável quando o frame muda:

- 32 px em PPU 32 = 1 unidade.
- 48 px em PPU 48 = 1 unidade.
- 64 px em PPU 64 = 1 unidade.

Se o frame mudar para 48/64 e o PPU não mudar, a nave ficará 1,5×/2× maior no mundo. Isso alteraria a leitura e a relação com collider.

**Regra:** se a resolução mudar, o PPU deve mudar junto.

## 9.5 Muzzle e thruster

Hoje:

- Muzzle do jogador: `localPosition (0, 0.55)`.
- Thruster: `localPosition (0, -0.62)`.
- `CharacterData` não possui offsets por piloto.

As naves têm comprimentos diferentes. Se o Baluarte ficar mais curto, o Lança mais comprido ou o Vetor mais fino, os tiros podem nascer dentro do casco ou o thruster pode flutuar.

**Recomendação:** adicionar ao `CharacterData`:

- `muzzleOffset`
- `thrusterOffset`
- `visualScale` (se necessário)

E configurar no builder por piloto. Não alterar o dano; apenas a âncora visual.

## 9.6 Portraits e UI

- `ui_pilot_*` são 64×64.
- Os cards exibem 96×96 (1,5×), escala fracionária.
- Se as naves mudarem, os portraits precisam ser atualizados.
- Preferir exibição 64 ou 128 (1×/2×).

## 9.7 Colliders

Os inimigos usam `CircleCollider2D.radius = 0.5` no root, com escala de `EnemyData.scale`. O redesign não deve mudar a silhueta de forma a deixar o collider muito maior/menor que o desenho. Não alterar mecânicas; apenas verificar a relação visual.

## 9.8 Performance

- 300 inimigos é o teto do spawner.
- 32/48/64 não mudam draw calls; mudam memória de textura e overdraw.
- Mais frames de idle/move aumentam o custo do `SpriteAnimator`, mas é pequeno.
- Evitar 300 animators individuais com lógica pesada; o `SpriteAnimator` atual é simples e adequado.
- Não criar partículas por frame; o combate 0.9.10 já tem FX poolados.

---

# 10. Assets que devem ser preservados

## Preservar sem redesenho estrutural

- `enemy_celula_idle/hit/death` — melhor inimigo do pacote.
- `enemy_olho_idle/charge/hit/death` — losango/olho forte.
- `enemy_casco_idle/hit/death` — silhueta de escudo pesado.
- `enemy_esporo_idle/hit/death` — anel compacto.
- `enemy_nucleo_idle/telegraph/attack/death` — anel/núcleo e frames.
- `ship_baluarte_idle` — silhueta larga.
- `ship_vetor_idle` — flecha.
- `ship_lanca_idle` — agulha.
- `enemy_fragmento_idle` — base para o split.

## Preservar do 0.9.10

- Sistema de cor de projéteis por time.
- Explosões coloridas por classe.
- Sequência death sheet → explosão.
- Camera shake por categoria.
- XP dourado vs crítico laranja.
- `ArtImportPipeline` com validação PNG.
- Level up em cards, paleta da UI e fonte pixel.
- Bloom contido e fundo `#05070F`.

---

# 11. Assets que devem ser redesenhados

## Redesenho parcial/do zero

- **Baluarte:** novo casco blindado, placas, motores, cockpit; manter silhueta larga.
- **Lança:** canhão/emissor frontal, cockpit amarelo, aletas agressivas; manter agulha.
- **Corredor/Dardo:** corpo com massa, cauda dupla, motor, margem; manter seta.
- **Fragmento:** desenho 12×12 próprio para o Divisor.
- **Frames de idle** de Drone/Tanque/Divisor/Olho: redesenhar o segundo frame.
- **Frames de move** de Drone/Corredor: usar/ajustar.
- **Portraits** `ui_pilot_*`: atualizar se as naves mudarem.
- **Thrusters:** acento do piloto e 3–4 frames.

## Refinamento

- **Vetor:** asas/verde/motor/cockpit; não redesenhar a flecha.
- **Drone:** hexagonalizar, rotor/espinhos, olho pulsante.
- **Tanque:** placas/vents, 5 cores.
- **Divisor:** preservar; só pulso.
- **Atirador:** canos/lente/charge.
- **Núcleo:** anel interno/shards; padding do attack; telegraph distinto.
- **Naves hit/death:** usar os sheets existentes, integrar ao jogador.

---

# 12. Comparação visual

Foram geradas folhas temporárias fora da produção:

- `ships_current.png`: naves atuais em 8× e escala de gameplay.
- `enemies_current.png`: inimigos em 4× e escala de gameplay.
- `nucleo_current.png`: 4 frames do Núcleo em 2×.
- `ships_current_vs_concept.png`: naves atuais ao lado dos painéis da folha de conceito interna (`docs/art/concept-sheet.png`).

## O que a comparação mostra

- **Baluarte:** o conceito é mais largo, chanfrado e blindado; o atual é um teardrop arredondado com pods pequenos.
- **Vetor:** o conceito tem asas verdes e motor verde; o atual tem asas escuras e motor mint.
- **Lança:** o conceito tem cockpit/canhão amarelo e nariz ofensivo; o atual tem agulha e 8 px de amarelo no motor.
- **Inimigos:** Drone, Tanque, Divisor e Olho já estão próximos do conceito. O Corredor é o mais distante (corpo vazio). O Núcleo está próximo, mas o interior pode ganhar camadas.
- **Escala de gameplay:** as silhuetas das naves funcionam, mas os acentos somem; o Corredor fica fino; o Tanque e o Divisor permanecem fortes; o Núcleo domina.

Não usei imagens externas; a referência de proposta é a folha de conceito interna do projeto.

---

# 13. Top 15 melhorias prioritárias

1. Redesenhar o Baluarte com casco blindado, placas, pods maiores e dois motores; manter a silhueta larga.
2. Redesenhar o Lança com canhão/emissor frontal, cockpit amarelo e aletas agressivas.
3. Refinar o Vetor com asas mais contrastadas, verde `#33FFB3` e motor verde.
4. Padronizar margem de 1 px em todos os frames; arte máxima 30×30 dentro de 32.
5. Unificar a paleta das naves: corpo `#4DFFE0`, highlight `#A6FFF2`, sombra `#1A8F86`, acentos exatos.
6. Redesenhar o Corredor com massa central, cauda dupla, motor e margem; usar `move` de 4 frames.
7. Refinar o Tanque com 1–2 placas, vents e 5 cores; preservar o escudo.
8. Refinar o Drone com contorno hexagonal, rotor/espinhos e olho pulsante.
9. Corrigir os idles duplicados de Drone, Tanque, Divisor e Olho.
10. Implementar move/bob para a horda: Drone/Corredor com sheets; Tanque/Divisor/Olho com bob de 1 px.
11. Refinar o Núcleo: anel interno/shards, padding no attack e telegraph distinto para rajada/investida.
12. Refinar o Atirador: canos, lente e leitura do charge.
13. Integrar o fragmento 12×12 no Divisor.
14. Integrar hit/death das naves e ajustar muzzle/thruster por piloto; atualizar portraits.
15. Validar importação e plataformas: PPU/frames, arrays de dados, platform overrides, WebGL/Standalone, 1×/2×/720p/1080p/4K.

---

# 14. Plano de implementação por etapas

## Etapa 0 — Pré-produção

- Fixar 32×32 para naves e 16/20/24/32/96 para inimigos/boss.
- Fechar paleta e regras de 1 px de margem.
- Definir silhuetas-mestre e âncoras de muzzle/thruster.
- Criar fontes editáveis (`.piskel`/`.aseprite`) em `docs/art/source`.
- Gerar folha de comparação final.

## Etapa 1 — Naves

- Baluarte: redesenho parcial.
- Lança: redesenho parcial.
- Vetor: refinamento.
- Idle de 2–4 frames; hit e death.
- Portraits e âncoras por piloto.
- Critério: três naves distintas em silhueta e acento; jogador reconhecível em 1 s; margem de 1 px.

## Etapa 2 — Inimigos

- Corredor: redesenho.
- Drone, Tanque, Olho: refinamento.
- Divisor: preservar e animar.
- Fragmento: integrar.
- Critério: cada classe reconhecível em preto puro e em horda.

## Etapa 3 — Núcleo

- Anel interno/shards.
- Padding dos frames de attack/telegraph.
- Telegraph distinto para rajada/investida.
- Critério: o boss lê como chefe, não como inimigo grande.

## Etapa 4 — Animações

- Idles reais.
- `move` de Drone/Corredor.
- Bob/pulso de Tanque/Divisor/Olho.
- Hit/death das naves.
- Thruster com acento.
- Critério: horda não desliza estática; cada frame tem função.

## Etapa 5 — Integração Unity

- Atualizar manifesto, frames, PPU, `.meta`, arrays de dados e cena.
- Atualizar `ArtImportPipeline` para arrays e platform settings.
- Ajustar muzzle/thruster/portrait.
- Validar Standalone/WebGL.
- Critério: nenhuma referência quebrada; importação 0 erros.

## Etapa 6 — QA visual

- 1×, 2×, 4×.
- 1280×720, 1920×1080, 2560×1440, 4K.
- Horda de 30–50 e 300 inimigos.
- Colorblind/grayscale.
- Comparar com 0.9.10.
- Critério: silhuetas legíveis, acentos visíveis, nenhum sprite tocando borda, nenhum blur.

---

# 15. Limitações desta auditoria

- Não foi executada build nova.
- Não foi feita sessão completa de gameplay.
- Não foi testado WebGL.
- Não foi feito stress de 300 inimigos.
- Não foram gerados assets finais de redesign; a proposta usa a folha de conceito interna e as folhas temporárias de comparação.
- As notas são qualitativas e técnicas, não um teste automatizado de percepção.
- O projeto local permanece com as alterações não commitadas da 0.9.10; nenhuma foi alterada nesta rodada.

---

# Conclusão

O SWARM não precisa de uma reformulação completa. Os inimigos e o Núcleo já têm uma base forte; as naves têm silhuetas corretas, mas execução desigual.

O maior ganho virá de:

- Redesenho parcial de Baluarte e Lança.
- Refinamento do Vetor.
- Redesenho do Corredor.
- Margem de 1 px e paleta oficial em todas as naves.
- Idles reais e `move` em uso.
- Integração de âncoras, portraits, arrays e platform settings.

A resolução recomendada é **32×32 para naves**, **16–32 para inimigos** e **96×96 para o Núcleo**. Aumentar para 48/64 sem mudar a escala de exibição ou adotar pixel-perfect quebraria a densidade atual.

**Nenhum arquivo do projeto foi alterado nesta rodada.**
