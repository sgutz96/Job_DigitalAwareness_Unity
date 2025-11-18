using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class OllamaChatSanpachito : MonoBehaviour
{
    public string model = "llama3.2";
    private string apiUrl = "http://localhost:11434/api/generate";

    [TextArea(2, 10)]
    public string propBase =
        "Eres el fraile Sanpachito. Eres franciscano, alegre, humilde y amante de la naturaleza. " +
        "Vives en el convento de San Buenaventura y dedicas tu vida a orientar a los jóvenes " +
        "para que encuentren su vocación y completen sus carreras universitarias con fe, esperanza y disciplina." +
        "Das respuestas cortas pero pensativas.";

    private List<string> chatHistory = new List<string>();

    // 🔹 Dataset externo: información universitaria
    public TextAsset universidadDatasetFile; // arrastra el JSON con la info de la universidad
    private UniversidadData universidadData;

    public TTS_PS TTS;
    public SanpachitoMager manager;

    

    public bool isThink = false;


    void Start()
    {
        if (universidadDatasetFile != null)
        {
            universidadData = JsonUtility.FromJson<UniversidadData>(universidadDatasetFile.text);

            // Agregar información del dataset al contexto base
            propBase += $@" 

Información de la Universidad:
- Nombre: {universidadData.universidad.nombre}
- Sede: {universidadData.universidad.sede}
- Sigla: {universidadData.universidad.sigla}
Programas disponibles:
{string.Join("\n", GetProgramasList())}
";
        }
        else
        {
            Debug.LogWarning("⚠️ No se asignó universidadDatasetFile en el inspector.");
        }
    }

    private List<string> GetProgramasList()
    {
        List<string> list = new List<string>();

        foreach (var p in universidadData.universidad.programas)
        {
            list.Add($"- {p.nombre}: {p.descripcion} (facultad: {p.facultad}) (SNIES: {p.codigo_SNIES}) (costo: {p.costo.valor_semestre} , {p.costo.moneda})");
        }

        return list;
    }


    public enum Reaction
    {
        Neutral,
        Happy,
        Sad,
        Thoughtful,
        Encouraging
    }

    [System.Serializable]
    public class ChatResult
    {
        public string text;
        public Reaction reaction;
        public float talkDuration;
    }

    public void SendPrompt(string userInput)
    {
        isThink = false;
        StartCoroutine(SendMessageToOllama(userInput));
    }

    IEnumerator SendMessageToOllama(string userInput)
    {
        StringBuilder fullPrompt = new StringBuilder();

        fullPrompt.AppendLine(propBase);
        fullPrompt.AppendLine();

        foreach (string message in chatHistory)
        {
            fullPrompt.AppendLine(message);
        }

        fullPrompt.AppendLine("Estudiante: " + userInput + "Respuesta corta porfavor");

        OllamaRequest req = new OllamaRequest
        {
            model = model,
            prompt = fullPrompt.ToString(),
            stream = true
        };

        string json = JsonUtility.ToJson(req);

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

                chatHistory.Add("Estudiante: " + userInput);
                chatHistory.Add("Sanpachito: " + finalResponse);

                TTS.Speak(finalResponse);
                ChatResult chatResult = ProcessResponse(finalResponse);

                Debug.Log("-------------------------------------");
                Debug.Log("Estudiante: " + userInput);
                Debug.Log("Sanpachito: " + chatResult.text);
                Debug.Log("Reacción: " + chatResult.reaction);
                Debug.Log("Duración: " + chatResult.talkDuration + "s");

                manager.AnimateSanpachito(chatResult.reaction, chatResult.talkDuration);
                manager.ChatRespuesta.text = chatResult.text;

                isThink = true;
            }
        }
    }

    private ChatResult ProcessResponse(string response)
    {
        ChatResult result = new ChatResult();
        result.text = response;
        result.talkDuration = response.Split(' ').Length * 0.18f;

        if (response.Contains("ánimo") || response.Contains("puedes"))
            result.reaction = Reaction.Encouraging;
        else if (response.Contains("feliz") || response.Contains("alegre"))
            result.reaction = Reaction.Happy;
        else if (response.Contains("triste") || response.Contains("difícil"))
            result.reaction = Reaction.Sad;
        else if (response.Contains("reflexiona") || response.Contains("piensa"))
            result.reaction = Reaction.Thoughtful;
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
