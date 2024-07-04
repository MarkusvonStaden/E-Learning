# Lern-Bot

## Zusammenfassung

Ziel dieses Projektes ist es, die Lernerfahrung mithilfe eines Chatbots, welcher auf Dokumente zugreifen kann, zu verbessern. Der Chatbot soll ergänzend zu Unterricht, Vorlesungen oder Seminaren eingesetzt werden und den Lernenden bei der Vorbereitung auf Prüfungen oder bei der Vertiefung des Stoffes unterstützen. Durch die Möglichkeit, auf Dokumente zuzugreifen, kann der Chatbot auf eine Vielzahl von Informationen zurückgreifen und so eine breite Palette von Fragen beantworten. Die Adaptivität des Chatbots ermöglicht es, auf die individuellen Bedürfnisse der Lernenden einzugehen und so eine personalisierte Lernerfahrung zu schaffen.

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

Der Lern-Bot basiert auf einer Reihe didaktischer Prinzipien, die darauf abzielen, den Lernprozess zu optimieren und zu individualisieren. Im Kern steht das Konzept des adaptiven Lernens, bei dem sich der Chatbot an das Vorwissen und die Lerngeschwindigkeit jedes einzelnen Lernenden anpasst, um eine personalisierte Lernerfahrung zu ermöglichen. Dies wird ergänzt durch den Ansatz des Just-in-Time-Learnings, bei dem der Bot Unterstützung genau dann bietet, wenn sie benötigt wird, was ein effizientes und bedarfsorientiertes Lernen fördert.

Die Interaktion mit dem Chatbot fördert zudem das aktive Lernen, indem Lernende ermutigt werden, Fragen zu stellen und sich intensiv mit dem Lernstoff auseinanderzusetzen. Dies führt zu einem tieferen Verständnis und einer besseren Retention des Gelernten. Die Fähigkeit des Bots, auf verschiedene Dokumente zuzugreifen, unterstützt dabei unterschiedliche Lernstile und ermöglicht eine multimodale Lernunterstützung.

Ein weiterer wichtiger Aspekt ist das kontinuierliche Feedback, das der Bot geben kann. Dies hilft den Lernenden, ihr Verständnis zu überprüfen und Missverständnisse schnell zu korrigieren. Gleichzeitig kann der Bot als eine Art Gerüst dienen, das die Lernenden schrittweise zu einem tieferen Verständnis führt, indem es gezielte Informationen und Erklärungen bereitstellt.

Der Lern-Bot fördert auch das selbstgesteuerte Lernen, indem er es den Lernenden ermöglicht, ihr Lernen selbst zu steuern und in ihrem eigenen Tempo voranzuschreiten. Dies stärkt die Autonomie und Motivation der Lernenden. Wichtig ist dabei zu betonen, dass der Bot als Ergänzung und nicht als Ersatz für den traditionellen Unterricht konzipiert ist. Er unterstützt das Lernen außerhalb des Klassenzimmers und hilft bei der Vor- und Nachbereitung des Unterrichts- und Vorlesungsstoffs.

Durch die Integration dieser didaktischen Prinzipien zielt der Lern-Bot darauf ab, eine effektive, flexible und motivierende Lernumgebung zu schaffen, die den modernen Anforderungen an individualisiertes und technologiegestütztes Lernen gerecht wird. Er bietet damit eine innovative Lösung, um die Lernerfahrung zu verbessern und an die Bedürfnisse der heutigen Lernenden anzupassen.

## Ausblick

Die Anwendung ist in ihrer aktuellen Form nicht mehr als ein Proof-of-Concept. Es gibt viele Möglichkeiten, die Anwendung zu erweitern und zu verbessern.

Um eine solche Anwendung in der Praxis zu verwenden, müssten noch einige Schritte unternommen werden.
In der aktuellen Version wurde der Aspekt der IT-Sicherheit, sowie der Datenschutz, noch nicht berücksichtigt. Um die Anwendung in der Praxis zu verwenden, müssten diese Aspekte jedoch unbedingt berücksichtigt werden.

Hierfür wäre z.B. ein vollständiges Nutzermanagement notwendig, damit Nutzer nur auf die Dokumente zugreifen können, auf die sie auch zugreifen dürfen. Zudem müssten die Dokumente verschlüsselt gespeichert werden, um die Vertraulichkeit zu gewährleisten.

In Zukunft wäre es auch denkbar, über die Möglichkeit des "Function Calling" des Modells weitere Funktionen zu implementieren. So könnte der Chatbot z.B. auch Multiple-Choice-Fragen beantworten oder Lückentexte vervollständigen. Wenn man dieses Konzept noch weiter denkt, können so auch Schnittstellen zu traditionellen Lernplattformen und Lernprogrammen geschaffen werden. So kann das Programm zum Beispiel anhand des Skriptes die Themen erkennen, die der Benutzer noch nicht verstanden hat und mithilfe von anderen fachspezifischen Programmen Übungsaufgaben erstellen. So kann eine noch effektivere Lernumgebung geschaffen werden.

## Installation

Dank der Containerisierung ist die Installation sehr einfach.

Um die Anwendung zu starten, muss Docker Compose bzw. Docker Desktop installiert sein.

Der OpenAI API Key muss in der Datei `.env` im Root-Verzeichnis hinterlegt werden. Hierfür kann die Datei `.env.template` als Vorlage verwendet werden.

Anschließend kann der Container mit dem Befehl `docker-compose up` gestartet werden. Der Container wird dann auf dem Port 8000 gehostet. Bei Bedarf kann der Port in der Datei `docker-compose.yml` geändert werden.
