using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BonusButton : MonoBehaviour
{
    [SerializeField] bool isGameButton;
    public Image relatedFrame;
    public Boost boostInfo;
    public int buttonIndex; // for equip slots
    public Color disabledColor;
    public AudioClip activateBoost;
    public AudioClip inactiveBoost;

    AudioSource audioSource;
    BoostBase concreteBonus;
    Image boostImage;
    float boostReloadDeltaPerMove;
    bool interactable = false;

    const string LOCK_TAG = "Lock";

    void Start()
    {
        boostImage = GetComponent<Image>();
        if (isGameButton)
        {
            bool slotUnlocked = GameData.gameData == null ? false : GameData.gameData.saveData.slotsForBoostsUnlocked[buttonIndex];
            if (slotUnlocked)
            {
                //disable lock image
                GameObject lockObj = CollectionController.GetChildWithTag(transform, LOCK_TAG);
                if (lockObj != null)
                    lockObj.SetActive(false);

                //if boost was picked 
                if (boostInfo != null)
                {
                    audioSource = GetComponent<AudioSource>();
                    if (AudioController.Instance)
                    {
                        //change volume if sfx volume slider changed
                        AudioController.Instance.onSFXVolumeChange += ChangeButtonVolume;
                        audioSource.volume = AudioController.Instance.SFXVolume;
                    }
                    boostImage.color = disabledColor;
                    boostImage.sprite = BonusManager.GetBoostImage(boostInfo);

                    int boostLevel = GameData.gameData.GetBoostLevel(boostInfo.id);
                    Type boostType = Type.GetType(boostInfo.BoostTypeString);
                    concreteBonus = gameObject.AddComponent(boostType) as BoostBase;
                    concreteBonus.SetBoostLevel(boostLevel);

                    boostReloadDeltaPerMove = 1 / boostInfo.GetMovesToReload(boostLevel);
                    StartCoroutine(UnlockAfterCountdown());
                }
                else
                {
                    //hide bonus button with its lock if wasnt picked
                    gameObject.SetActive(false);
                }
            }
            else
            {
                //disable button for locked boost button
                GetComponent<Button>().interactable = false;
                //disable boost graphics
                GetComponent<Image>().enabled = false;
            }
        }
    }
    void ChangeButtonVolume(float volume)
    {
        if (audioSource)
            audioSource.volume = volume;
    }
    IEnumerator UnlockAfterCountdown()
    {
        DisableButton();
        yield return new WaitForSeconds(3.5f);
        ActivateButton();
    }
    public void SetButtonForGame(Boost boost)
    {
        boostInfo = boost;
    }

    EquipButton equipButton;
    public void GetBoostInfo()
    {
        if (boostInfo == null)
        {
            Debug.LogError("No boostInfo found in button, assign it in the inspector");
            return;
        }
        BoostBase myBonus = GetComponent<BoostBase>();

        ClickOnBoost.Instance.ChangeBoostText(boostInfo);
        LevelUp levelUp = FindObjectOfType<LevelUp>();
        levelUp.boostInfo = boostInfo;
        levelUp.bonus = myBonus;

        if (equipButton == null)
            equipButton = FindObjectOfType<EquipButton>();
        equipButton.currentBonus = myBonus;
        equipButton.bonusSprite = boostImage.sprite;
        BonusManager.Instance.currButtonSelected = this;
    }

    public void ActivateBonus()
    {
        if (interactable)
        {
            if (concreteBonus != null)
            {
                concreteBonus.ExecuteBonus();
                if (!concreteBonus.IsFinished())//if boost hasn't canceled 
                {
                    boostImage.fillAmount = 0;
                    EndGameManager.Instance.onMatchedBlock += FillReloadImage;

                    audioSource.PlayOneShot(activateBoost);
                    BonusManager.Instance.SetAllButtonsInterraction(false);
                    StartCoroutine(WaitForBoostFinish());
                }
            }
        }
        else
        {
            audioSource.PlayOneShot(inactiveBoost);
        }
    }
    void FillReloadImage()
    {
        boostImage.fillAmount += boostReloadDeltaPerMove;
        if (boostImage.fillAmount >= 1)
        {
            ActivateButton();
        }
    }
    IEnumerator WaitForBoostFinish()
    {
        yield return new WaitUntil(() => concreteBonus.IsFinished());
        BonusManager.Instance.SetAllButtonsInterraction(true);
    }
    public void SetInteractable(bool state)
    {
        if (gameObject.activeSelf && state == true && boostImage.fillAmount >= 1)
            ActivateButton();
        else
            DisableButton();
    }

    public void ActivateButton()
    {
        EndGameManager.Instance.onMatchedBlock -= FillReloadImage;
        boostImage.fillAmount = 1;
        boostImage.color = Color.white;
        interactable = true;
    }
    public void DisableButton()
    {
        interactable = false;
        boostImage.color = disabledColor;
    }
    public void EquipBonus()
    {
        LevelUp levelUp = FindObjectOfType<LevelUp>();
        boostInfo = levelUp.boostInfo;
        BoostBase bonusToEquip = levelUp.bonus;
        if (bonusToEquip != null)
        {
            BonusManager.Instance.CheckIfEquipedInOtherSlot(buttonIndex, boostInfo.id);
            GetComponent<Image>().color = new Color(1, 1, 1, 1);
            GetComponent<Image>().sprite = BonusManager.GetBoostImage(boostInfo);
            GetComponent<AudioSource>().Play();

            GameData.gameData.saveData.equipedBoosts[buttonIndex] = boostInfo.id;
            GameData.Save();
        }
    }
}