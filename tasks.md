### Dokumentation der implementierten Prozess und der Überlegungen

**Herausforderung 1**

Unsere erste grosse Herausforderung war, dass die Transactions mit EF-Core nicht
richtig funktioniert haben. Entweder hat sich die Datenbank gelockt oder die Transactions
schienen nichts bewirkt zu haben, wodurch sich der Geldbetrag trotz Transactions verändert hat.

Wir haben ziemlich lange versucht das Problem zu lösen, aber kamen zuerst nicht zu einer zufriedenstellenden
und funktionierenden Lösung. Das Problem hat sich jedoch später zum Teil von selbst gelöst, als wir zum Testing
gekommen sind, denn da haben wir eine Sqlite DB eingesetzt, wo die Transactions plötzlich viel besser funktioniert
haben. Darum sind wir dann später in der Produktionsumgebung auch auf Sqlite gewechselt, da MariaDB viel grössere Probleme
machte.

Mit Sqlite funktionieren die Tests und Lasttests nun besser und zuverlässiger. Zwar gibt es bei den concurrency Tests
selten immer noch einen DB-Lock, meistens laufen sie aber durch.

**Herausforderung 2**

Ein grosses Problem auf das wir beim Testing gestossen sind, war die dependency-injection. Diese funktionert nicht nativ
mit XUnit, was wir zu beginn jedoch nicht wussten. Da wir jedoch nicht mit Mocks arbeiten sollten, mussten wir dafür eine
Lösung finden.

Wir haben im Internet gesucht wie man dependency-injection in XUnit verwenden kann und sind dann schnell darauf gekommen,
dass dafür ein externes Package notwendig ist. Davon gab es mehrere und wir haben uns für eines entschieden.

Mit dem dependency-injection Package konnten wir die services, repositories etc. dann wie im backend in einem `Startup.cs`
file laden und sie dann per dependency-injection verwenden.

**Herausforderung 3**

Beim Schreiben der load-tests mussten wir zuerst per `HttpClient` die Daten von der API abfragen. Dafür haben wir DTO-Klassen
geschrieben. Jedoch hat die API zuerst nicht die richtigen Daten von uns bekommen, obwohl wir das DTO per JSON-serializer
serialisiert haben.

Nach längerem debuggen haben wir gemerkt, dass der JSON-Serializer die DTOs nicht richtig serialisiert, weil die Felder
auf dem DTO keine getter und setter hatten.

Nachdem wir getter und setter zu den Feldern des DTOs hinzugefügt hatten, hat das serializing funktioniert.

### Dokumentation der Tests und der Testergebnisse.

**Test 1 - MultiUserBookingServiceTests (TestBookingParallel)**

In diesem Test haben wir getestet, ob die Applikation multi-user-fähig ist und mehrere Datenbankzugriffe auf einmal aushält.
Dabei war vor allem wichtig, dass kein Geld verloren geht, wenn z.B. mal eine Transaction failed.

Dieser Test wurde mit direktem Datenbankzugriff über den Kontext durchgeführt. Um mehrere user zu simulieren, haben wir die
Methode, welche die Transaktionen durchführt, per multithreading mehrmals parallel ausgeführt. So wurden sehr viele bookings
auf einmal durchgeführt.

Bei MariaDB war das Resultat oft, dass sich die Geldmenge verändert hat oder die Transaktionen gar nicht funktioniert haben.
Nachdem wir begonnen hatten sqlite zu verwenden, passed dieser Test nun meistens und kein Geld geht mehr verloren.

**Test 2 - BookingServiceTests (SuccessfulBooking)**

Mit diesem Test haben wir getestet, ob die Methode zum Geld überweisen richtig implementiert ist und das Geld richtig abgezogen
und addiert wird.

Der Test ist ein parametrisierter Test, was bedeutet, dass wir ihn mehrmals mit unterschiedlichen Werten ausführen. Zuerst wird
die `Book` Method aufgerufen, mit dem Sender, dem Empfänger und dem Amount. Danach wird sichergestellt, dass dieser Methodenaufruf
keine Exception geworfen hat. Zum Schluss wird aus der Datenbank gelesen und sichergestellt, dass die Geldbeträge richtig gespeichert
wurden.

Der Test hat gezeigt, dass die Buchungen bei uns richtig funktionieren und die Beträge richtig gesetzt werden.

**Test 3 - LoadTest.Cli**

Der Lasttest stellt sicher, dass unsere API resistent gegenüber einer Flut von plötzlichen Anfragen an den booking endpoint ist und
auch hier nicht plötzlich Geld generiert oder verloren wird.

Um den Test zu implementieren, haben wir ein Tool namens NBomber verwendet, welches uns erlaubt sehr viele Requests pro Sekunde abzusenden.
Zudem haben wir zu Beginn und am Ende des Tests die Geldmenge ausgelesen und am Schluss sichergestellt, dass sich diese nicht verändert hat.

Der Test hat ergeben, dass MariaDB recht schlecht mit der grossen Last klargekommen ist und sich die Geldmenge teilweise auch verändert hat.
Als wir dann zu sqlite gewechselt sind, hat die Geldmenge sich nicht mehr verändert, sondern wir haben nur selten die erwarteten conflict Meldungen
erhalten.

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

Lösung 3: Die Authorisation stellt sicher, dass Benutzende nur auf Konti zugreifen können, die ihnen auch
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