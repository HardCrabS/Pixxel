using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog")]
public class DialogState : ScriptableObject
{
    [TextArea(10, 14)] [SerializeField] string dialog;
    [SerializeField] Sprite charakterSprite;
    [SerializeField] string greenButtonText;
    [SerializeField] string redButtonText;
    [SerializeField] DialogState[] nextStates;

    public string StateDialog
    {
        get
        {
            return dialog;
        }
    }

    public DialogState[] NextStates
    {
        get
        {
            return nextStates;
        }
    }
    public string GreenButtonText
    {
        get { return greenButtonText; }
    }
    public string RedButtonText
    {
        get { return redButtonText; }
    }

    public Sprite CharakterSprite
    {
        get { return charakterSprite; }
    }
}