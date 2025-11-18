using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

// Atributo vacío solo para marcar el campo
public class VoiceSelectorAttribute : PropertyAttribute { }

[CustomPropertyDrawer(typeof(VoiceSelectorAttribute))]
public class VoiceSelectorDrawer : PropertyDrawer
{
    private List<string> voices;
    private string[] voiceArray;
    private int index = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (voices == null)
        {
            voices = TTS_PS.GetVoices();
            voiceArray = voices.ToArray();

            index = voices.IndexOf(property.stringValue);
            if (index < 0) index = 0;
        }

        index = EditorGUI.Popup(position, label.text, index, voiceArray);

        property.stringValue = voiceArray.Length > 0 ? voiceArray[index] : "";
    }
}
