using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SequentialText : MonoBehaviour
{
    public System.Action CharacterShown;

    public bool PlayingMessage { get; private set; }
    public bool EndOfVisibleCharacters { get; private set; }

    private Text text;

    private float characterFrequency;

    private Color currentColor = Color.black;
    private bool skipToEnd;

    private const float DEFAULT_FREQUENCY = 10f;

    [SerializeField]
    private Color defaultColor = Color.black;

    [SerializeField]
    private Color specialColor = Color.red;

    [SerializeField] AudioClip typeSound;

    public float voicePitch = 1f;
    AudioSource audioSource;

    void Awake()
    {
        text = GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
        currentColor = defaultColor;
    }

    public void PlayMessage(string message)
    {
        SetToDefaults();

        if (TextActive())
        {
            StartCoroutine(PlayMessageSequence(message));
        }
    }

    public void Clear()
    {
        StopAllCoroutines();
        PlayingMessage = false;

        SetText("");
    }

    public void SkipToEnd()
    {
        skipToEnd = true;
    }

    void SetText(string text)
    {
        if (this.text != null)
        {
            this.text.text = text;
        }
    }

    bool TextActive()
    {
        return text != null && text.gameObject.activeInHierarchy;
    }

    string GetText()
    {
        return text.text;
    }

    private void AppendText(string appendText)
    {
        SetText(GetText() + appendText);
    }

    private void SetToDefaults()
    {
        currentColor = defaultColor;
        characterFrequency = DEFAULT_FREQUENCY;
        skipToEnd = false;
    }

    private void FillTextBoxWithHiddenChars(string message)
    {
        for (int i = 0; i < message.Length; i++)
        {
            AppendText(ColorCharacter(message[i], new Color(0f, 0f, 0f, 0f)));
        }
    }

    private int RemoveFirstHiddenChar()
    {
        int startIndex = GetText().IndexOf("<color=#00000000>");
        SetText(GetText().Remove(startIndex, 26));
        return startIndex;
    }

    private IEnumerator PlayMessageSequence(string message)
    {
        PlayingMessage = true;
        EndOfVisibleCharacters = false;
        SetText("");

        string shown = "";

        FillTextBoxWithHiddenChars(message);

        bool soundWasPlayed = false;
        int index = 0;
        while (message.Length > index)
        {
            if (message[index].ToString() == " ")
            {
                yield return new WaitForSeconds(Skipping() ? 0.01f : 0.025f);
            }
            else
            {
                CharacterShown?.Invoke();
                if (!Skipping())
                {
                    if (!soundWasPlayed) //playing sound every 2 characters
                    {
                        audioSource.PlayOneShot(typeSound);
                        soundWasPlayed = true;
                    }
                    else
                        soundWasPlayed = false;
                }
            }
            shown += message[index];
            if (message == shown)
            {
                EndOfVisibleCharacters = true;
            }

            int insertIndex = RemoveFirstHiddenChar();
            Color color = SpecialSubstring(message, index) ? specialColor : defaultColor;
            string newCharacterWithColorCode = ColorCharacter(message[index], color);
            SetText(GetText().Insert(insertIndex, newCharacterWithColorCode));
            index++;

            if (index > 0 && (message[index - 1] == '.' || message[index - 1] == '?' || message[index - 1] == '!'))
            {
                yield return new WaitForSeconds(Skipping() ? 0.01f : 0.3f);
            }

            float adjustedFrequency = Mathf.Clamp(characterFrequency * 0.01f, 0.01f, 0.2f);

            if (Skipping())
            {
                adjustedFrequency *= 0.1f;
            }

            float waitTimer = 0f;
            while (!skipToEnd && waitTimer < adjustedFrequency)
            {
                waitTimer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        yield return new WaitForSeconds(Skipping() ? 0.05f : 0.15f);
        PlayingMessage = false;
    }

    bool SpecialSubstring(string str, int index)
    {
        int startIndex = str.IndexOf('[');
        int endIndex = str.IndexOf(']');

        if (startIndex < 0 || endIndex < 0) return false;
        return index >= startIndex && index <= endIndex;
    }

    private bool Skipping()
    {
        return Input.anyKey;
    }

    public static string ColorString(string str, Color c)
    {
        string coloredString = "<color=#" + ColorUtility.ToHtmlStringRGBA(c) + ">";
        coloredString += str;
        coloredString += "</color>";

        return coloredString;
    }

    public static string ColorCharacter(char character, Color c)
    {
        return ColorString(character.ToString(), c);
    }
}