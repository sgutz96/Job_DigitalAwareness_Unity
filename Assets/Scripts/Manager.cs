using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Manager : MonoBehaviour
{
    [SerializeField]
    private OllamaChatWithDataSet Ollama;

    [SerializeField]
    private Button button;

    [SerializeField]
    private TMP_InputField inputField;

    private void Awake()
    {
        // Asignamos el evento del botón
        button.onClick.AddListener(SentTextmeshPro);
    }

    public void SentTextmeshPro()
    {
        string text = inputField.text;
        Ollama.SendPrompt(text);
    }


}
