# SWARM — como abrir (Windows)

Projeto Unity **completo** (não é só uma pasta de scripts).

```
C:\Users\scoty\OneDrive\Documentos\Default Project\swarm
```

| Item | Valor |
|---|---|
| Editor | **Unity 6000.6.1f1** (já instalado na sua máquina) |
| Template | 2D (URP) oficial — URP 17.6.0, Input System 1.19.0 |
| Versão | **0.9.8** (playtest) |
| Cena do jogo | gerada pelo menu `SWARM` em `Assets/Swarm/Scenes/Swarm.unity` |

---

## 1. Abrir o projeto

1. Abra o **Unity Hub**.
2. **Projects** → **Add** → **Add project from disk**.
3. Selecione a pasta `swarm` acima.
4. Clique no projeto para abrir.

Na primeira abertura a Unity vai:
- baixar/importar os pacotes (URP, Input System) — 3 a 6 min;
- criar a pasta `Library` (normal, não versione).

> Se o Hub não listar a pasta, verifique se existe
> `ProjectSettings\ProjectVersion.txt` — ele está lá.

---

## 2. Gerar o jogo

A cena **já foi gerada e validada**. Para jogar:

1. Abra `Assets/Swarm/Scenes/Swarm.unity`.
2. Aperte **Play**.

Se alterar o código e quiser recriar tudo: **`SWARM` → `Montar Protótipo Completo`**.
Para conferir as referências: **`SWARM` → `Validar Cena do Protótipo`**.

### Controles

| Tecla | Ação |
|---|---|
| `WASD` / setas | mover |
| `ENTER` | jogar (menu principal) |
| `1` / `2` / `3` | escolher piloto ou upgrade |
| `4` / `5` / `6` | comprar melhorias permanentes |
| `O` | opções |
| `ESC` | pausar / continuar |
| `R` | reiniciar |
| `F1` | overlay de debug (FPS, inimigos, stats) |
| `F3` | (editor) adiciona um orbe orbital |
| `F12` | screenshot |

---

## 3. Se algo der errado

| Problema | Solução |
|---|---|
| Console com erro vermelho no primeiro open | `Assets > Reimport All` e espere terminar |
| Menu `SWARM` não aparece | espere a compilação terminar (canto inferior direito) |
| Sprites rosa/roxo | `Edit > Rendering > Materials > Convert All Built-in Materials to URP` |
| "InputSystem" não encontrado | Package Manager → Input System → Install |
| Erro no `SwarmSceneBuilder` | apague `Assets/Swarm/Data`, `Scenes` e `Art` e rode o menu de novo |
| Tela preta no Play | abra a cena `Assets/Swarm/Scenes/Swarm.unity` |

---

## 4. Aviso sobre OneDrive

O projeto funciona no OneDrive, mas a pasta `Library` (gerada pela Unity) tem
**dezenas de milhares de arquivos** e pode travar a sincronização.

Recomendações:
- **Pause a sincronização do OneDrive** enquanto estiver com a Unity aberta; ou
- mova o projeto para `C:\Dev\swarm` e deixe uma cópia no OneDrive só para backup.

O `.gitignore` já exclui `Library/`, `Temp/`, `Logs/` e `Builds/` do Git.

---

## 5. Documentação

Tudo em `docs/`:

1. `01-GDD.md` — design e números
2. `02-PLANO-DEV.md` — sprints
3. `03-SETUP.md` — setup detalhado
4. `04-ARQUITETURA.md`
5. `05` a `09` — tutoriais
6. `10-PERFORMANCE-QA.md`
7. `11-PUBLICACAO.md`
