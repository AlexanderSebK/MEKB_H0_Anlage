# MEKB_H0_Anlage
H0 Anlagensteuerung über Z21-Protokoll und Windows-Forms

#Inhalt

#Zusammenfassung
Das Program dient zur Steuerung der H0-Anlage des Modell-Eisenbahn-Klubs Berlin 1932 e.V. (MEKB). 
Über das öffentlich zugängliche Z21-Protokoll werden Lokomotiven, Weichen, Signale, Rückmelder, ect. angesteuert um einen Bahnbetrieb zu simulieren. Das Program ist erstmal auf diese Anlage ausgerichtet, soll aber so portiert werden, dass auch andere Anlagen damit gesteuert werden können.

Die Idee hinter den Program ist, dass wir zunächst die Anlage über Railware gesteuert haben, konnten aber "nur" den Loks beim Fahren zusehen und nicht selber steuern. Zudem mussten die Loks eingelesen werden, was viele Lokomotiven vom Betrieb abgehalten hatte, da nicht alle Klubmitglieder sind so technikaffin sind. Daher wurden folgende Designziele für das Program festgelegt:

## Designziele
- Lokomotiven sollen nicht eingelesen werden (höchstens ein paar Parameter zum nachjustieren). Jede Lok mit DCC-Decoder soll durch Place&Play sofort in den Bahnbetrieb eingebunden werden können.
- Z21-App soll auf die Anlage anwendbar sein
- Einzelne Loks sollen unabhängig auf Automatikbetrieb oder manuell gesteuert werden. Es soll z.B. die Situation möglich sein, dass eine Lok von einem Klubmitglied gesteuert wird und alle anderen Loks je nach Signalstellung automatisch Fahren und Halten. Manuell gesteuerte Loks sollen zudem zwangsgebremst werden, wenn versucht wird ein rotes Signal zu überfahren
- Der Fahrdienstleiter soll sowohl manuell als auch automatisch möglich sein.
