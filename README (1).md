
# Projekt MUD - Dokumentace pro testování



---

## 1. Jak spustit Server
Server musí běžet jako první, aby se k němu mohli klienti připojit.

1. Otevřete složku s projektem.
2. Přejděte do složky se spustitelným souborem (např. `bin/Debug/net8.0/`).
3. Spusťte soubor serveru (např. `Pv_projekt.exe`).
4. V konzoli serveru by se měla objevit zpráva: **"=== Server startuje ==="** nebo **"MUD server byl spusten."**
5. **Důležité:** Server standardně naslouchá na portu **65525**.

---

## 2. Jak se připojit přes PuTTY (Návod pro testera)
Pro nejlepší zážitek doporučujeme otevřít dvě okna PuTTY a přihlásit se dvěma různými jmény.

1. Spusťte program **PuTTY**.
2. Do pole **Host Name (or IP address)** napište: ip adresu vasho pocitace , kterou muzte zjistit pomoci konzole a komandu ipconfig
3. Do pole **Port** napište: `65525`
4. Jako **Connection type** vyberte: `Raw`
5. Klikněte na tlačítko **Open**.

---

## 3. Registrace a Přihlášení
Hra vás po připojení vyzve k zadání jména a hesla.

- Pokud jméno neexistuje, hra vás automaticky provede registrací (vytvoří nový účet).
- Pokud chcete použít připravený účet:
  - **Jméno:** `save_hrac`
  - **Heslo:** `Save123`

---

## 4. Základní herní příkazy
Veškeré akce zadávejte do PuTTY terminálu:

- `pomoc` - Zobrazí seznam všech dostupných příkazů.
- `rozhledni` - Popíše místnost, ukáže věci na zemi a ostatní hráče.
- `jdi [sever/jih/vychod/zapad]` - Pohyb mezi lokacemi.
- `status` - Zobrazí HP, poškození, obranu a počet duší.
- `vezmi [id_predmetu]` - Sebere věc ze země (např. `vezmi rezava_dyka`).
- `inventar` - Seznam věcí, které nesete.
- `utoc [npc]` - Zahájí souboj (např. `utoc nizsi_demon`).
- `krik [zprava]` - Pošle zprávu všem lidem v celém Pekle.
- `ukonci` - Uloží postup a bezpečně vás odpojí.

---

## 5. Herní mechaniky k otestování
- **Soubojový bonus:** Pokud máte v inventáři dýku, příkaz `utoc utok` automaticky přičte její sílu k vašemu poškození.
- **Zamčené lokace:** Do VIP Kasina se nedostanete bez předmětu `vip_karta`.
- **Obchodování:** U Obchodníka duší použijte `mluv obchodnik_dusi` pro start dialogu a `kup lektvar` pro nákup.
- **Hazard:** V Kasinu vyzkoušejte příkaz `blackjack` pro hru o duše.


