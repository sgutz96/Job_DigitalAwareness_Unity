using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class OllamaChatWithProp : MonoBehaviour
{
    public string model = "mistral";
    private string apiUrl = "http://localhost:11434/api/generate";

    [TextArea(2, 10)]
    public string propBase = "Eres Samantha. Vives en el 308 de la calle Guillermo de Ocaña y trabajas de mesera en un bar.";

    private List<string> chatHistory = new List<string>();

    public void SendPrompt(string userInput)
    {
        StartCoroutine(SendMessageToOllama(userInput));
    }

    IEnumerator SendMessageToOllama(string userInput)
    {
        // 1. Construir el prompt completo con contexto inicial y memoria
        StringBuilder fullPrompt = new StringBuilder();

        // Añadir contexto base
        fullPrompt.AppendLine(propBase);
        fullPrompt.AppendLine();

        // Añadir historial del chat
        foreach (string message in chatHistory)
        {
            fullPrompt.AppendLine(message);
        }

        // Añadir nuevo mensaje del usuario
        fullPrompt.AppendLine("Usuario: " + userInput);

        // 2. Preparar solicitud JSON
        OllamaRequest req = new OllamaRequest
        {
            model = model,
            prompt = fullPrompt.ToString(),
            stream = true
        };

        string json = JsonUtility.ToJson(req);

        // 3. Enviar solicitud a Ollama
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                // 4. Leer respuesta streaming
                string rawResponse = request.downloadHandler.text;
                string[] lines = rawResponse.Split('\n');
                StringBuilder responseBuilder = new StringBuilder();

                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        try
                        {
                            var jsonObj = JsonUtility.FromJson<ResponseLine>(line);
                            responseBuilder.Append(jsonObj.response);
                        }
                        catch { }
                    }
                }

                string finalResponse = responseBuilder.ToString().Trim();

                // 5. Guardar en el historial
                chatHistory.Add("Usuario: " + userInput);
                chatHistory.Add("Mistral: " + finalResponse);

                Debug.Log("Mistral: " + finalResponse);
            }
        }
    }

    [System.Serializable]
    public class OllamaRequest
    {
        public string model;
        public string prompt;
        public bool stream;
    }

    [System.Serializable]
    public class ResponseLine
    {
        public string response;
    }
}
