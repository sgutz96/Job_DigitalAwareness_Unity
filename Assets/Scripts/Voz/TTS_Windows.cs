using UnityEngine;
using System.Diagnostics;

public class TTS_PS : MonoBehaviour
{
    public void Speak(string text)
    {
        string cmd = $"Add-Type -AssemblyName System.Speech; (New-Object System.Speech.Synthesis.SpeechSynthesizer).Speak('{text}')";
        Process.Start("powershell.exe", $"-Command \"{cmd}\"");
    }

    void Start()
    {
        Speak("Hola desde PowerShell.");
    }
}
