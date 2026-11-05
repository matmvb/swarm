# Setup do ambiente

> **Atalho:** o projeto já está **criado e pronto** em
> `C:\Users\scoty\OneDrive\Documentos\Default Project\swarm`
> com o editor **6000.6.1f1** que você já tem instalado.
> Abra pelo Unity Hub (`Add project from disk`) e pule para a seção 7.
> As seções 4–6 abaixo servem para recriar o projeto do zero em outra máquina.

> **Leia isto primeiro:** você está no **WSL2**. A Unity deve rodar no **Windows**.
> Motivos: o WSL tem só 6,7 GB de RAM (a Unity precisa de 4–8 GB), o acesso a arquivos
> em `/mnt/c` é lento e o editor depende de GPU/áudio/janela nativos. O WSL fica para
> git, edição de texto e scripts.

---

## 1. Versão da Unity

| Opção | Versão | Quando usar |
|---|---|---|
| **Já instalada na sua máquina** | **6000.6.1f1** | Use esta — o projeto já está configurado com ela |
| Patch mais recente da linha | 6000.6.7f1 (21/out/2026) | Só se quiser atualizar (opcional) |
| Conservadora | 6000.3.x (6.3 **LTS**) | Se quiser suporte longo e máxima estabilidade |

> A 6000.6.1f1 e a 6000.6.7f1 são a mesma linha 6.6; a diferença são correções.
> **Não atualize no meio de um sprint** — trave a versão no `ProjectVersion.txt`.

---

## 2. Instalar a Unity Hub (Windows)

1. Baixe em `https://unity.com/download` → **Download for Windows**.
2. Instale o **Unity Hub**.
3. Hub → **Installs** → **Install Editor** → **Archive** → procure `6000.6.7f1`.
4. Marque os módulos:
   - ✅ **Windows Build Support (IL2CPP)**
   - ✅ **Documentation**
   - ⬜ Android/iOS/Web (só depois; cada um adiciona GB)
5. Instale o **Visual Studio 2022 Community** com a carga **Game development with Unity**
   (ou VS Code + extensão C# Dev Kit — mais leve).

> Se o disco estiver apertado: a instalação base ocupa ~10 GB e o `Library` do projeto
> cresce para 2–5 GB.

### Alternativa: Linux nativo (não recomendado no seu caso)

Só faça isso se for usar Linux como sistema principal:

```bash
# baixe o AppImage do Hub em unity.com/download e:
chmod +x UnityHub.AppImage
./UnityHub.AppImage
```

---

## 3. Onde o projeto deve ficar

**No sistema de arquivos do Windows**, nunca em `\\wsl.localhost\...` (a Unity recompila
o projeto inteiro a cada salvamento e fica 5–10x mais lento).

```
C:\Dev\Swarm\            ← projeto Unity
├── Assets\
│   └── Swarm\           ← copiado deste repositório
├── Packages\
├── ProjectSettings\
└── Library\             ← gerado pela Unity (não versionar)
```

### Copiar deste repositório (do WSL, com o projeto já criado)

```bash
# crie o projeto na Unity primeiro em C:\Dev\Swarm, depois:
mkdir -p /mnt/c/Dev/Swarm/Assets
cp -r /home/scoty/swarm/Assets/Swarm /mnt/c/Dev/Swarm/Assets/
```

Ou no **PowerShell**:

```powershell
robocopy "\\wsl.localhost\Ubuntu\home\scoty\swarm\Assets\Swarm" "C:\Dev\Swarm\Assets\Swarm" /E
```

> Se `\\wsl.localhost\Ubuntu` não existir, rode `wsl -l -v` e use o nome exato da distro.

---

## 4. Criar o projeto

1. Unity Hub → **New project**.
2. Template: **2D (URP)** (aparece como *Universal 2D*).
3. Nome: `Swarm` · Local: `C:\Dev\Swarm` · **Create**.
4. Espere o primeiro import (2–5 min).

### Pacotes

`Window > Package Manager`:

| Pacote | Versão | Por quê |
|---|---|---|
| **Input System** | mais recente | leitura de teclado/gamepad |
| Universal RP | já vem no template | render 2D |
| 2D Sprite / Tilemap | já vem | sprites |
| TextMeshPro (UGUI) | já vem | não usamos no MVP |

Se instalar o Input System, a Unity pergunta se quer reiniciar e mudar o
*Active Input Handling* → **Yes**.

---

## 5. Configurações do projeto (fazer uma vez)

`Edit > Project Settings`:

| Tela | Ajuste | Valor |
|---|---|---|
| Player | **Active Input Handling** | `Input System Package (New)` |
| Player | Company / Product Name | seu nome / `SWARM` |
| Player > Resolution | Fullscreen Mode | `Fullscreen Window` |
| Quality | VSync Count | `Every V Blank` |
| Time | Fixed Timestep | `0.02` |
| Editor | Enter Play Mode Options | **desligado** (evita bugs de estado estático) |

`Edit > Project Settings > Graphics`: confirme que o **URP Asset** está atribuído
(o template 2D já faz isso).

> **Color Space** deve estar em **Linear** (padrão do URP). Se estiver Gamma, o neon
> fica lavado.

---

## 6. Instalar o código

1. Copie `Assets/Swarm` para dentro de `C:\Dev\Swarm\Assets\`.
2. Volte para a Unity e espere compilar (`Compiling…` no canto inferior direito).
3. **Console deve estar limpo.** Se houver erro vermelho, veja a seção 8.

---

## 7. Gerar o protótipo

No menu superior:

```
SWARM > Montar Protótipo Completo
```

Ele cria automaticamente:

- `Assets/Swarm/Art/white.png` (sprite branco reutilizável)
- `Assets/Swarm/Data/Enemies/*.asset` (4 inimigos)
- `Assets/Swarm/Data/Upgrades/*.asset` (12 upgrades)
- `Assets/Swarm/Scenes/Swarm.unity` (cena completa, já ligada)
- Camera, Player, HUD, Spawner, templates e pools

Depois: abra `Assets/Swarm/Scenes/Swarm.unity` (se não abrir sozinha) e aperte **Play**.

### Checklist do primeiro Play

- [ ] WASD move a nave
- [ ] Ela atira sozinha no inimigo mais próximo
- [ ] Inimigos morrem, piscam e soltam orbes verdes
- [ ] Coletar orbes sobe a barra de XP
- [ ] Ao subir de nível, o tempo congela e aparecem 3 opções
- [ ] `1/2/3` aplica o upgrade e o jogo volta
- [ ] Ao morrer, aparece a tela de fim com tempo, kills e Núcleos
- [ ] `R` reinicia

---

## 8. Erros comuns

| Sintoma | Causa | Solução |
|---|---|---|
| `The type or namespace name 'InputSystem'...` | pacote não instalado | Package Manager → Input System |
| `InvalidOperationException: You are trying to read Input using the UnityEngine.Input class` | Active Input Handling = New e código legado | O `InputReader` já trata isso; confirme que o pacote está instalado |
| Sprites rosa/roxo | material do URP quebrado | `Edit > Rendering > Materials > Convert...` ou recriar o material com `Sprite-Lit-Default` |
| Nada aparece no Play | cena errada aberta | abra `Assets/Swarm/Scenes/Swarm.unity` |
| Texto do HUD invisível | fonte embutida não encontrada | recrie a HUD pelo menu; se persistir, troque a fonte no Inspector |
| Personagem atravessa tudo | colisor do player é trigger de propósito | dano usa `OnTriggerStay2D`; não mude para sólido |
| `NullReferenceException` no `Sfx` | objeto `Sfx` não existe na cena | rode o gerador de novo |
| Level up não abre | `HudController` sem referência | rode o gerador de novo (ele religa tudo) |

> Se o gerador falhar no meio, apague `Assets/Swarm/Data`, `Assets/Swarm/Scenes` e
> `Assets/Swarm/Art` e rode de novo. Ele recria tudo.

---

## 9. Versionamento

```bash
cd /home/scoty/swarm
git init
git add .
git commit -m "chore: scaffold inicial do SWARM"
```

Na Unity: `Edit > Project Settings > Version Control` → Mode: **Visible Meta Files**
(e confirme que `.gitignore` cobre `Library/`, `Temp/`, `Logs/`).

**Nunca versione:** `Library/`, `Temp/`, `Logs/`, `Build/`, `*.csproj`, `*.sln`.

---

## 10. Atalhos que você vai usar todo dia

| Atalho | Ação |
|---|---|
| `Ctrl+P` | Play / Stop |
| `Ctrl+Shift+P` | Pause |
| `Ctrl+Alt+P` | Pause e avança 1 frame |
| `F` | enquadrar objeto selecionado na Scene |
| `Ctrl+Shift+F` | alinhar objeto à câmera |
| `Ctrl+D` | duplicar |
| `Ctrl+Shift+C` | abrir Console |
