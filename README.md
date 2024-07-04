# Lern-Bot

## Zusammenfassung

## Virtuelle Umgebung

## Backend

### API

Das Backend wurde in Python mit FastAPI entwicklt. FastAPI bietet insbesondere für Prototypen den Vorteil, dass es sehr schnell und einfach zu implementieren ist. Zudem bietet es eine automatische Dokumentation der API, was die Entwicklung und Wartung erleichtert.

Um die Entwicklung auch in Zukunft zu erleichtern, wurde die API in verschiedene Routen unterteilt. Jede Route hat eine eigene Datei, in der die Endpunkte definiert sind. Die Endpunkte sind in der Datei `main.py` importiert und werden dort aufgerufen. Bisher existieren nur die beiden Routen `documents` und `completion`, in Zukunft ermöglicht diese Architektur allerdings eine leichte Erweiterung.

Die API bietet zwei Endpunkte:

- POST `/documents`: Hier können Dokumente hochgeladen werden. Die Dokumente werden in einem temporären Ordner auf dem Server gespeichert und anschließend verarbeitet.

- WS `/completion`: Hier kann der Benutzer Textabschnitte eingeben, die er vervollständigen möchte. Der Chatbot gibt dann eine Vervollständigung unter Zuhilfenahme der bereitgestellten Dokumente zurück.

### Informationverarbeitung

Über eine Schaltfläche hat der Benutzer die Möglichkeit, Dokumente hochzuladen.
Die Dokumente können über den POST Endpunkt "/dokumente" hochgeladen werden.
Zur weiteren verarbeitung werden die Dokumente vorübergehend in einem Ordner auf dem Server gespeichert.
Von hier an wird das Dokument zuerst von einem Document-Loader verarbeitet. Dieser hat die Aufgbabe, unterschiedliche Formate von PDFs einzulesen und einheitlich in ein Textformat zu konvertieren. Hierbei werden die Seiten der PDFs einzeln verarbeitet, so dass später ein Textabschnitt wieder einer Seite im PDF zugeordnet werden kann.

Anschließend muss das Dokument in kleinere Abschnitte unterteilt werden. Hierfür gibt es mehrere Ansätze. Hier wurde der Einfachheit halber der "RecursiceCharacterTextSplitter" mit den Parametern Chunksize = 1000 und Overlap = 200 gewählt. Das bedeutet, dass ein Abschnitt 1000 Zeichen umfasst, wobei die ersten und letzten 200 Zeichen mit den benachbarten Abschnitten geteilt werden. Dieser Schritt ist notwendig, da die Texte sonst zu lang sind und die Zuordnung ungenau ist.

Wird später vom Benutzer eine Frage gestellt, wird diese Frage ebenfalls zuerst vektorisiert. Mithilfe diesen Vektors wird dann der Abschnitt gesucht, der am besten zur Frage passt. Hierfür wird der Cosinus-Ähnlichkeitswert berechnet. Der Abschnitt mit dem höchsten Wert wird dann zusammen mit der Frage in eine Prompt-Vorlage geschrieben und an den Chatbot weitergegeben, damit dieser eine Antwort aus den Dokumenten generieren kann.

### Containerisierung

Um das deployment zu erleichtern, wurde die API in einem Docker-Container mithilfe von Docker Compose gehostet.

Um die Anwendung zu containerisieren, wurde ein Dockerfile erstellt. Dieses Dockerfile erstellt ein Image, das alle notwendigen Abhängigkeiten enthält. Das Image kann dann in einem Container ausgeführt werden. Der Container wird auf einem Server gehostet und stellt die API zur Verfügung.

### Chatbot

Zur Vervollständigung der Textabschnitte wird ein Chatbot verwendet. Als Sprachmodell wurde hier GPT-4o gewählt. Dieses Modell wurde von OpenAI entwickelt und ist eines der fortschrittlichsten Sprachmodelle, die derzeit verfügbar sind. Es wird über die API von OpenAI angesprochen und gibt eine Vervollständigung des Textes zurück.

## Didaktischer Ansatz

## Ausblick

Die Anwendung ist in ihrer aktuellen Form nicht mehr als ein Proof-of-Concept. Es gibt viele Möglichkeiten, die Anwendung zu erweitern und zu verbessern.

Um eine solche Anwendung in der Praxis zu verwenden, müssten noch einige Schritte unternommen werden.
In der aktuellen Version wurde der Aspekt der IT-Sicherheit, sowie der Datenschutz, noch nicht berücksichtigt. Um die Anwendung in der Praxis zu verwenden, müssten diese Aspekte jedoch unbedingt berücksichtigt werden.

Hierfür wäre z.B. ein vollständiges Nutzermanagement notwendig, damit Nutzer nur auf die Dokumente zugreifen können, auf die sie auch zugreifen dürfen. Zudem müssten die Dokumente verschlüsselt gespeichert werden, um die Vertraulichkeit zu gewährleisten.

## Installation

Dank der Containerisierung ist die Installation sehr einfach.

Um die Anwendung zu starten, muss Docker Compose bzw. Docker Desktop installiert sein.

Der OpenAI API Key muss in der Datei `.env` im Root-Verzeichnis hinterlegt werden. Hierfür kann die Datei `.env.template` als Vorlage verwendet werden.

Anschließend kann der Container mit dem Befehl `docker-compose up` gestartet werden. Der Container wird dann auf dem Port 8000 gehostet. Bei Bedarf kann der Port in der Datei `docker-compose.yml` geändert werden.
