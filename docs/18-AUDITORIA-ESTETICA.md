# Auditoria estética

## Corrigido nesta rodada (build 0.9.2)

| Problema | Correção |
|---|---|
| **Tiro saía da lateral** | O `Muzzle` agora é filho do `Visual`, em `(0, 0.55)` — na ponta do triângulo |
| **Nave sempre olhando para cima** | `PlayerVisual` gira o **filho Visual** para o inimigo mais próximo (ou para a direção do movimento), a 720°/s |
| **Fundo chapado** | Grid neon de células de 4 unidades, seguindo a câmera em passos de 4 (parece rolar com o mundo) |
| Colisor girando junto | O corpo/colisor ficam no root; **só o Visual gira** (sem efeito colateral na física) |
| Cor do piloto sumindo | `CharacterSelect` e `OrbitalWeapon` agora buscam o sprite nos filhos |

Detalhes técnicos:

- `PlayerVisual` usa `LateUpdate`, então roda depois da lógica do jogador.
- O grid tem `sortingOrder -100` e alfa 0,35; fica atrás de tudo e não estoura o bloom.
- A textura do grid é 128×128, linha de 1 px a cada 32 px, PPU 8 → célula de 4 unidades.

---

## Próximas melhorias (priorizadas)

| # | Item | Por que | Esforço |
|---|---|---|---|
| 1 | **Muzzle flash** | hoje o tiro “aparece” sem origem; um flash de 0,08 s vende o disparo | baixo |
| 2 | **Fonte pixel no HUD** | Arial quebra o estilo neon da capa | médio |
| 3 | **Rastro do propulsor** | dá leitura de movimento e velocidade | baixo |
| 4 | **Rastro do projétil** | ajuda a ler a direção no meio do caos | baixo |
| 5 | **Aviso de spawn** | inimigos surgem do nada; um anel de aviso no ponto de nascimento | baixo |
| 6 | **Números de dano** | satisfação por acerto (pool + limite para não alocar) | médio |
| 7 | **Animação de morte** | escala/fade em vez de sumir seco | baixo |
| 8 | **Intro do boss** | zoom curto + shake no nascimento do Núcleo | baixo |
| 9 | **Consistência da UI** | barras, painéis e textos no mesmo tom neon | baixo |
| 10 | **Zoom punch no level up** | ênfase na pausa de escolha | baixo |

---

## O que **não** vale a pena agora

- Shader customizado de glow: o bloom do URP já resolve.
- Iluminação 2D: custa performance e não muda a leitura.
- Fundo com parallax de várias camadas: o grid já dá a sensação de movimento.
