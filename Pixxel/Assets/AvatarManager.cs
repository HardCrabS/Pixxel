using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarManager : MonoBehaviour 
{
    [SerializeField] Image avatarImage;
    [SerializeField] Transform selectionFrame;
    [SerializeField] Text confirmButtonText;
    [SerializeField] Button[] allAvatars;
    [SerializeField] int[] avatarCosts;

    private const string AVATARS_LOCATION = "Sprites/Avatars/";

    Sprite currSprite;
    int currButtonIndex = 0;

    public delegate void ConfirmButton();
    public ConfirmButton onConfirmButtonDown;
	// Use this for initialization
	void Start () 
    {
        CreateInstances();
    }

    void CreateInstances()
    {
        bool[] avatars = GameData.gameData.saveData.avatars;
        for (int i = 0; i < allAvatars.Length; i++)
        {
            GameObject button = allAvatars[i].gameObject;
            if (avatars[i] == true)
            {
                allAvatars[i].onClick.AddListener(delegate () { OnButtonClicked(button); });
            }
            else
            {
                int index = i;
                allAvatars[i].onClick.AddListener(delegate () { ClickOnLockedAvatar(button, index); });
            }
        }
    }

    void OnButtonClicked(GameObject button)
    {
        Sprite buttonSprite = button.GetComponent<Image>().sprite;
        if (buttonSprite == avatarImage.sprite)
        {
            confirmButtonText.text = "Equiped!";
        }
        else
        {
            confirmButtonText.text = "Confirm";
        }
        onConfirmButtonDown = EquipAvatar;

        selectionFrame.SetParent(button.transform);
        selectionFrame.position = button.transform.position;
        currSprite = buttonSprite;
    }

    void ClickOnLockedAvatar(GameObject button, int index)
    {
        currButtonIndex = index;
        onConfirmButtonDown = BuyAvatar;

        selectionFrame.SetParent(button.transform);
        selectionFrame.position = button.transform.position;

        confirmButtonText.text = "Cost:\n" + avatarCosts[index];
        currSprite = button.GetComponent<Image>().sprite;
    }

    void EquipAvatar()
    {
        if(currSprite != null)
        {
            avatarImage.sprite = currSprite;
            string spritePath = AVATARS_LOCATION + currSprite.name;
            DatabaseManager.ChangeAvatar(spritePath);
            GameData.gameData.saveData.playerInfo.spritePath = spritePath;
            GameData.gameData.Save();

            confirmButtonText.text = "Equiped!";
        }
    }
    void BuyAvatar()
    {
        if(CoinsDisplay.Instance.GetCoins() >= avatarCosts[currButtonIndex])
        {
            CoinsDisplay.Instance.DecreaseCoins(avatarCosts[currButtonIndex]);
            GameData.gameData.UnlockAvatar(currButtonIndex);

            GameObject button = allAvatars[currButtonIndex].gameObject;
            allAvatars[currButtonIndex].onClick.AddListener(delegate () { OnButtonClicked(button); });

            confirmButtonText.text = "Confirm";
            onConfirmButtonDown = EquipAvatar;
        }
    }

    public void CallConfirmButton()
    {
        onConfirmButtonDown();
    }
}