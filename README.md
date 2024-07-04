# Lern-Bot

## Zusammenfassung

## Virtuelle Umgebung

## Backend

### Informationverarbeitung

Über eine Schaltfläche hat der Benutzer die Möglichkeit, Dokumente hochzuladen.
Die Dokumente können über den POST Endpunkt "/dokumente" hochgeladen werden.
Zur weiteren verarbeitung werden die Dokumente vorübergehend in einem Ordner auf dem Server gespeichert.
Von hier an wird das Dokument zuerst von einem Document-Loader verarbeitet. Dieser hat die Aufgbabe, unterschiedliche Formate von PDFs einzulesen und einheitlich in ein Textformat zu konvertieren. Hierbei werden die Seiten der PDFs einzeln verarbeitet, so dass später ein Textabschnitt wieder einer Seite im PDF zugeordnet werden kann.

Anschließend muss das Dokument in kleinere Abschnitte unterteilt werden. Hierfür gibt es mehrere Ansätze. Hier wurde der Einfachheit halber der "RecursiceCharacterTextSplitter" mit den Parametern Chunksize = 1000 und Overlap = 200 gewählt. Das bedeutet, dass ein Abschnitt 1000 Zeichen umfasst, wobei die ersten und letzten 200 Zeichen mit den benachbarten Abschnitten geteilt werden. Dieser Schritt ist notwendig, da die Texte sonst zu lang sind und die Zuordnung ungenau ist.

### Chatbot

## Didaktischer Ansatz

## Installation
