using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBubble : MonoBehaviour
{
    [SerializeField] Dialogue dialogue;

    [SerializeField]
    private SequentialText text;

    public IEnumerator PlayDialogue()
    {
        for (int i = 0; i < dialogue.Text.Length; i++)
        {
            text.Clear();
            yield return new WaitForSeconds(0.15f);
            text.PlayMessage(dialogue.Text[i]);
            yield return new WaitWhile(() => text.PlayingMessage);
            yield return new WaitForSeconds(0.25f);
        }
    }
}