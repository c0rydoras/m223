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


### Usertesting

#### Anforderungen
1. Nur Benutzer:innen in der Rolle «Administrators» oder «Users» können alle Ledgers sehen.
2. Bei einer Buchung wird ein Geldbetrag von einem Konto auf ein anderes verschoben.
3. Nur Benutzer:innen in der Rolle «Administrators» können Ledgers verändern.
4. Bei einer Buchung darf der Kontostand nie unter 0 fallen.
5. Benutzer:innen mit der Rolle "Administrators" oder "Users" können die Auflistung der letzten
Transaktionen einsehen.
6. Nur Benutzer:innen mit der Rolle "Administrators" können Ledgers löschen

#### Testfälle
1. Buchung in der Rolle "Administrators" ist erfolgreich.
2. Buchung in der Rolle "Users" ist nicht möglich.
3. Alle Ledgers mit der Rolle "Administrators" anzeigen.
4. Alle Ledgers mit der Rolle "Users" anzeigen.
5. Ledgers können mit der Rolle "Administrators" bearbeitet werden.
6. Ledgers können mit der Rolle "Users" **nicht** bearbeitet werden.
7. Buchungen sind nicht möglich, wenn der Kontostand dadurch unter 0 fallen würde.
8. Rolle "Administrators" kann die Liste mit den letzten Transaktionen einsehen. 
9. Rolle "Users" kann die Liste mit den letzten Transaktionen einsehen.
10. Rolle "Administrators" kann Ledgers löschen.
11. Rolle "Users" kann Ledgers nicht löschen.

| Testfall Nr. | Anforderung | Eingabe                                                                                        | Ausgabe                                                                                                                                                                          |
|--------------|-------------|------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 1            | 2           | 10$ von Ledger "Manitu AG" auf Ledger "Chrysalkis GmbH"                                        | Kontostand von "Manitu AG" ist um 10 gesunken und Kontostand von "Chrysalkis GmbH" um 10 gestiegen                                                                               |
| 2            | 2           | Das Eingabefeld für die Buchung kann nicht erreicht werden                                     | Der Kontostand von keinem Ledger konnte verändert werden                                                                                                                         |
| 3            | 1           | Die Startseite der Applikation wird aufgerufen                                                 | Alle Ledgers sind in der Auflistung zu sehen                                                                                                                                     |
| 4            | 1           | Die Startseite der Applikation wird aufgerufen                                                 | Alle Ledgers sind in der Auflistung zu sehen                                                                                                                                     |
| 5            | 3           | Ledger "Manitu AG" wird zu "Manifu AG" umbenannt                                               | Der Ledger hat nun den neuen Namen und ist auf der Übersichtsseite mit neuem Namen zu sehen                                                                                      |
| 6            | 3           | Das Bearbeiten des ledgers ist nicht möglich                                                   | Keine Daten des Ledgers wurden geändert                                                                                                                                          |
| 7            | 4           | Es wird versucht 230'000 von von Ledger "Smith & Co KG" auf Ledger "Chrysalkis GmbH" zu buchen | Eine Fehlermeldung wird angezeigt, dass der zu buchende Betrag unzureichend ist                                                                                                  |
| 8            | 5           | Die Detailseite des Ledgers "Smith & Co KG" wird aufgerufen                                    | Die Liste mit den letzten Buchungen die diesen Ledger betreffen ist ersichtlich                                                                                                  |
| 9            | 5           | Die Detailseite des Ledgers "Smith & Co KG" wird aufgerufen                                    | Die Liste mit den letzten Buchungen die diesen Ledger betreffen ist ersichtlich                                                                                                  |
| 10           | 6           | Der Ledger "Smith & Co KG" wird gelöscht                                                       | Der Ledger "Smith & Co KG" existiert nicht mehr und wird auf der Übersichtsseite nicht mehr angezeigt. Zudem wird er in den letzten Transaktionen als "Deleted Ledger" angezeigt |
| 11           | 6           | Das Löschen des Ledgers "Chrysalkis GmbH" ist nicht möglich                                    | Der Ledger bleibt bestehen und ist noch sichtbar                                                                                                                                 |

#### Exploratives Testen
Zu testende Anforderungen: 1, 4, 5

Rahmenbedingungen:

- Es muss eine geseedete Datenbank mit existierenden Ledgers haben
- Die Testperson muss eingeloggt sein
- Alle Aspekte der Applikation, die mit dem Test zu tun haben oder dessen Endergebnis beeinflussen könnten,
sollen bedient werden
- Um Fehler zu finden, sollen extra fehlerhafte Eingaben gemacht werden und alle möglichen Szenarien sollen
durchgespielt werden

Testideen:

- Versuchen einen Negativen Betrag zu übertragen
- Versuchen beim Betrag, Text einzugeben
- Versuchen einem Ledger einen leeren Namen zu geben