# Arte — SWARM

Brief completo: [`../26-ART-BRIEF-SPRITES.md`](../26-ART-BRIEF-SPRITES.md).

| Arquivo | Uso |
|---|---|
| `concept-sheet.png` | folha visual com naves, inimigos, boss, projéteis e paleta |
| `concept-sheet.html` | fonte editável da folha (abra no navegador) |
| `swarm-palette.gpl` | paleta para importar no Piskel/GIMP |
| `source/` | salve aqui os arquivos `.piskel` (não vão para a Unity) |

## Fluxo

1. Abra a folha de conceito.
2. Escolha um asset e crie no Piskel no tamanho exato da tabela.
3. Use a paleta `swarm-palette.gpl`.
4. Exporte PNG transparente, tira horizontal, sem padding.
5. Salve o `.piskel` em `source/`.
6. Copie o PNG para `Assets/Swarm/Art/Sprites/<pasta>/`.
7. Importe com Point, Compression None, Mipmaps off e PPU = frame.

> Naves e inimigos: 1 px de margem e contorno `#02040A`.
> Projéteis e FX: branco/cinza para o código tingir.
