using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using TMPro;

using SFB;
using WebSocketSharp;

public class RobotInteraction : MonoBehaviour
{
    public GameObject uiPanel; // Das Interface Panel
    public GameObject speechBubbleBackground; // Der Hintergrund der Sprechblase
    public TMP_Text speechBubbleText; // Der Text der Sprechblase
    public Button askQuestionButton; // Der Button zum Stellen einer Frage
    public Button uploadDocumentButton; // Der Button zum Hochladen eines Dokuments
    public TMP_InputField questionInputField;

    public string documentUploadUrl = "http://127.0.0.1:8000/documents";
    public string websocketUrl = "ws://127.0.0.1:8000/ws/";

    private bool isUIVisible = false;
    private WebSocket websocket;

    void Start()
    {
        // Setze die OnClick-Listener für die Buttons
        askQuestionButton.onClick.AddListener(AskQuestion);
        uploadDocumentButton.onClick.AddListener(UploadDocument);

        // Verstecke das UI-Panel und die Sprechblase zu Beginn
        uiPanel.SetActive(false);
        speechBubbleBackground.SetActive(false);

        // WebSocket initialisieren
        websocket = new WebSocket(websocketUrl);
        websocket.OnMessage += (sender, e) =>
        {
            ShowWebSocketResponse(e.Data);
        };
        websocket.Connect();
    }

    void OnDestroy()
    {
        websocket.Close();
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

        string question = questionInputField.text;
        websocket.Send(question);
    }

    void UploadDocument()
    {
        // Logik zum Hochladen eines Dokuments
        Debug.Log("Dokument hochladen");

        StartCoroutine(UploadDocumentCoroutine());
    }

    IEnumerator UploadDocumentCoroutine()
    {
        yield return new WaitForEndOfFrame();

        // Verwende StandaloneFileBrowser, um die Datei auszuwählen
        string path = StandaloneFileBrowser.OpenFilePanel("Select Document", "", "", false)[0];
        if (!string.IsNullOrEmpty(path))
        {
            byte[] fileData = File.ReadAllBytes(path);
            StartCoroutine(UploadFile(path, fileData));
        }
    }

    IEnumerator UploadFile(string path, byte[] fileData)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, Path.GetFileName(path));

        using (UnityWebRequest www = UnityWebRequest.Post(documentUploadUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("File successfully uploaded");
            }
        }
    }

    void ShowWebSocketResponse(string response)
    {
        speechBubbleText.text = response;
        speechBubbleBackground.SetActive(true);
        //Invoke("HideSpeechBubble", 3f);
    }
}


