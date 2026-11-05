# SWARM — Bullet Heaven (protótipo)

Releitura de **Space Invaders** como *bullet heaven* (estilo Vampire Survivors):
tiro automático, milhares de inimigos, upgrades a cada ~20 segundos e runs de 10 minutos.

> **Pitch:** *"Space Invaders na dose de Survivors."*

---

## O que tem aqui

| Pasta | Conteúdo |
|---|---|
| `docs/` | GDD, plano de desenvolvimento, guia de setup, arquitetura, 5 tutoriais, performance e publicação |
| `Assets/Swarm/Scripts/` | Scripts C# prontos (gameplay completo do protótipo) |
| `Assets/Swarm/Editor/` | Gerador automático de cena + assets (menu `SWARM`) |

Este repositório é um **overlay**: a pasta `Assets/Swarm` é copiada para dentro de um
projeto Unity vazio. Não copie `ProjectSettings`, `Packages` nem `Library` — a Unity cria esses.

---

## Quickstart (30 min)

O projeto **já está pronto** no Windows:

```
C:\Users\scoty\OneDrive\Documentos\Default Project\swarm
```

1. Abra o **Unity Hub** → **Add project from disk** → selecione a pasta acima.
2. Espere o primeiro import (3–6 min).
3. Menu **`SWARM > Montar Protótipo Completo`**.
4. A cena `Assets/Swarm/Scenes/Swarm.unity` abre — aperte **Play**.

Controles: **WASD/setas** move · tiro é automático · **1/2/3** escolhe upgrade · **R** reinicia.

> Para recriar do zero em outra máquina, siga [`docs/03-SETUP.md`](docs/03-SETUP.md).
> Editor usado: **6000.6.1f1** (template 2D URP oficial).

---

## Ordem de leitura

1. [`docs/01-GDD.md`](docs/01-GDD.md) — o jogo em números (leia antes de codar)
2. [`docs/03-SETUP.md`](docs/03-SETUP.md) — ambiente e primeiro Play
3. [`docs/04-ARQUITETURA.md`](docs/04-ARQUITETURA.md) — como o código se organiza
4. [`docs/05-TUTORIAL-01-MOVIMENTO.md`](docs/05-TUTORIAL-01-MOVIMENTO.md) → tutorial 05
5. [`docs/02-PLANO-DEV.md`](docs/02-PLANO-DEV.md) — sprints e cronograma
6. [`docs/10-PERFORMANCE-QA.md`](docs/10-PERFORMANCE-QA.md) e [`docs/11-PUBLICACAO.md`](docs/11-PUBLICACAO.md)

---

## Estado atual

- [x] Loop núcleo jogável (mover, atirar, matar, morrer)
- [x] Inimigos com escalonamento de dificuldade
- [x] XP, level up e escolha de 3 upgrades
- [x] Juice básico (flash, camera shake, SFX procedurais)
- [x] Meta-moeda + save (PlayerPrefs)
- [x] Gerador automático de cena e dados
- [ ] Arte definitiva
- [ ] Boss, armas e personagens (Sprint 4)
- [ ] Build publicável (Sprint 7)

> Protótipo usa **quadrados coloridos** de propósito: primeiro o loop, depois a arte.
