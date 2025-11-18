using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TTS_PS : MonoBehaviour
{
    [Header("Configuración de Voz")]
    [VoiceSelector]   // ← Atributo que activa el dropdown
    public string voiceName = "";

    [Range(-10, 10)]
    public int rate = 0;

    [Range(0, 100)]
    public int volume = 100;

    public void Speak(string text)
    {
        text = text.Replace("'", "''");

        string cmd = "Add-Type -AssemblyName System.Speech; " +
                     "$synth = New-Object System.Speech.Synthesis.SpeechSynthesizer; ";

        if (!string.IsNullOrEmpty(voiceName))
            cmd += $"$synth.SelectVoice('{voiceName}'); ";

        cmd += $"$synth.Rate = {rate}; ";
        cmd += $"$synth.Volume = {volume}; ";
        cmd += $"$synth.Speak('{text}'); ";
        cmd += "$synth.Dispose()";

        ProcessStartInfo psi = new ProcessStartInfo()
        {
            FileName = "powershell.exe",
            Arguments = $"-WindowStyle Hidden -Command \"{cmd}\"",
            CreateNoWindow = true,
            UseShellExecute = false
        };

        Process.Start(psi);
    }

    // Devuelve lista de voces instaladas desde PowerShell
    public static List<string> GetVoices()
    {
        List<string> voices = new List<string>();

        string cmd = "Add-Type -AssemblyName System.Speech; " +
                     "$synth = New-Object System.Speech.Synthesis.SpeechSynthesizer; " +
                     "$synth.GetInstalledVoices() | ForEach-Object { $_.VoiceInfo.Name }";

        ProcessStartInfo psi = new ProcessStartInfo()
        {
            FileName = "powershell.exe",
            Arguments = $"-Command \"{cmd}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process p = Process.Start(psi);
        string output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();

        foreach (string voice in output.Split('\n'))
            if (!string.IsNullOrWhiteSpace(voice))
                voices.Add(voice.Trim());

        return voices;
    }

    void Start()
    {
        Speak("Hola, selector de voces funcionando.");
    }
}
