# Tutorial 05 — Meta-progressão, build e playtest

**Objetivo:** transformar o protótipo em algo que dá para mandar para alguém. 1 dia.

---

## 1. Núcleos (a moeda que nunca se perde)

Em `GameManager.EndRun`:

```csharp
int cores = Mathf.FloorToInt(Kills / 10f) + Mathf.FloorToInt(Elapsed / 60f);
SaveSystem.AddCores(cores);
```

- 100 kills + 5 min = **15 Núcleos**.
- Uma melhoria permanente custa 50 → **~3,5 runs** para a primeira.
- Esse é o ritmo: o jogador sempre sai de uma run com **algum** progresso.

`SaveSystem` usa `PlayerPrefs` (arquivo do sistema, por usuário). Para o MVP é
suficiente. Se um dia tiver 20+ campos, migre para JSON em `Application.persistentDataPath`.

### Teste do save

1. Jogue, morra, veja os Núcleos.
2. Feche a Unity e abra de novo.
3. Rode → os Núcleos continuam.

Se zerar, o `PlayerPrefs.Save()` não está sendo chamado.

---

## 2. Adicionar a loja de Núcleos (Sprint 5)

Estrutura mínima:

```
TelaMeta (Canvas)
├── TextoNucleos
├── BotaoDano     (+5% dano, 50)
├── BotaoVida     (+5% vida, 50)
└── BotaoMotor    (+5% velocidade, 50)
```

Guarde os níveis em `PlayerPrefs` (ou estenda o `SaveSystem`) e aplique no
`PlayerStats.Awake`:

```csharp
// exemplo — implemente os getters no SaveSystem
int metaDamage = PlayerPrefs.GetInt("meta.damage", 0);
damage *= 1f + 0.05f * metaDamage;
```

Cuidado: **não** aplique bônus permanente em cima de upgrade de run sem separar
`baseStats` de `runStats`. Senão, ao reiniciar, o bônus acumula infinitamente.

---

## 3. Build Windows

`File > Build Settings`:

1. **Platform:** Windows, Mac, Linux → Target: **Windows** → Architecture: `x86_64`.
2. **Player Settings**:
   - Company Name / Product Name: `SeuNome` / `SWARM`
   - Icon: 1024x1024 (obrigatório para Steam)
   - Default Screen Width/Height: 1920x1080
   - Fullscreen Mode: `Fullscreen Window`
   - **Scripting Backend:** `IL2CPP` (release) / `Mono` (teste rápido)
   - **Api Compatibility Level:** `.NET Standard 2.1`
3. **Scenes In Build:** arraste `Swarm.unity` e marque.
4. **Build** → pasta `Builds/Swarm/`.

### Antes de enviar para alguém

- [ ] Roda em 1920x1080, 1600x900 e 1280x720 sem cortar HUD.
- [ ] Alt+F4 fecha o jogo.
- [ ] `Esc` não trava (não usamos, mas teste).
- [ ] Console de build sem erro.
- [ ] `.exe` + pasta `_Data` juntos em um `.zip`.

---

## 4. Protocolo de playtest (o passo que quase todo mundo pula)

Mande para **10 pessoas** que não são seus amigos próximos. Peça 10 minutos.

### Não pergunte "o que achou?"

Perguntas abertas geram educação, não dado. Use:

1. *Em que minuto você morreu?* (dado)
2. *Você jogaria de novo agora?* (sim/não)
3. *O que você queria fazer e o jogo não deixou?* (design)
4. *Em que momento você ficou confuso?* (UI)
5. *Descreva sua build em uma frase.* (clareza)

### O que observar (sem falar nada)

| Comportamento | Significa |
|---|---|
| Reinicia em < 10 s | ✅ loop funcionando |
| Lê as 3 opções por > 4 s | ❌ opções confusas |
| Não percebe que levou dano | ❌ falta juice |
| Para de atirar (aperta botão) | ❌ o jogo não comunicou que é automático |
| Sai antes de 3 min | ❌ ritmo inicial lento |

**Meta:** 5 de 10 reiniciando sozinhos.

---

## 5. O que fazer com o feedback

Crie `docs/feedback.md` com 3 colunas: *o que ouvi*, *quantas pessoas*, *o que vou fazer*.

Regra: só aja no que apareceu **2+ vezes**. Um pedido isolado é gosto pessoal.

Nunca implemente uma sugestão que quebra um pilar do GDD. Se pedirem "modo fácil",
a resposta é ajustar a curva, não criar menu.

---

## 6. Próximos passos (ordem recomendada)

1. **Boss aos 5:00** — o clímax que a run não tem.
2. **2ª arma** — a primeira decisão de build de verdade.
3. **3 personagens** — variedade barata (só mudam números).
4. **Tela de meta-upgrade** — retenção entre sessões.
5. **Leaderboard diário** — o "só mais uma" social.

Cada item é um sprint. Não tente os cinco de uma vez.

---

## 7. Checkpoint final

- [ ] Build `.zip` roda em outra máquina.
- [ ] Save persiste entre sessões.
- [ ] 10 playtesters responderam.
- [ ] Você tem 3 GIFs de 6 s do jogo.
- [ ] Você sabe qual é o **minuto médio de morte** e ele está entre 6 e 11.

Se chegou aqui, você tem uma **vertical slice**, não um protótipo. É o que se
mostra para publisher, Steam e comunidade.
