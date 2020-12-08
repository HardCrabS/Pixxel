using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextChanger : MonoBehaviour 
{
    [SerializeField] Text trinketName;
    [SerializeField] Text trinketRequirements;
	
    public void ChangeTrinketTextName(string _trinketName)
    {
        trinketName.text = _trinketName;
    }
    public void ChangeTrinketTextCondition(string _requirement)
    {
        trinketRequirements.text = _requirement;
    }

    public void SetDefaultText()
    {
        trinketName.text = "Trinket Name";
        trinketRequirements.text = "Requirements";
    }
    void OnEnable()
    {
        SetDefaultText();
    }
}
