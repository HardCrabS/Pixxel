using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarManager : MonoBehaviour 
{
    [SerializeField] Image avatarImage;
    [SerializeField] Transform selectionFrame;
    [SerializeField] Button[] allAvatars;

    private const string AVATARS_LOCATION = "Sprites/Avatars/";

    Sprite currSprite;
	// Use this for initialization
	void Start () 
    {
        CreateInstances();
    }

    void CreateInstances()
    {
        for (int i = 0; i < allAvatars.Length-1; i++)
        {
            GameObject button = allAvatars[i].gameObject;
            allAvatars[i].onClick.AddListener(delegate () { OnButtonClicked(button); });
        }
    }

    void OnButtonClicked(GameObject button)
    {
        selectionFrame.SetParent(button.transform);
        selectionFrame.position = button.transform.position;
        currSprite = button.GetComponent<Image>().sprite;
    }

    public void ConfirmAvatar()
    {
        if(currSprite != null)
        {
            avatarImage.sprite = currSprite;
            string spritePath = AVATARS_LOCATION + currSprite.name;
            DatabaseManager.ChangeAvatar(spritePath);
            GameData.gameData.saveData.playerInfo.spritePath = spritePath;
            GameData.gameData.Save();
        }
    }
}