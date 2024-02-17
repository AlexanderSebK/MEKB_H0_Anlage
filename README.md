# MEKB H0-Anlage
H0 Anlagensteuerung über Z21-Protokoll und Windows-Forms

# Inhalt
- Zusammenfassung
- Designziele
- Features in Planung
- Features für die Zukunft

# Zusammenfassung
Das Program dient zur Steuerung der H0-Anlage des Modell-Eisenbahn-Klubs Berlin 1932 e.V. (MEKB). 
Über das öffentlich zugängliche Z21-Protokoll werden Lokomotiven, Weichen, Signale, Rückmelder, ect. angesteuert um einen Bahnbetrieb zu simulieren. Das Program ist erstmal auf diese Anlage ausgerichtet, soll aber so portiert werden, dass auch andere Anlagen damit gesteuert werden können.

Die Idee hinter den Program ist, dass wir zunächst die Anlage über Railware gesteuert haben, konnten aber "nur" den Loks beim Fahren zusehen und nicht selber steuern. Zudem mussten die Loks eingelesen werden, was viele Lokomotiven vom Betrieb abgehalten hatte, da nicht alle Klubmitglieder sind so technikaffin sind. Daher wurden folgende Designziele für das Program festgelegt:

# Designziele
- Lokomotiven sollen nicht eingelesen werden (höchstens ein paar Parameter zum nachjustieren). Jede Lok mit DCC-Decoder soll durch Place&Play sofort in den Bahnbetrieb eingebunden werden können.
- Z21-App soll auf die Anlage anwendbar sein
- Einzelne Loks sollen unabhängig auf Automatikbetrieb oder manuell gesteuert werden. Es soll z.B. die Situation möglich sein, dass eine Lok von einem Klubmitglied gesteuert wird und alle anderen Loks je nach Signalstellung automatisch Fahren und Halten. 
- Manuell gesteuerte Loks sollen über Signalstellungen informiert werden, ob sie fahren dürfen und zwangsgebremst werden, wenn versucht wird ein rotes Signal zu überfahren
- Der Fahrdienstleiter soll sowohl manuell als auch automatisch möglich sein.

# Features in Planung
Allgemeines
- [x] Z21 Kommunikation 
Grundlegende Kommunikation. Anlegen von Funktionen und Callback-Interrupts

- [x] Lokomotiven Verwaltung
Aufbau einer Lokomotivenliste mit verschiedenen Suchfunktionen

- [x] Lokeditor
Anlegen eines Tools um die Lokomotiven in der Verwaltung zu ändern/neu anlegen

Basisfunktionen
- [x] Weichensteuerung
Anlegen von Weichensymbolen als Buttons. Lesen der Weicheneigenschaften über XML. Rückmeldung über die Z21 über die Weichenstellung 

- [x] Signalsteuerung
Anlegen von Schaltsymbolen. Lesen der Eigenschaften über XML.

- [x] Zugsteuerung
Anlegen einer Lokomotiven Klasse und Steuerung der Lokomotiven. Lesen der Lokeigenschaften über XML.

- [x] Fahrstraßen
Fahrstraßen der Anlage hinzufügen mit Kontroller der Weichenstellung. 

- [x] Belegtmelder
Lesen des Belegtmelder-Status über Z21.

- [ ] Lokverfolgung
Verfolgung der Lok über die Belegtmelder der Anlage

- [ ] Bremsweg / Zwangsbremsung
Zwangsbremsung manuellgesteuerter Loks vor rotem Signal

- [x] Signal / Blockbelegterkennung
Signal Schaltung nach Fahrstrassen und Belegtmeldung. 

- [x] Bahnhofsansagen
Wenn ein Zug einen bestimmten Punkt erreicht, soll eine Ansage ertönen mit der Zugnummer (Gattung+Adresse) und in welches Gleis er einfahren soll

MEKB Anlage 
- [x] Gleisplan zeichnen
Aktuellen Gleisplan der MEKB H0-Anlage importieren

- [x] Alle Weichen zuordnen
Alle Weichen mit Adressen und Schaltzeiten zuordnen

- [x] Alle Signal zuordnen
Alle Signale mit Adressen zuordnen 

- [x] Alle Fahrstrassen zuordnen
Alle Fahrstrassen für die Anlage generieren

- [x] Alle Blöcke zuordnen
Alle Belegtblöcke nach Port und Pin zuorndne

- [x] Portieren des Gleisplans auf externe Datei. 
Gleisplan ist in eine XML-Datei gespeichert

Automatisierung
- [x] Automatische Signalsteuerung
Signale schalten je nach Weichenstellung / Gesetzte Fahrstrassen / Gleisbesetzung

- [ ] Automatische Zugsteuerung
Züge fahren automatisch wenn das Signal auf grün steht bis zum Nächsten Signal. Inklusive realistisches Anfahren und Abbremsen

- [ ] Automatischer Fahrdienstleiter
Fahrstrassen werden automatisch gesetzt, entweder nach Fahrplan oder Gattung der Züge (Wartezeit, Priorität)

# Features für die Zukunft
Weitere Ideen für das Programm nach erster Fertigstellung
- Rangier-Fahrstrassen
Es soll möglich sein spezielle Abschnitte für Rangierfahrten zu reservieren 
- Externes Display für Lokinformationen für Zuschauer
Soll ein Externes Display (Oder extra PC) an das Netzwerk angeschlossen werden, der Details zu den Fahren Loks anzeigt.

