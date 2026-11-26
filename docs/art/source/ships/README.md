# SWARM 0.9.11 — ship source

Estes arquivos documentam e permitem reproduzir o passe de redesign das naves
Baluarte, Vetor e Lança.

## Ferramenta

Não havia Aseprite/Piskel disponível no ambiente. A fonte real deste passe é o
script Python `refine_ships.py`, que trabalha pixel a pixel com Pillow, sem
anti-aliasing, sem gradientes e com a paleta explícita do SWARM.

- `base/` — PNGs 0.9.10 usados como silhueta de referência, incluindo `ui_pilot_*`.
- `export/` — saída do script (idle, hit, death, thruster e portraits).
- `preview/` — folhas de comparação geradas durante o passe.

## Executar

```bash
python3 docs/art/source/ships/refine_ships.py
```

O script escreve em `export/`; ele **não** altera `Assets/` automaticamente.
A integração é feita copiando os PNGs aprovados para:

- `Assets/Swarm/Art/Sprites/Ships/`
- `Assets/Swarm/Art/Sprites/UI/`

Os `.meta` existentes devem ser preservados (mesmas dimensões, frames e nomes).

## Limitação

O script é uma fonte procedural editável; não é um arquivo `.aseprite` ou
`.piskel`. A silhueta foi preservada de propósito, conforme a auditoria 0.9.11.

Os PNGs de `base/` também preservam os assets 0.9.10 das naves e portraits para
recuperação caso o redesign precise ser comparado ou revertido.
