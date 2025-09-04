using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GlitchText : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private string text;
    private string scrambledText;

    public float revealDuration = 1f;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        text = textMesh.text;
    }

    private void OnEnable()
    {
        StartCoroutine(ScrambleText());
    }
    private IEnumerator ScrambleText()
    {
        int revealedLetters = 0;
        int stringLength = text.Length;

        float revealInterval = revealDuration / stringLength;

        char[] letters = new string(' ', stringLength).ToCharArray();

        float timer = 0;
        while (revealedLetters < stringLength)
        {
            letters[revealedLetters] = (char)('A' + Random.Range(1, 26));
            if (timer >= revealInterval)
            {
                timer = 0;
                letters[revealedLetters] = text[revealedLetters];
                revealedLetters++;
            }
            textMesh.text = new string(letters);

            timer += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
