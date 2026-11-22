# Página do itch.io — SWARM

Tudo o que você precisa para publicar a demo. Copie e cole no itch.io.

---

## Ficha do projeto

| Campo | Valor |
|---|---|
| **Title** | SWARM |
| **Tagline** | Space Invaders na dose de Survivors. |
| **Classification** | Game |
| **Kind of project** | Downloadable |
| **Pricing** | `$0 or donate` |
| **Platforms** | Windows |
| **Status** | In development (demo) |
| **Tags** | survivors-like, bullet-heaven, roguelite, arcade, 2d, neon, singleplayer, short-session |

---

## Descrição curta

> Um bullet heaven neon: sobreviva 10 minutos, escolha um upgrade a cada nível e derrube o Núcleo.

---

## Descrição completa

**SWARM** é um survivor-like 2D onde você não aperta "atirar" — o sistema atira por você.
Sua decisão é **para onde ir** e **qual upgrade escolher**.

Cada run dura até 10 minutos. Ao subir de nível (no começo, a cada ~20 segundos) o jogo congela e oferece 3 upgrades.
Às 5:00 o **Núcleo** nasce, limpa a horda e transforma a run em um duelo.
Sobreviva até 10:00 para vencer.

### O que tem na demo

- **6 inimigos** com papéis diferentes (horda, corredor, tanque, divisor, atirador e boss)
- **Boss com padrão**: rajada radial + investida, com telegrafo legível
- **20 upgrades** com pré-requisitos, efeitos duplos e limites
- **3 pilotos**: Tanque, Veloz e Atirador
- **2 armas**: tiro automático e orbes orbitais
- **Meta-progressão**: gaste Núcleos em bônus permanentes
- **Trilha e efeitos gerados por código** (sem assets externos)

### Controles

| Tecla | Ação |
|---|---|
| ENTER | jogar (menu principal) |
| WASD / setas | mover |
| 1 / 2 / 3 | escolher piloto / upgrade |
| 4 / 5 / 6 | comprar melhorias permanentes |
| O | opções (volume, tela cheia, reset) |
| ESC | pausar / continuar |
| R | reiniciar |
| F1 | overlay de debug |
| F12 | screenshot |

### Isso é um protótipo

Estou testando **ritmo, clareza e sensação de poder**. Se algo parecer injusto, confuso ou
repetitivo, escreva nos comentários — é exatamente esse retorno que estou buscando.

Obrigado por jogar. 🎮

---

## Upload

| Item | Especificação |
|---|---|
| Build | `Builds/SWARM-windows.zip` (0.9.8, ~39 MB) |
| Cover | 630 × 500 |
| Screenshots | 5 × 1920 × 1080 |
| GIFs | 3 (6–10 s cada) |
| Trailer | 30 s, 1920 × 1080 |

### Como capturar (F12)

Na build, aperte **F12**. Os PNGs vão para:

```
%USERPROFILE%\AppData\LocalLow\Scoty\SWARM\shots
```

Sugestão de 5 prints:

1. Tela de escolha de piloto (mostra a loja de Núcleos)
2. Horda cheia com o jogador no meio
3. Level up com 3 opções
4. Boss com o halo vermelho do telegrafo
5. Tela de vitória ou de fim com a build

---

## Trailer de 30 s (roteiro)

| Tempo | Imagem | Áudio |
|---|---|---|
| 0–3 s | horda cheia + level up estourando | música + som de level up |
| 3–10 s | gameplay cru: desviar, atirar, matar | música |
| 10–18 s | 3 level ups rápidos (mostra a build) | sons de upgrade |
| 18–25 s | boss: telegrafo → rajada → morte | telegrafo + impacto |
| 25–30 s | logo SWARM + "Demo grátis no itch.io" | música fecha |

Sem narração. Cortes a cada 1,5–2 s. Legenda grande.

---

## Checklist antes de publicar

- [ ] 5 screenshots em 1920×1080
- [ ] 3 GIFs de 6–10 s
- [ ] Trailer de 30 s
- [ ] ZIP da build testado em outra máquina
- [ ] Descrição e tags preenchidas
- [ ] Página publicada como **Restricted** primeiro
- [ ] Link enviado para 5 pessoas
- [ ] Depois de 3 dias de feedback, tornar **Public**

> O save da build é separado do editor. A primeira run começa com 0 Núcleos e sem meta.

---

## Status para publicação (0.9.8)

| Item | Status |
|---|---|
| Build Windows | ✅ `Builds/Windows` + `Builds/SWARM-windows.zip` |
| Ícone do executável | ✅ 1024 aplicado nos 8 slots |
| Cover 630×500 | ✅ `docs/press/cover-630x500.png` |
| Bloom + vinheta na build | ✅ perfil e shaders corrigidos |
| Screenshots | ⬜ 0 de 5 |
| GIFs | ⬜ 0 de 3 |
| Trailer | ⬜ não gravado |
| Teste em outra máquina | ⬜ pendente |

**Bloqueio atual:** a build está tecnicamente pronta, mas a página ainda não tem screenshots, GIFs nem trailer. Para publicar, faltam esses assets e o teste em uma máquina limpa.
