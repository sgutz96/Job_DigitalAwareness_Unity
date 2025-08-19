using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Text;
using UnityEngine.InputSystem; // si usas el New Input System

public class VoiceToTextConsole : MonoBehaviour
{
    private DictationRecognizer recognizer;
    private StringBuilder buffer = new StringBuilder();

    void Awake()
    {
        recognizer = new DictationRecognizer(ConfidenceLevel.Medium);

        recognizer.DictationHypothesis += OnHypothesis;
        recognizer.DictationResult += OnResult;
        recognizer.DictationComplete += OnComplete;
        recognizer.DictationError += OnRecognizerError;
    }

    void OnDestroy()
    {
        if (recognizer != null)
        {
            if (recognizer.Status == SpeechSystemStatus.Running)
                recognizer.Stop();

            recognizer.Dispose();
        }
    }

    void Update()
    {
        // Con el nuevo Input System
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (recognizer.Status == SpeechSystemStatus.Running)
                StopDictation();
            else
                StartDictation();
        }
    }

    public void StartDictation()
    {
        if (recognizer.Status == SpeechSystemStatus.Running) return;
        buffer.Clear();
        recognizer.Start();
        Debug.Log("🎙️ Dictado iniciado...");
    }

    public void StopDictation()
    {
        if (recognizer.Status != SpeechSystemStatus.Running) return;
        recognizer.Stop();
        Debug.Log("🛑 Dictado detenido.");
    }

    private void OnHypothesis(string text)
    {
        Debug.Log($"🟡 Parcial: {text}");
    }

    private void OnResult(string text, ConfidenceLevel confidence)
    {
        buffer.AppendLine(text);
        Debug.Log($"✅ Final ({confidence}): {text}");
    }

    private void OnComplete(DictationCompletionCause cause)
    {
        Debug.Log($"ℹ️ Finalizó por: {cause}");
    }

    private void OnRecognizerError(string error, int hresult)
    {
        Debug.LogError($"❌ Error: {error} (0x{hresult:X})");
    }
}
