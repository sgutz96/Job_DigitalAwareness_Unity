using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
public class Manager : MonoBehaviour
{
    [SerializeField]
    private OllamaChatWithDataSet Ollama;

    [SerializeField]
    private Button button;

    [SerializeField]
    private TMP_InputField inputField;


    public Animator animator;
    public TMP_Text ChatRespuesta;

    private void Awake()
    {
        // Asignamos el evento del botón
        button.onClick.AddListener(SentTextmeshPro);
    }
    private void Start()
    {
        Ollama.SendPrompt("Podemos Hablar");
    }
    private void Update()
    {
        if (Ollama.isThink)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }
    public void SentTextmeshPro()
    {
        string text = inputField.text;
        if (text != "")
        {
            Ollama.SendPrompt(text);
            inputField.text = null;

        }
    }

    internal void AnimateCharacter(OllamaChatWithDataSet.Reaction reaction, float t )
    {
        animator.SetBool(reaction.ToString(), true);
        StartCoroutine(AnimateCharacterResert(reaction.ToString(), t));
    }

    IEnumerator AnimateCharacterResert(string v1, float v2)
    {
        yield return new WaitForSeconds(v2);
        animator.SetBool(v1, false);
        // throw new NotImplementedException();
    }
}