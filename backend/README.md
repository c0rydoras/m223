# Bank Backend

Stelle sicher, dass du .NET 9 installiert hast.

## Setup

Kopiere (in Bank.Cli und Bank.Web) `appsettings.sample.json` in `appsettings.json` und passe den ConnectionString an.

### Teilaufgabe 1 - Analyse

Übertrag von Konto Quelle auf Konto Ziel:

- Betrag validieren -> Betrag muss > 0 sein
- Kontostand von Konto Quelle auslesen
- Überprüfen ob Betrag <= Kontostand Quelle
- Neuen Kontostand von Konto Quelle berechnen
- Kontostand von Konto Ziel auslesen
- Neuen Kontostand von Konto Ziel berechnen
- Kontostände auf beiden Konti setzen

Problem 1: Der Kontostand von Konto Quelle wird nach dem Auslesen
durch einen anderen Zugriff geändert und so falsch weiterverarbeitet.

Problem 2: Während eines Übertrags könnte ein Konto gelöscht worden sein.

Problem 3: Es kann sein, dass ein Benutzende ein Quellen Konto auswählt worauf sie nicht berechtigt ist.

Lösung 1 & 2: Es sollten Transaktionen vom Typ serializable verwendet werden. Durch diese werden die Daten,
auf die Zugegriffen wurde, gelockt, was dazu führt, dass andere Prozesse diese nicht verändern können.

Lösung 3: Die Authorisierung stellt sicher, dass Benutzende nur auf Konti zugreifen können, die ihnen auch
wirklich gehören.
