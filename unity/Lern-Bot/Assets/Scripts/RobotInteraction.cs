using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RobotInteraction : MonoBehaviour
{
    public GameObject uiPanel; // Das Interface Panel
    public GameObject speechBubbleBackground; // Der Hintergrund der Sprechblase
    public TMP_Text speechBubbleText; // Der Text der Sprechblase
    public Button askQuestionButton; // Der Button zum Stellen einer Frage
    public Button uploadDocumentButton; // Der Button zum Hochladen eines Dokuments

    private bool isUIVisible = false;

    void Start()
    {
        // Setze die OnClick-Listener für die Buttons
        askQuestionButton.onClick.AddListener(AskQuestion);
        uploadDocumentButton.onClick.AddListener(UploadDocument);

        // Verstecke das UI-Panel und die Sprechblase zu Beginn
        uiPanel.SetActive(false);
        speechBubbleBackground.SetActive(false);
    }

    void OnMouseDown()
    {
        ToggleUI();
        ShowSpeechBubble();
    }

    void ToggleUI()
    {
        isUIVisible = !isUIVisible;
        uiPanel.SetActive(isUIVisible);
    }

    void ShowSpeechBubble()
    {
        speechBubbleText.text = "Guten Tag, wie kann ich dir heute helfen?";
        speechBubbleBackground.SetActive(true);
        // Deaktiviere die Sprechblase nach 3 Sekunden
        //Invoke("HideSpeechBubble", 3f);
    }

    void HideSpeechBubble()
    {
        speechBubbleBackground.SetActive(false);
    }

    void AskQuestion()
    {
        // Logik zum Stellen einer Frage
        Debug.Log("Frage stellen");
        // Hier kannst du Code hinzufügen, um eine Frage zu verarbeiten oder ein neues UI zu öffnen
    }

    void UploadDocument()
    {
        // Logik zum Hochladen eines Dokuments
        Debug.Log("Dokument hochladen");
        // Hier kannst du Code hinzufügen, um den Datei-Upload-Prozess zu starten
    }
}


