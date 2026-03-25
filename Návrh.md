# Navrh
### rozdeleni
* Martin - mechaniky , struktura, 
* Michal - mapa, items , mistnost.json , pripadna vypomoc , mistnost , klient

### infrastruktura

I1 – Načítání herního světa z externích souborů
Herní svět se musí načítat z externích resource souborů, například ve formátu JSON, XML nebo jiném vhodném formátu dle vaší volby.

To zahrnuje zejména:

místnosti,
předměty,
NPC postavy,
dialogy,
případně další herní data.
Požadavky:

Server nesmí mít herní data napevno v kódu.
Změna herního světa nesmí vyžadovat rekompilaci aplikace.
Použitý formát souborů musí být popsán v dokumentaci.
I2 – Logování
Server musí zaznamenávat důležité události, například:

připojení a odpojení hráče,
zadané příkazy,
chyby a výjimečné stavy.
Požadavky:

Logy se ukládají do souboru.
Každý záznam obsahuje časovou značku.
Konkrétní formát logu je na vašem rozhodnutí.
Logování nesmí výrazně zpomalovat obsluhu klientů.
I3 – Přihlášení a persistence hráče
Hra musí podporovat základní systém uživatelských účtů.

Požadavky:

Hráč se přihlašuje pomocí jména a hesla.
Nový hráč si může vytvořit účet.
Při odpojení se uloží stav hráče, například:
aktuální pozice v herním světě,
inventář,
postup hrou.
Při dalším přihlášení se uložený stav obnoví.
Hesla nesmí být ukládána v čistém textu.
I4 – Vlastní klientský program
Součástí řešení bude vaše vlastní klientská aplikace.

Požadavky:

Klient může být konzolový nebo mít jednoduché GUI.
Klient se připojí k serveru, zobrazuje výstup a odesílá příkazy od uživatele.
PuTTY nebo jiný TCP klient může sloužit pouze pro testování.
Finální odevzdané řešení musí obsahovat vlastního klienta.


### Návrh architektury




### mechaniky
M1 – Komunikace mezi hráči
Příkaz rekni odešle zprávu všem hráčům ve stejné místnosti.
Příkaz krik odešle zprávu všem připojeným hráčům bez ohledu na místnost.
Ostatní hráči vidí oznámení, když někdo vstoupí do jejich místnosti nebo ji opustí.
Volitelně lze doplnit soukromé zprávy mezi hráči.

M2 – Souboj s NPC
NPC má atributy jako životy, útok a případně obranu.
Hráč může NPC napadnout a probíhá souboj podle pravidel vašeho návrhu.
Po poražení NPC hráč získá odměnu.

M4 – Obchodování
Speciální NPC obchodují s hráčem za herní měnu.
Hráč získává měnu aktivitami ve hře.
Nabídka a ceny jsou načítány z externích souborů.

M8 – Používání předmětů
Předměty mají definovaný účinek při použití.
Hráč je může aktivovat příkazem.
Účinky a pravidla použití jsou načítány z externích souborů.

M9 – Generování herního světa
Část nebo celý herní svět je generován procedurálně při spuštění serveru.
Generované místnosti mají smysluplné propojení.
Musí být jasné, co je generované a co pevně definované.

M10 – Úkoly / questy
Hráč může získávat a plnit jednoduché úkoly.
Úkol může spočívat například v doručení předmětu, poražení NPC nebo nalezení konkrétní lokace.
Po splnění úkolu hráč získá odměnu.
Stav úkolu se ukládá jako součást postupu hrou.

M11 – Zamčené místnosti a klíče
Některé místnosti nebo průchody nejsou přístupné bez splnění podmínky.
Podmínkou může být například vlastnictví klíče, splnění úkolu nebo použití konkrétního předmětu.
Hráč musí dostat srozumitelnou informaci, proč nemůže pokračovat.

M13 - ruska ruleta - dabelskly revolver  - mistnost casino  - mechanika faith - stat ktery podle sve hodnoty urci  bonusy i debuffy 


### Náčrt herního světa
mapa bude ve stylu grid 6x6
hrac je v pekle a snazi se dostat ven, 
hrac se dostane ven potom co porazi samotnho satana 
behem sve cesty se hrac bude utkavav s demony i s nimi obchodovat 
dale bude moct se dostat do pekelneho kasina 


