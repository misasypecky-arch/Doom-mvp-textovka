# Dokumentace a Vysvětlení Kódu - MUD Projekt



## 1. Architektura a Průběh Příkazu
Hra funguje na principu **Command Patternu** (i když v jednodušší formě switch-case).
- Každý řetězec, který přijde od klienta, se nejdříve očistí (`Trim`, `ToLower`).
- První slovo určí **akci**, zbytek slov jsou **argumenty**.

## 2. Klíčové Mechaniky

### A. Pohyb a Zámky (`CmdJdi`)
Logika kontroluje, zda směr (sever, jih...) existuje v seznamu východů aktuální místnosti.
- **Zamykání:** Pokud má místnost příznak `IsLocked = true`, kód projde hráčův `Inventory.Items` a hledá ID klíče (např. `vip_karta`). Pokud ho hráč nemá, nepustí ho dál.

### B. Soubojový Systém (`ProvedSoubojovyTah`)

- **Detekce zbraní:** Kód dynamicky kontroluje inventář. Pokud položka obsahuje slova jako *"dyka"*, *"mec"* nebo *"sekera"*, automaticky přičte +5 k útoku.
- **NPC Turn:** Pokud NPC přežije hráčův útok, ihned útočí zpět. Pokud hráč zemře (HP <= 0), je teleportován do startovní místnosti a jeho HP se resetuje.

### C. Dialogy a Obchodování (`ZpracujDialog`)
Když hráč napíše `mluv [npc]`, přepne se do stavu `IsInDialog`. V tomto stavu příkazy nejdou do hlavního procesoru, ale do dialogové metody.
- Umožňuje to nákup předmětů (např. `lektvar`) výměnou za "duše" (Money).

### D. Blackjack (Kasino)
Jednoduchá implementace hazardu. Hráč sází 10 duší. Pokud má součet karet vyšší než dealer (ale ne přes 21), vyhrává dvojnásobek.

## 3. Původní Komentáře (Odstraněno z kódu)

- *Pokud je hráč v dialogu, vstup řeší dialogový systém* - Důležité pro odlišení herních příkazů od povídání.
- *Bonus dýky se automaticky přičte* - Vysvětlení, proč hráč dává větší damage, i když nikde nic "nevybavil".
- *Uloží postup a bezpečně odpojí* - Funkce příkazu `ukonci`.




