using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class OllamaChatWithReaction : MonoBehaviour
{
    public string model = "mistral";
    private string apiUrl = "http://localhost:11434/api/generate";

    [TextArea(2, 10)]
    public string propBase = "Eres Samantha. Vives en el 308 de la calle Guillermo de Ocaña y trabajas de mesera en un bar.";

    private List<string> chatHistory = new List<string>();

    // 🔹 Enum de reacciones disponibles
    public enum Reaction
    {
        Neutral,
        Happy,
        Sad,
        Angry,
        Surprised,
        Confused
    }

    // 🔹 Clase para almacenar respuesta procesada
    [System.Serializable]
    public class ChatResult
    {
        public string text;
        public Reaction reaction;
        public float talkDuration; // en segundos
    }

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

                // 6. Procesar respuesta con reacción y duración
                ChatResult chatResult = ProcessResponse(finalResponse);

                Debug.Log("Mistral: " + chatResult.text);
                Debug.Log("Reacción: " + chatResult.reaction);
                Debug.Log("Tiempo de hablar: " + chatResult.talkDuration + "s");

                // 🔹 Aquí puedes llamar a tus animaciones
                // AnimateCharacter(chatResult.reaction, chatResult.talkDuration);
            }
        }
    }

    // 🔹 Procesar respuesta y asignar reacción/duración
    private ChatResult ProcessResponse(string response)
    {
        ChatResult result = new ChatResult();
        result.text = response;

        // Duración estimada (0.15s por palabra aprox)
        int wordCount = response.Split(' ').Length;
        result.talkDuration = wordCount * 0.15f;

        // Reacción básica por keywords
        if (response.Contains("feliz") || response.Contains("alegre"))
            result.reaction = Reaction.Happy;
        else if (response.Contains("triste") || response.Contains("llorando"))
            result.reaction = Reaction.Sad;
        else if (response.Contains("enojado") || response.Contains("molesto"))
            result.reaction = Reaction.Angry;
        else if (response.Contains("sorprendido") || response.Contains("wow"))
            result.reaction = Reaction.Surprised;
        else if (response.Contains("no entiendo") || response.Contains("confuso"))
            result.reaction = Reaction.Confused;
        else
            result.reaction = Reaction.Neutral;

        return result;
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