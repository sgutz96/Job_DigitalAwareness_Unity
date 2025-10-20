using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows.Speech;

public class VoiceToTextConsole : MonoBehaviour
{
    private DictationRecognizer recognizer;
    private StringBuilder buffer = new StringBuilder();
    private bool isDictating = false;

    [Header("Texto Reconocido")]
    [TextArea(3, 10)]
    public string textoCompleto = "";

    [Header("Última Frase")]
    public string ultimaFrase = "";

    void Awake()
    {
        recognizer = new DictationRecognizer(ConfidenceLevel.Medium);
        recognizer.DictationHypothesis += OnHypothesis;
        recognizer.DictationResult += OnResult;
        recognizer.DictationComplete += OnComplete;
        recognizer.DictationError += OnRecognizerError;

        // Iniciar la dictadura cuando el juego se inicia
        StartDictation();
    }

    void OnDestroy()
    {
        if (recognizer != null)
        {
            if (isDictating)
                StopDictation();
            recognizer.Dispose();
        }
    }

    void Update()
    {
        // Con el nuevo Input System
        if (Keyboard.current.rKey.wasPressedThisFrame && !isDictating)
        {
            StartDictation();
        }
        else if (Keyboard.current.rKey.wasPressedThisFrame && isDictating)
        {
            StopDictation();
        }

        // Presiona C para limpiar el texto
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            LimpiarTexto();
        }
    }

    public void StartDictation()
    {
        if (isDictating) return;

        recognizer.Start();
        Debug.Log("🎙️ Dictado iniciado... (Presiona R para detener, C para limpiar)");
        isDictating = true;
    }

    public void StopDictation()
    {
        if (!isDictating) return;

        recognizer.Stop();
        Debug.Log("🛑 Dictado detenido.");
        isDictating = false;
    }

    public void LimpiarTexto()
    {
        buffer.Clear();
        textoCompleto = "";
        ultimaFrase = "";
        Debug.Log("🧹 Texto limpiado");
    }

    private void OnHypothesis(string text)
    {
        Debug.Log($"🟡 Escuchando: {text}");
    }

    private void OnResult(string text, ConfidenceLevel confidence)
    {
        // Agregar al buffer
        buffer.AppendLine(text);

        // Actualizar variables públicas
        textoCompleto = buffer.ToString();
        ultimaFrase = text;

        // Mostrar en consola con formato mejorado
        Debug.Log($"✅ Reconocido ({confidence}): {text}");
        Debug.Log($"📝 TEXTO COMPLETO:\n{textoCompleto}");
        Debug.Log("═══════════════════════════════════");
    }

    private void OnComplete(DictationCompletionCause cause)
    {
        Debug.Log($"ℹ️ Finalizó por: {cause}");

        if (cause == DictationCompletionCause.TimeoutExceeded)
        {
            Debug.Log("⏱️ Se detuvo por inactividad. Presiona R para reiniciar.");
        }
    }

    private void OnRecognizerError(string error, int hresult)
    {
        Debug.LogError($"❌ Error: {error} (0x{hresult:X})");
    }

    // Método público para obtener el texto desde otros scripts
    public string ObtenerTextoCompleto()
    {
        return textoCompleto;
    }

    // Método público para obtener la última frase
    public string ObtenerUltimaFrase()
    {
        return ultimaFrase;
    }
}