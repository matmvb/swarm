# SWARM — brief de sprites para Piskel

**Versão:** 0.9.8  
**Folha visual:** [`concept-sheet.png`](art/concept-sheet.png) · [`concept-sheet.html`](art/concept-sheet.html)  
**Paleta:** [`swarm-palette.gpl`](art/swarm-palette.gpl)

---

## 1. Direção de arte

**Nome da campanha:** *Sinal Morto*.

O jogador é uma **Cunha**: geometria limpa, simétrica, fechada e ciano — um sinal
organizado atravessando uma rede corrompida. Os inimigos são **Ecos**: polígonos
abertos, assimétricos, com núcleos ocos e cores quentes. O boss é o **Núcleo**, a
fonte do enxame: um anel quebrado em volta de um olho vermelho.

| Linguagem | Jogador | Inimigos |
|---|---|---|
| Forma | fechada, simétrica, aponta para cima | aberta, assimétrica, aponta para fora/baixo |
| Cor | ciano + acento do piloto | uma cor dominante por tipo |
| Detalhe | cockpit/core central | olho, oco ou costura central |
| Leitura | “eu” | “ameaça” |

**Regras de ouro**

1. Silhueta reconhecível em preto puro a 1×.
2. No máximo **5–7 cores por sprite**.
3. Contorno escuro de **1 px** (`#02040A`).
4. Brilho vem do **Bloom da URP**, não de pixels semitransparentes.
5. Nada de antialias, gradiente ou ruído de 1 px.
6. Deixe **1 px de margem** nas naves e inimigos para o contorno respirar.
7. O olho/core é o ponto de leitura: 1–2 px claros, sempre.

---

## 2. Especificações técnicas

| Item | Valor |
|---|---|
| Estilo | pixel art 1×, fundo transparente |
| Pivô | centro (`0.5, 0.5`) |
| Colisor | manter `CircleCollider2D.radius = 0.5` no root |
| PPU recomendado | **maior dimensão do frame** (32 px → 32, 16 → 16, 96 → 96) |
| Filtro | Point (no filter) |
| Compressão | None |
| Mipmaps | desligado |
| Alpha Is Transparency | ligado |
| Mesh Type | Full Rect |
| Extrude Edges | 0 |
| Max Size | 2048 |
| Material | Sprite-Unlit atual |

### Por que PPU = tamanho do frame

O projeto atual cria sprites 64×64 com PPU 64, então **todo sprite tem 1 unidade**
e o campo `EnemyData.scale` define o tamanho final. Se você importar o novo sprite
com PPU igual ao frame, os scales atuais continuam valendo:

| Asset | Frame | PPU | Scale atual | Tamanho no mundo |
|---|---:|---:|---:|---:|
| Nave | 32×32 | 32 | 0.8 | 0.8 u |
| Esporo | 16×16 | 16 | 0.7 | 0.7 u |
| Dardo | 20×20 | 20 | 0.55 | 0.55 u |
| Casco | 32×32 | 32 | 1.4 | 1.4 u |
| Célula | 24×24 | 24 | 1.0 | 1.0 u |
| Olho | 24×24 | 24 | 0.8 | 0.8 u |
| Núcleo | 96×96 | 96 | 2.2 | 2.2 u |
| Projétil | 8×8 | 8 | 0.22 | 0.22 u |
| Orbe | 12×12 | 12 | 0.3 | 0.3 u |
| XP | 8×8 | 8 | 0.3 | 0.3 u |

> Em uma folha (sprite sheet), o PPU é o tamanho do **frame**, não da folha.
> Ex.: folha 64×32 com frames 32×32 → PPU 32.

### Pixel-perfect

A câmera atual é ortográfica com size 6 (12 unidades de altura). Em 1080p, cada
unidade tem ~90 px; um sprite de 32 px vira ~72 px na tela (2,25 px por pixel de
arte). O upscale não é inteiro. Para pixel-perfect absoluto, seria necessário um
**Pixel Perfect Camera** com referência 480×270 e PPU 32. Nesta rodada, use Point
Filter e aceite o leve shimmer; o Bloom e o grid disfarçam. Se quiser, eu configuro
a Pixel Perfect Camera depois.

---

## 3. Paleta

| Nome | Hex | Uso |
|---|---|---|
| Void | `#05070F` | fundo |
| Panel | `#0B1020` | painéis/UI |
| Outline | `#02040A` | contorno de tudo |
| Cyan | `#4DFFE0` | nave, projétil, orbe |
| Cyan hi | `#A6FFF2` | cockpit, highlight |
| Cyan lo | `#1A8F86` | sombra da nave |
| Magenta | `#FF4D8D` | Esporo |
| Magenta hi | `#FF9BBE` | olho/highlight |
| Orange | `#FF8C2E` | Dardo / piloto Tanque |
| Purple | `#BF33FF` | Casco |
| Green | `#33FFB3` | Célula |
| Blue | `#59BFFF` | Olho |
| Red | `#FF2626` | Núcleo / perigo |
| Yellow | `#FFE066` | aviso / piloto Atirador |
| White | `#EBF7FF` | núcleo, flash, olho |
| Muted | `#8FA3B8` | texto secundário |

**Distribuição:** cada inimigo usa 1 cor dominante + 1 highlight + 1 sombra + outline.
O jogador é sempre ciano; o piloto entra só como acento (laranja, verde ou amarelo).

---

## 4. Naves — 32×32

Todas: 32×32, pivô `(16,16)`, contorno 1 px, cockpit central, 1 px de margem.
A nave gira para o alvo; o **muzzle** fica na ponta e o **engine** na base.

### 4.1 BALUARTE — piloto Tanque

- **Silhueta:** casco largo, pods laterais, nariz curto; lê como “muralha”.
- **Cores:** ciano + acento laranja; cockpit ciano-claro; engine laranja.
- **Detalhes:** pods 5×9 px, cockpit 4×6, engine 10×3, duas saídas de chama.
- **Muzzle:** `(16,1)` · **Engine:** `(16,29)`.
- **Animação:** idle 2 frames (respiração de 1 px), hit 1, death 6.
- **Leitura:** deve ser a nave mais “gorda” e estável das três.

### 4.2 VETOR — piloto Veloz

- **Silhueta:** flecha estreita, asas varridas para trás, nariz longo; lê como “velocidade”.
- **Cores:** ciano + acento verde; cockpit ciano-claro; engine verde.
- **Detalhes:** asas 8×10 px, cockpit 2×5, engine 6×2, duas linhas de velocidade.
- **Muzzle:** `(16,0)` · **Engine:** `(16,29)`.
- **Animação:** idle 2 frames (asas vibram 1 px), hit 1, death 6.
- **Leitura:** deve ser a nave mais fina e pontuda.

### 4.3 LANÇA — piloto Atirador

- **Silhueta:** agulha longa, aletas laterais, canhão traseiro; lê como “precisão”.
- **Cores:** ciano + acento amarelo; cockpit amarelo; boca do canhão amarela.
- **Detalhes:** aletas 9×6 px, cockpit 2×4, canhão 4×5, nariz de 1 px.
- **Muzzle:** `(16,0)` · **Engine:** `(16,30)`.
- **Animação:** idle 2 frames (mira pulsa), hit 1, death 6.
- **Leitura:** deve ser a nave mais comprida e fina.

### Folhas das naves

| Arquivo | Frames | Frame | Folha | FPS |
|---|---:|---:|---:|---:|
| `ship_baluarte_idle.png` | 2 | 32×32 | 64×32 | 6 |
| `ship_baluarte_hit.png` | 1 | 32×32 | 32×32 | — |
| `ship_baluarte_death.png` | 6 | 32×32 | 192×32 | 12 |
| `ship_baluarte_thruster.png` | 3 | 8×16 | 24×16 | 12 |
| `ship_vetor_idle.png` | 2 | 32×32 | 64×32 | 6 |
| `ship_vetor_hit.png` | 1 | 32×32 | 32×32 | — |
| `ship_vetor_death.png` | 6 | 32×32 | 192×32 | 12 |
| `ship_vetor_thruster.png` | 3 | 8×16 | 24×16 | 12 |
| `ship_lanca_idle.png` | 2 | 32×32 | 64×32 | 6 |
| `ship_lanca_hit.png` | 1 | 32×32 | 32×32 | — |
| `ship_lanca_death.png` | 6 | 32×32 | 192×32 | 12 |
| `ship_lanca_thruster.png` | 3 | 8×16 | 24×16 | 12 |

> O thruster pode ser o mesmo para as três naves, tingido pelo acento do piloto.

---

## 5. Inimigos

### 5.1 ESPORO — Drone · 16×16

- **Papel:** horda, muitos na tela.
- **Silhueta:** hexágono invertido, oco, com um olho central; 6 pontas curtas.
- **Cores:** magenta dominante, olho magenta-claro, contorno escuro.
- **Detalhes:** 1 px de contorno, oco de 6×8, olho de 2×2, sulcos de 1 px.
- **Pivô:** `(8,8)` · **Colisor:** `r=6 px`.
- **Animação:** idle 2 (olho pulsa), death 4. Movimento pode ser código (bob/rotação).
- **Leitura:** o inimigo mais simples; não coloque detalhe que suma a 1×.

### 5.2 DARDO — Corredor · 20×20

- **Papel:** rápido, persegue o jogador.
- **Silhueta:** flecha horizontal com cauda dupla; aponta para a direção do movimento.
- **Cores:** laranja dominante, olho laranja-claro, contorno escuro.
- **Detalhes:** nariz de 1 px, corpo de 10×6, cauda em duas pontas, olho de 2×2.
- **Pivô:** `(10,10)` · **Colisor:** `r=6 px`.
- **Animação:** idle 2, move 4 (cauda chicoteia), death 4.
- **Leitura:** a silhueta deve lembrar uma seta mesmo sem cor.

### 5.3 CASCO — Tanque · 32×32

- **Papel:** lento, muito HP, dano de contato alto.
- **Silhueta:** hexágono blindado, placa frontal e vents laterais; lê como “parede”.
- **Cores:** roxo dominante, highlights roxo-claro, núcleo escuro.
- **Detalhes:** placa frontal 12×6, vents 3×8, núcleo 6×6, três linhas de blindagem.
- **Pivô:** `(16,16)` · **Colisor:** `r=12 px`.
- **Animação:** idle 2 (vents pulsam), death 6 (placas se soltam).
- **Leitura:** deve ser o inimigo mais largo e “quadrado” da horda.

### 5.4 CÉLULA — Divisor · 24×24

- **Papel:** ao morrer, divide em dois fragmentos.
- **Silhueta:** círculo com costura vertical e dois lóbulos; lê como “célula”.
- **Cores:** verde dominante, lóbulos verde-claro, costura escura.
- **Detalhes:** círculo r=10, costura 2×18, dois olhos 2×2, tendrils de 2×3.
- **Pivô:** `(12,12)` · **Colisor:** `r=9 px`.
- **Animação:** idle 2 (lóbulos pulsam), death 4 (abre e libera dois esporos).
- **Fragmento:** `enemy_fragmento_idle.png` 12×12, 2 frames, mesmo verde; pode ser o
  próprio Esporo reduzido a 0.5, mas um sprite 12×12 fica mais legível.

### 5.5 OLHO — Atirador · 24×24

- **Papel:** para, carrega e atira.
- **Silhueta:** losango vertical, olho grande e dois canos laterais.
- **Cores:** azul dominante, olho azul-claro/branco, pupila escura.
- **Detalhes:** losango 20×20, olho r=5, pupila 2×3, canos 4×4 nas laterais.
- **Pivô:** `(12,12)` · **Colisor:** `r=8 px`.
- **Animação:** idle 2 (pisca), charge 3 (olho acende), death 4.
- **Leitura:** o olho precisa ser o ponto mais claro do sprite.

### 5.6 NÚCLEO — Boss · 96×96

- **Papel:** clímax da run; limpa a horda e vira duelo.
- **Silhueta:** anel externo quebrado + núcleo octogonal + 4 shards orbitais.
- **Cores:** vermelho dominante, núcleo vermelho-claro, olho branco, anel escuro.
- **Detalhes:** anel r=38 com 4 gaps, núcleo r=14, olho 4×4, shards 4×12.
- **Pivô:** `(48,48)` · **Colisor:** `r=36 px`.
- **Animação:** idle 4, telegraph 4 (anel abre, olho acende), attack 4 (shards
  avançam), hit 1, death 8 (anel quebra, núcleo implode).
- **Leitura:** o anel deve funcionar como “halo” e o olho como “alvo”.

### Folhas dos inimigos

| Arquivo | Frames | Frame | Folha | FPS |
|---|---:|---:|---:|---:|
| `enemy_esporo_idle.png` | 2 | 16×16 | 32×16 | 6 |
| `enemy_esporo_move.png` | 2 | 16×16 | 32×16 | 8 |
| `enemy_esporo_hit.png` | 1 | 16×16 | 16×16 | — |
| `enemy_esporo_death.png` | 4 | 16×16 | 64×16 | 12 |
| `enemy_dardo_idle.png` | 2 | 20×20 | 40×20 | 8 |
| `enemy_dardo_move.png` | 4 | 20×20 | 80×20 | 10 |
| `enemy_dardo_hit.png` | 1 | 20×20 | 20×20 | — |
| `enemy_dardo_death.png` | 4 | 20×20 | 80×20 | 12 |
| `enemy_casco_idle.png` | 2 | 32×32 | 64×32 | 4 |
| `enemy_casco_hit.png` | 1 | 32×32 | 32×32 | — |
| `enemy_casco_death.png` | 6 | 32×32 | 192×32 | 10 |
| `enemy_celula_idle.png` | 2 | 24×24 | 48×24 | 6 |
| `enemy_celula_hit.png` | 1 | 24×24 | 24×24 | — |
| `enemy_celula_death.png` | 4 | 24×24 | 96×24 | 12 |
| `enemy_fragmento_idle.png` | 2 | 12×12 | 24×12 | 8 |
| `enemy_olho_idle.png` | 2 | 24×24 | 48×24 | 6 |
| `enemy_olho_charge.png` | 3 | 24×24 | 72×24 | 10 |
| `enemy_olho_hit.png` | 1 | 24×24 | 24×24 | — |
| `enemy_olho_death.png` | 4 | 24×24 | 96×24 | 12 |
| `enemy_nucleo_idle.png` | 4 | 96×96 | 384×96 | 6 |
| `enemy_nucleo_telegraph.png` | 4 | 96×96 | 384×96 | 10 |
| `enemy_nucleo_attack.png` | 4 | 96×96 | 384×96 | 12 |
| `enemy_nucleo_hit.png` | 1 | 96×96 | 96×96 | — |
| `enemy_nucleo_death.png` | 8 | 96×96 | 768×96 | 10 |

---

## 6. Projéteis, orbes e pickups

| Asset | Tamanho | Frames | Folha | Cor | Notas |
|---|---:|---:|---:|---|---|
| `proj_player.png` | 8×8 | 2 | 16×8 | ciano/branco | diamante com núcleo branco |
| `proj_enemy.png` | 8×8 | 2 | 16×8 | magenta | diamante com centro escuro |
| `proj_boss.png` | 12×12 | 3 | 36×12 | vermelho/branco | orbe espinhoso |
| `orbital.png` | 12×12 | 4 | 48×12 | ciano | anel + núcleo, rotação |
| `xp_orb.png` | 8×8 | 4 | 32×8 | verde | pulso; pode ser 1 frame + escala |
| `core_pickup.png` | 12×12 | 4 | 48×12 | âmbar | opcional para Núcleos |

> **Tint:** o código atual tinge projéteis e orbes com `SpriteRenderer.color`.
> Para não perder o controle de cor, desenhe esses assets em **branco/cinza** e
> deixe o código aplicar ciano, magenta, verde ou amarelo. Na folha de conceito eles
> aparecem coloridos só para visualização.

---

## 7. FX

| Asset | Tamanho | Frames | Folha | FPS | Notas |
|---|---:|---:|---:|---:|---|
| `fx_muzzle.png` | 16×16 | 4 | 64×16 | 16 | estrela de 4 pontas |
| `fx_thruster.png` | 8×16 | 3 | 24×16 | 12 | pivô topo-centro |
| `fx_hit.png` | 8×8 | 3 | 24×8 | 16 | faísca de 4 pontas |
| `fx_explosion.png` | 32×32 | 6 | 192×32 | 12 | branco/cinza para tint |
| `fx_spawn.png` | 32×32 | 4 | 128×32 | 8 | anel tracejado fecha |
| `fx_boss_telegraph.png` | 96×96 | 4 | 384×96 | 8 | opcional; pode usar o frame do boss |

**Regra de FX:** faça em branco/cinza e tinte no `SpriteRenderer`/material. Assim
uma explosão serve para todos os inimigos. Nada de blur ou alpha em pixel art;
o Bloom faz o brilho.

---

## 8. UI — ícones

### 8.1 20 upgrades · 24×24

Ícones de linha, ciano/branco, 1 px de contorno. Um acento semântico quando ajudar.

| Upgrade | Símbolo |
|---|---|
| Munição Pesada | bala + seta para baixo/peso |
| Gatilho Rápido | raio/gatilho |
| Tiro Duplo | duas balas paralelas |
| Perfuração | flecha atravessando escudo |
| Perna Leve | asa/bota |
| Blindagem | escudo |
| Ímã | ímã em U |
| Mira Fria | crosshair + floco |
| Alcance | flecha + linha pontilhada |
| Projétil Veloz | bala + linhas de velocidade |
| Reparo | chave + cruz |
| Núcleo Instável | hexágono rachado |
| Orbe Orbital | planeta + anel |
| Orbe Denso | orbe + peso |
| Placas | três placas sobrepostas |
| Nanorrobôs | robô pequeno + cruz |
| Ímã Forte | ímã + dois arcos |
| Calibre Pesado | bala grossa + marcas |
| Execução | crosshair + caveira |
| Casca Extra | casca/heart em camadas |

Formato: `ui_upgrade_<nome>.png`, 24×24, 1 frame. Se preferir uma folha:
`ui_upgrades.png` 480×24 (20 frames), fatiar 24×24.

### 8.2 Pilotos · 64×64

| Asset | Símbolo |
|---|---|
| `ui_pilot_baluarte.png` | escudo + nave larga, acento laranja |
| `ui_pilot_vetor.png` | asa/flecha, acento verde |
| `ui_pilot_lanca.png` | crosshair + agulha, acento amarelo |

Podem ser emblemas, não retratos: o jogo é abstrato; um brasão funciona melhor que
um rosto.

### 8.3 Outros

| Asset | Tamanho | Uso |
|---|---:|---|
| `ui_core.png` | 16×16 | Núcleos/meta-moeda |
| `ui_boss.png` | 24×24 | ícone da barra do boss |
| `ui_key_*.png` | 24×24 | opcional para atalhos |

---

## 9. Fundo e logo (opcional)

| Asset | Tamanho | Notas |
|---|---:|---|
| `bg_grid_tile.png` | 64×64 | tile contínuo, 1 px, alpha baixo |
| `bg_star_tile.png` | 128×128 | estrelas 1 px, opcional |
| `logo_wordmark.png` | 320×80 | wordmark pixel, opcional |

O grid e a vinheta atuais são procedurais; só troque se quiser um visual mais autoral.

---

## 10. Como montar no Piskel

1. **Novo sprite** com o tamanho exato da tabela; fundo transparente.
2. **Paleta:** importe `swarm-palette.gpl` ou cole os hex.
3. Ative a grade e desenhe a **silhueta** com uma cor sólida.
4. Faça o **contorno** de 1 px em `#02040A`.
5. Preencha com a cor dominante; adicione **highlight** no topo/esquerda e sombra
   embaixo/direita.
6. Coloque o **olho/core** com 1–2 px de branco/light.
7. Para animação: duplique o frame, mova 1 px, ajuste o olho/chama; use onion skin.
8. Configure o FPS da animação (tabela acima).
9. Exporte **PNG sprite sheet**: colunas = frames, linhas = 1, sem padding, sem fundo.
10. Salve o `.piskel` fonte em `docs/art/source/`.
11. Exporte o PNG final para `Assets/Swarm/Art/Sprites/...`.
12. Teste em 100%, 200% e 400%; depois no meio de 30–50 sprites.

### Nomes

```
ship_baluarte_idle.png
enemy_esporo_death.png
proj_player.png
fx_explosion.png
ui_upgrade_municao_pesada.png
```

Use lowercase, snake_case, sem acentos.

---

## 11. Integração na Unity

### Pastas

```
Assets/Swarm/Art/Sprites/
  Ships/
  Enemies/
  Projectiles/
  FX/
  UI/
  Background/
docs/art/source/        ← .piskel, não importado
```

### Importação

- Texture Type: **Sprite (2D and UI)**
- Sprite Mode: **Single** (1 frame) ou **Multiple** (sheet)
- Multiple: **Grid By Cell Size**, frame = tamanho da tabela, pivot center
- Pixels Per Unit: **maior dimensão do frame**
- Filter Mode: **Point**
- Compression: **None**
- Generate Mip Maps: **off**
- Alpha Is Transparency: **on**
- Wrap Mode: **Clamp**
- Max Size: **2048**
- Mesh Type: **Full Rect**
- Extrude Edges: **0**

### Cores e tint

- Se o sprite já tem cor: `EnemyData.color = Color.white` e `SpriteRenderer.color = white`.
- Se o sprite é branco: mantenha o `EnemyData.color` atual para tingir.
- **Hit flash:** o código atual faz `Lerp(baseColor, white)` no `SpriteRenderer.color`.
  Com sprite colorido e base branca, o flash não aparece. Duas saídas:
  1. overlay branco (segundo `SpriteRenderer` com o mesmo sprite, alpha 0→1); ou
  2. shader de flash aditivo com `_Flash`.
  Recomendo o overlay: não mexe no shader e funciona com qualquer sprite.
- **Critical hit:** o flash amarelo pode usar o mesmo overlay com `Color.yellow`.

### Escalas

Com PPU = frame, mantenha os valores atuais:

```
Drone      0.7
Corredor   0.55
Tanque     1.4
Divisor    1.0
Atirador   0.8
Núcleo     2.2
```

Se você normalizar tudo para PPU 32, reajuste:

| Asset | Scale para PPU 32 |
|---|---:|
| Nave 32 | 0.8 |
| Esporo 16 | 1.4 |
| Dardo 20 | 0.88 |
| Casco 32 | 1.4 |
| Célula 24 | 1.33 |
| Olho 24 | 1.07 |
| Núcleo 96 | 0.73 |
| Projétil 8 | 0.88 |
| Orbe 12 | 0.8 |
| XP 8 | 1.2 |

### Animações

O jogo atual não tem `Animator`; para usar frames, adicione um componente simples:

```csharp
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public Sprite[] frames;
    public float fps = 8f;
    public bool loop = true;

    SpriteRenderer _sr;
    float _clock;
    int _index;

    void Awake() => _sr = GetComponent<SpriteRenderer>();

    void Update()
    {
        if (frames == null || frames.Length < 2) return;

        _clock += Time.deltaTime;
        if (_clock < 1f / fps) return;
        _clock = 0f;

        _index++;
        if (_index >= frames.Length)
        {
            if (!loop) { _index = frames.Length - 1; enabled = false; }
            else _index = 0;
        }

        _sr.sprite = frames[_index];
    }
}
```

> Para 300 inimigos, **não anime todos**. Use 1 frame + bob/rotação no código para a
> horda; reserve frames para elites, boss e FX. O `Update` por inimigo também custa;
> se precisar, um sistema compartilhado resolve.

### Ordem de renderização

| Camada | Sorting Order |
|---|---:|
| Grid/fundo | -20 |
| Aviso de spawn | 1 |
| XP/pickups | 2 |
| Inimigos | 3 |
| Player | 10 |
| Projéteis | 5 |
| FX | 6 |
| UI | Screen Space |

### Atlas

Crie um **Sprite Atlas** com todos os sprites de gameplay, padding 2, sem rotação,
compressão None. Isso reduz draw calls com centenas de inimigos.

---

## 12. Prioridade de produção

### Fase 0 — MVP (1 frame por asset)

- 3 naves
- 6 inimigos
- 1 boss
- projétil do jogador, projétil inimigo, projétil do boss
- orbe orbital, XP, núcleo pickup
- muzzle, explosão, aviso de spawn

**~20 imagens.** Já dá para substituir os sprites geométricos e testar leitura.

### Fase 1 — vida

- idle 2 frames para naves, Esporo, Célula, Olho, Núcleo
- hit + death de cada inimigo
- thruster 3 frames
- muzzle/explosão completos

### Fase 2 — polimento

- charge do Olho, telegraph/attack do Núcleo
- ícones dos 20 upgrades
- emblemas dos 3 pilotos
- fundo/logo

### Fase 3 — extra

- frames de movimento do Dardo
- variações de paleta para elites
- boss com camadas separadas (anel/core/shards) para rotação

---

## 13. Checklist de qualidade

- [ ] Canvas exato da tabela.
- [ ] Fundo transparente.
- [ ] Contorno 1 px `#02040A`.
- [ ] No máximo 7 cores.
- [ ] 1 px de margem (naves/inimigos).
- [ ] Pivô centro.
- [ ] Silhueta reconhecível em preto a 1×.
- [ ] Olho/core legível a 1×.
- [ ] Sheet horizontal, sem padding.
- [ ] Nome snake_case.
- [ ] PPU = frame.
- [ ] Filter Point, Compression None, Mipmaps off.
- [ ] `EnemyData.color = white` se o sprite já tem cor.
- [ ] Escala atual conferida no Play.
- [ ] Teste com 50 inimigos na tela.
- [ ] Sprite Atlas atualizado.

---

## 14. O que evitar

- Antialias, gradientes e pixels semitransparentes.
- Contorno colorido ou grosso demais.
- Detalhe de 1 px que desaparece no meio da horda.
- Muitos frames na horda (custo e ruído visual).
- Preto puro como preenchimento; use contorno escuro.
- Mudar o colisor sem testar; o `radius = 0.5` do root já está calibrado.
- Esquecer de salvar o `.piskel` fonte.

---

**Resumo:** comece pelas 3 naves, 6 inimigos, boss e 3 projéteis. Use a folha de
conceito como referência de silhueta e a tabela como contrato de tamanho. O resto é
polimento.
