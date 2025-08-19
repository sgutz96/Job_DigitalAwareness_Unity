
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
public class OllamaChat : MonoBehaviour
{
    public string model = "mistral";
    private string apiUrl = "http://localhost:11434/api/generate";
    private List<string> chatHistory = new List<string>();

    public void SendPrompt(string prompt)
    {
        StartCoroutine(SendMessageToOllama(prompt));
    }

    IEnumerator SendMessageToOllama(string prompt)
    {

        string json = "{\"model\": \"" + model + "\", \"prompt\": \"" + prompt + "\", \"stream\": true}";

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer(); // Gets full body as one string
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                string rawResponse = request.downloadHandler.text;

                // Cada línea es un objeto JSON separado
                string[] lines = rawResponse.Split('\n');
                StringBuilder fullResponse = new StringBuilder();


                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        try
                        {
                            var jsonObj = JsonUtility.FromJson<ResponseLine>(line);
                            fullResponse.Append(jsonObj.response);
                        }
                        catch
                        {
                            // Omitir errores de parseo (en caso de líneas vacías o incorrectas)
                        }
                    }
                }

                Debug.Log("Respuesta completa de Mistral: " + fullResponse.ToString());
            }
        }
    }

    [System.Serializable]
    public class ResponseLine
    {
        public string response;
    }
}


/*public class OllamaChat : MonoBehaviour
{
    public string model = "mistral";
    private string apiUrl = "http://localhost:11434/api/generate";

    public void SendPrompt(string prompt)
    {
        StartCoroutine(SendMessageToOllama(prompt));
    }

    IEnumerator SendMessageToOllama(string prompt)
    {
        string json = "{\"model\": \"" + model + "\", \"prompt\": \"" + prompt + "\"}";

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
                Debug.Log("Respuesta de Mistral: " + request.downloadHandler.text);
                // Aquí puedes parsear el JSON si quieres solo el texto
            }
        }
    }
}*/
