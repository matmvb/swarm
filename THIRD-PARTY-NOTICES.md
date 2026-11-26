# SWARM — Third-party notices

SWARM is proprietary software and art. See `LICENSE`.

This file lists the main third-party components used by the project. It is not
an exhaustive dependency audit; package versions are listed in
`Packages/manifest.json` and `Packages/packages-lock.json`.

## Press Start 2P font

- **Asset:** `Assets/Swarm/Art/Fonts/PressStart2P-Regular.ttf`
- **Copyright:** Copyright 2012 The Press Start 2P Project Authors
  (cody@zone38.net), with Reserved Font Name “Press Start 2P”.
- **License:** SIL Open Font License, Version 1.1.
- **Full license text:** `Assets/Swarm/Art/Fonts/OFL.txt`
- **Official project:** https://github.com/google/fonts/tree/main/ofl/pressstart2p

The font is embedded in the game for titles, HUD, and UI. The OFL permits use
and embedding in games, including commercial games, provided the font is not
sold by itself and the license/copyright notices are preserved. When distributing
a build, include this notice and `OFL.txt` with the build.

## Unity

SWARM is made with Unity 6.6 and Unity packages (URP 2D, UGUI, Input System,
2D Animation, Timeline, and others).

- Unity Engine and Unity packages are governed by Unity's terms:
  https://unity.com/legal
- The repository does not redistribute the Unity Editor.
- Package-specific licenses are available in the Unity Package Manager and in
  the package cache.

## Site and repository

- GitHub Pages hosts the public project pages.
- The feedback form uses Formspree when configured by the site.
- These services have their own terms and privacy policies; see
  `docs/privacidade.html`.

## Distribution note

If you publish a Windows build or a ZIP, include:

1. `LICENSE`
2. `THIRD-PARTY-NOTICES.md`
3. `Assets/Swarm/Art/Fonts/OFL.txt`

The in-game credits page can link to the public license page:
https://matmvb.github.io/swarm/licenca.html
