# Auditoria de UI/UX e visual

## 1. Tipografia

| Item | Estado | Avaliação |
|---|---|---|
| Fonte do jogo | `LegacyRuntime.ttf` (Arial embutida) | Legível e zero asset, mas **genérica** e sem relação com a capa pixel |
| Títulos | Bold, maiúsculas, "S W A R M" com espaçamento manual | Melhorou a hierarquia, mas não é uma fonte display |
| Tamanhos | 72 (título), 60 (pausa), 46 (painéis), 30–34 (corpo), 18–24 (secundário) | Hierarquia clara |
| Recomendação | Importar uma **TTF pixel/OFL** (ex.: Press Start 2P) e usar só nos títulos | Alto impacto, custo baixo |

> O `Text` legado não tem *letter spacing*; o espaçamento do título é feito com espaços.
> Uma fonte pixel resolve título e HUD de uma vez.

---

## 2. Cores

Paleta única em `UIPalette`:

| Papel | Cor | Uso |
|---|---|---|
| Accent | `#4DFFE0` | título, timer, nível, teclas |
| Accent soft | `#A6FFF2` | textos de destaque |
| Text | `#EBF7FF` | corpo |
| Muted | `#8FA3B8` | ações secundárias |
| Danger | `#FF4D59` | vida, boss, dano |
| Warning | `#FFBF4D` | toast do boss |
| Exp | `#66FF80` | barra de XP |
| Panel | `#05070F` 94% | painéis |

Antes: branco puro em quase tudo. Agora: **um acento, um corpo, um muted** e cores semânticas.
Contraste texto/fundo > 7:1 (AAA) nos textos principais.

---

## 3. Fluxo e menus

```
MENU PRINCIPAL
  [ENTER] Jogar ──► ESCOLHA DE PILOTO (1/2/3 + loja 4/5/6)
  [O] Opções          └── [1/2/3] escolhe ──► RUN
  [ESC] Sair                                  │
                                        [ESC] PAUSA
                                          ├── [ESC] continuar
                                          ├── [O] opções
                                          ├── [R] reiniciar (pula o menu)
                                          ├── [M] menu principal
                                          └── [Q] sair
```

- **Opções** acessível do menu principal, da pausa e da escolha de piloto.
- **Reiniciar** usa `AutoStartRun`: recarrega a cena e cai direto nos pilotos.
- **Menu principal** recarrega a cena sem auto-start.
- **Game over**: `R` reinicia direto; a tela mostra tempo, kills, nível, Núcleos e tempo do boss.

### Teclas

| Contexto | Tecla | Ação |
|---|---|---|
| Menu | Enter | Jogar |
| Menu | O | Opções |
| Menu | Esc | Sair |
| Run | Esc | Pausar |
| Pausa | Esc | Continuar |
| Pausa | R / M / Q | Reiniciar / Menu / Sair |
| Run | 1/2/3 | Escolher upgrade |
| Run | F1 / F2 / F3 / F12 | Debug / boss / orbes / screenshot |

---

## 4. Acessibilidade

- ✅ 100% teclado; nenhuma ação depende de mouse.
- ✅ Toda ação tem **rótulo textual** (não depende só de cor).
- ✅ Tamanho mínimo 18 px; corpo 24–34.
- ✅ Contraste alto (texto claro em fundo quase preto).
- ✅ Flash de dano é um fade de 0,5 s (não pisca).
- ⚠️ Sem opção de remapear teclas (futuro).
- ⚠️ Sem opção de reduzir flashes/efeitos (futuro).

---

## 5. Escala e resolução

- CanvasScaler: 1920×1080, match 0.5.
- Testar em 1280×720 e 2560×1440.
- O grid e os sprites são world-space; não sofrem com a escala da UI.

---

## 6. Assets

| Asset | Estado |
|---|---|
| Formas (nave, inimigos, projéteis, orbes) | Procedurais, com AA |
| Grid de fundo | Procedural |
| Capa 630×500 | Gerada |
| Fonte | Embutida (recomendo pixel TTF) |
| Ícone do executável | **Gerado (1024) e aplicado no BuildScript** |
| Rastro do projétil | TrailRenderer com material Sprite-Unlit (verificar na build) |

### Recomendações

1. **Fonte pixel** para títulos.
2. **Testar o TrailRenderer** na build; se sumir, trocar por material de partícula.
3. **Painel de controles** no menu (lista de teclas) — hoje as teclas estão nos próprios botões.
4. **Ícones 16–1024**: o BuildScript preenche os 8 slots do Standalone com o PNG 1024; a Unity reduz.

---

## 7. O que mudou nesta rodada

- Menu principal com título, tagline, Jogar/Opções/Sair e versão.
- Pausa com Continuar/Opções/Reiniciar/Menu/Sair.
- Opções acessível dos três contextos.
- Paleta única e hierarquia tipográfica em todos os painéis.
- HUD com cores semânticas e dica de pausa.

---

## 8. Correções de build (0.9.8)

- **Shader stripping do URP desligado** para pós-processamento e variantes 2D. Antes, o build removia Bloom, vinheta e todos os passes; o log do player confirmava `shader ... stripped from the build`.
- **NeonProfile regenerado**: Bloom e Vignette agora são sub-assets reais do perfil. Antes os dois componentes estavam com `fileID: 0`, então nem no editor o efeito existia.
- **ZIP de release limpo**: sem `SWARM_BackUpThisFolder_ButDontShipItWithYourGame`, sem PDBs, com `SWARM.exe` na raiz.
- Versão **0.9.8** no executável, no menu e no `ProjectSettings`.
