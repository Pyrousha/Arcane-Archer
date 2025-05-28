using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float writingSpeed = 50;

    [SerializeField] private Color textColor;
    [SerializeField] private Color coloredTextColor;
    public Color ColoredTextColor => coloredTextColor;
    [SerializeField] private AudioSource voiceAudioSource;
    private float voiceVol;

    public Color TextColor => textColor;

    public bool IsRunning { get; private set; }
    private TMP_Text currTextLabel;

    private readonly List<Punctuation> punctuations = new List<Punctuation>()
    {
        new Punctuation(new HashSet<char>(){'.','!','?'}, 0.6f),
        new Punctuation(new HashSet<char>(){','}, 0.3f),
        new Punctuation(new HashSet<char>(){' '}, 0.01f)
    };

    private Coroutine typingCoroutine = null;

    private void Start()
    {
        voiceVol = voiceAudioSource.volume;
    }

    public void ShowText(string textToType, TMP_Text textLabel, AudioClip voiceClip, List<int> coloredIndices = null)
    {
        IsRunning = true;
        currTextLabel = textLabel;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(textToType, textLabel, voiceClip, coloredIndices));
    }

    public void Stop()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        voiceAudioSource.Stop();
        IsRunning = false;

        if (currTextLabel != null)
        {
            currTextLabel.color = textColor;
            currTextLabel = null;
        }
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel, AudioClip voiceClip, List<int> coloredIndices = null)
    {
        if (coloredIndices == null)
            coloredIndices = new List<int>();

        bool pressedSkip = false;

        textLabel.text = textToType;
        textLabel.maxVisibleCharacters = 0;

        TMP_TextInfo textInfo = textLabel.textInfo;

        IsRunning = true;

        float t = 0;
        int charIndex = 0;

        if (voiceClip != null)
        {
            voiceAudioSource.clip = voiceClip;
            voiceAudioSource.time = 0;
            voiceAudioSource.volume = voiceVol * SaveData.CurrSaveData.SfxVol;
            voiceAudioSource.Play();
        }

        yield return null;

        while (charIndex < textInfo.characterCount)
        {
            if (InputHandler.Instance.Interact.Down)
                pressedSkip = true;

            int lastCharIndex = charIndex;

            t += Time.deltaTime * writingSpeed;
            charIndex = Mathf.FloorToInt(t);

            charIndex = Mathf.Clamp(charIndex, 0, textInfo.characterCount);

            if (pressedSkip)
                charIndex = textInfo.characterCount;

            textLabel.maxVisibleCharacters = charIndex;

            for (int i = lastCharIndex; i < charIndex; i++)
            {
                bool isLast = i >= textInfo.characterCount - 1;

                char currCharacter = textInfo.characterInfo[i].character;
                if (IsPunctuation(currCharacter, out float waitTime) && !isLast && !IsPunctuation(textInfo.characterInfo[i + 1].character, out _))
                {
                    if (voiceClip != null)
                    {
                        voiceAudioSource.time = 0;
                        voiceAudioSource.volume = voiceVol * SaveData.CurrSaveData.SfxVol;
                        voiceAudioSource.Play();
                    }

                    if (!pressedSkip)
                        yield return new WaitForSeconds(waitTime);
                }
            }

            if (!pressedSkip)
                yield return null;
        }

        IsRunning = false;

        TutorialText.Instance.OnTextDoneTyping();
        voiceAudioSource.Stop();
    }

    private bool IsPunctuation(char character, out float waitTime)
    {
        foreach (Punctuation punctuationCategory in punctuations)
        {
            if (punctuationCategory.punctuations.Contains(character))
            {
                waitTime = punctuationCategory.waitTime;
                return true;
            }
        }

        waitTime = default;
        return false;
    }

    private readonly struct Punctuation
    {
        public readonly HashSet<char> punctuations;
        public readonly float waitTime;

        public Punctuation(HashSet<char> hashset, float _waitTime)
        {
            waitTime = _waitTime;
            punctuations = hashset;
        }
    }
}
