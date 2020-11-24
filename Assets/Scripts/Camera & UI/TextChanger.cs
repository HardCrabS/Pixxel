using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextChanger : MonoBehaviour 
{
    [SerializeField] Text trinketName;
    [SerializeField] Text trinketCondition;
	
    public void ChangeTrinketTextName(string _trinketName)
    {
        trinketName.text = _trinketName;
    }
    public void ChangeTrinketTextCondition(string _condition)
    {
        trinketCondition.text = _condition;
    }

    public void SetDefaultText()
    {
        trinketName.text = "Trinket Name";
        trinketCondition.text = "Requirements";
    }
    void OnEnable()
    {
        SetDefaultText();
    }
}
