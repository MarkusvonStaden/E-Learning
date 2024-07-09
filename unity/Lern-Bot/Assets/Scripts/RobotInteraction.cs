using System;
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
    public GameObject uiPanel;
    public GameObject uiPanel2;
    public GameObject uiPanel3;
    public GameObject speechBubbleBackground;
    public TMP_Text speechBubbleText;
    public Button askQuestionButton;
    public Button uploadDocumentButton;
    public Button quitButton;
    public TMP_InputField questionInputField;

    public string documentUploadUrl = "http://127.0.0.1:8000/documents";
    public string websocketUrl = "ws://127.0.0.1:8000/ws/";

    private bool isUIVisible = false;
    private WebSocket websocket;

    private string receivedResponse;
    private bool responseReceived = false;
    private bool waitingForResponse = false;

    void Start()
    {
        // On-click listener for buttons
        askQuestionButton.onClick.AddListener(AskQuestion);
        uploadDocumentButton.onClick.AddListener(UploadDocument);
        quitButton.onClick.AddListener(QuitGame);

        // Hide UI at beginning
        uiPanel.SetActive(false);
        uiPanel2.SetActive(false);
        uiPanel3.SetActive(false);
        speechBubbleBackground.SetActive(false);

        // Activate UIPanel2 after 6 secs
        StartCoroutine(ActivatePanelAfterDelay(6f));

        // Initialize WebSocket
        websocket = new WebSocket(websocketUrl);
        websocket.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket-Connection opened");
        };

        websocket.OnMessage += WebSocket_OnMessage;

        websocket.OnError += (sender, e) =>
        {
            Debug.LogError("WebSocket-Error: " + e.Message);
        };

        websocket.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket-Connection closed");
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
        uiPanel2.SetActive(!isUIVisible);
    }

    void ShowSpeechBubble()
    {
        speechBubbleText.text = "Wie kann ich dir heute helfen?";
        speechBubbleBackground.SetActive(true);
    }

    IEnumerator ActivatePanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        uiPanel2.SetActive(true); // Activate the UI panels
        uiPanel3.SetActive(true);
    }

    void AskQuestion()
    {   
        if(waitingForResponse)
        {
            Debug.Log("Waiting for previous response.");
            return;
        }

        string question = questionInputField.text;
        Debug.Log("Ask question: " + question);
        
        // Perform WebSocket operations
        if(websocket != null && websocket.ReadyState == WebSocketState.Open && question != "")
        {
            websocket.Send(question);
            Debug.Log("Question sent");

            waitingForResponse = true;

            // Wait for response
            StartCoroutine(WaitForResponseCoroutine());
        }
        else if(question == "")
        {
            Debug.Log("Enter a question first");

            speechBubbleText.text = "Bitte gib unter 'Enter Text...' erste eine Frage ein und drücke dann auf 'Eine Frage stellen'";
            speechBubbleText.fontSize = 36;
        }
        else
        {
            Debug.LogError("WebSocket-Connection not opened");
        }
    }

    IEnumerator WaitForResponseCoroutine()
    {
        // Wait until response is received
        while(!responseReceived)
        {
            yield return null;
        }

        // Process received response
        UpdateSpeechBubble();
    }

    void WebSocket_OnMessage(object sender, MessageEventArgs e)
    {
        // Handle WebSocket message received
        Debug.Log("Message received");
        receivedResponse = e.Data;
        responseReceived = true;
    }

    void UpdateSpeechBubble()
    {
        // Display received response in speech bubble
        speechBubbleText.text = receivedResponse;
        speechBubbleText.fontSize = 36;
        speechBubbleBackground.SetActive(true);

        Debug.Log("Answer updated");
        waitingForResponse = false;
        responseReceived = false;
    }

    void UploadDocument()
    {
        Debug.Log("Upload document");

        StartCoroutine(UploadDocumentCoroutine());
    }

    IEnumerator UploadDocumentCoroutine()
    {
        yield return new WaitForEndOfFrame();

        var paths = StandaloneFileBrowser.OpenFilePanel("Select Document", "", "pdf", false);
        if(paths.Length > 0)
        {
            string path = paths[0];
            if(!string.IsNullOrEmpty(path))
            {
                if(Path.GetExtension(path).ToLower() == ".pdf")
                {
                    byte[] fileData = File.ReadAllBytes(path);
                    StartCoroutine(UploadFile(path, fileData));
                }
                else
                {
                    Debug.LogError("Only PDF files are allowed.");
                }
            }
        }
    }

    IEnumerator UploadFile(string path, byte[] fileData)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, Path.GetFileName(path), "application/pdf");

        using(UnityWebRequest www = UnityWebRequest.Post(documentUploadUrl, form))
        {
            Debug.Log("Uploading file: " + Path.GetFileName(path));
            yield return www.SendWebRequest();

            if(www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.downloadHandler.text);
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("File successfully uploaded");
            }
        }
    }

    void QuitGame()
    {
        // Quit the game
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}


