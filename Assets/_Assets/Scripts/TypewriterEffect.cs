using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float writingSpeed = 50;

    [SerializeField] private Color textColor;
    [SerializeField] private Color coloredTextColor;
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

    public void ShowText(string textToType, TMP_Text textLabel, AudioClip voiceClip, List<int> coloredIndices)
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

    private IEnumerator TypeText(string textToType, TMP_Text textLabel, AudioClip voiceClip, List<int> coloredIndices)
    {
        textLabel.text = textToType;

        TMP_TextInfo textInfo = textLabel.textInfo;
        Color32[] newVertexColors;
        textLabel.color = Color.clear;

        IsRunning = true;

        float t = 0;
        int charIndex = 0;

        int numBreaks = Regex.Matches(textToType, "<br>").Count;

        if (voiceClip != null)
        {
            voiceAudioSource.clip = voiceClip;
            voiceAudioSource.time = 0;
            voiceAudioSource.volume = voiceVol * SaveData.CurrSaveData.SfxVol;
            voiceAudioSource.Play();
        }

        yield return null;

        while (charIndex < textToType.Length - numBreaks * 4)
        {
            int lastCharIndex = charIndex;

            t += Time.deltaTime * writingSpeed;
            charIndex = Mathf.FloorToInt(t);

            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            for (int i = lastCharIndex; i < charIndex; i++)
            {
                bool isLast = i >= textToType.Length - 1;

                //Update text color to type text
                {
                    // Get the index of the material used by the current character.
                    int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                    // Get the vertex colors of the mesh used by this text element (character or sprite).
                    newVertexColors = textInfo.meshInfo[materialIndex].colors32;

                    // Get the index of the first vertex used by this text element.
                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                    // Only change the vertex color if the text element is visible.
                    if (textInfo.characterInfo[i].isVisible)
                    {
                        if (coloredIndices.Count > 0 && i == coloredIndices[^1])
                        {
                            newVertexColors[vertexIndex + 0] = coloredTextColor;
                            newVertexColors[vertexIndex + 1] = coloredTextColor;
                            newVertexColors[vertexIndex + 2] = coloredTextColor;
                            newVertexColors[vertexIndex + 3] = coloredTextColor;
                            coloredIndices.RemoveAt(coloredIndices.Count - 1);
                        }
                        else
                        {
                            newVertexColors[vertexIndex + 0] = textColor;
                            newVertexColors[vertexIndex + 1] = textColor;
                            newVertexColors[vertexIndex + 2] = textColor;
                            newVertexColors[vertexIndex + 3] = textColor;
                        }

                        // New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
                        textLabel.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                        // This last process could be done to only update the vertex data that has changed as opposed to all of the vertex data but it would require extra steps and knowing what type of renderer is used.
                        // These extra steps would be a performance optimization but it is unlikely that such optimization will be necessary.
                    }
                }

                if (IsPunctuation(textToType[i], out float waitTime) && !isLast && !IsPunctuation(textToType[i + 1], out _))
                {
                    if (voiceClip != null)
                    {
                        voiceAudioSource.time = 0;
                        voiceAudioSource.volume = voiceVol * SaveData.CurrSaveData.SfxVol;
                        voiceAudioSource.Play();
                    }
                    yield return new WaitForSeconds(waitTime);
                }
            }

            yield return null;
        }

        IsRunning = false;
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
