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

    Animation buttonReloadAnim;
    AudioSource audioSource;
    BoostBase concreteBonus;
    Image boostImage;
    float boostReloadDeltaPerMove;
    bool interactable = false;

    void Start()
    {
        boostImage = GetComponent<Image>();
        if (isGameButton)
        {
            bool slotUnlocked = GameData.gameData == null ? false : GameData.gameData.saveData.slotsForBoostsUnlocked[buttonIndex];
            if (slotUnlocked)
            {
                if (boostInfo != null)
                {
                    audioSource = GetComponent<AudioSource>();
                    boostImage.color = disabledColor;
                    boostImage.sprite = BonusManager.GetBoostImage(boostInfo);
                    //buttonReloadAnim = GetComponent<Animation>();

                    int boostLevel = GameData.gameData.GetBoostLevel(boostInfo.id);
                    Type boostType = Type.GetType(boostInfo.BoostTypeString);
                    concreteBonus = gameObject.AddComponent(boostType) as BoostBase;
                    concreteBonus.SetBoostLevel(boostLevel);

                    /*foreach (AnimationState state in buttonReloadAnim)
                    {
                        state.speed = 1 / boostInfo.GetReloadSpeed(boostLevel);
                    }*/
                    boostReloadDeltaPerMove = 1 / boostInfo.GetReloadSpeed(boostLevel);
                    StartCoroutine(UnlockAfterCountdown());
                }
                else
                {
                    gameObject.SetActive(false); //hide bonus button if wasnt picked
                }
            }
            else
            {
                Button button = GetComponent<Button>(); //set lock image if panel is locked
                button.interactable = false;
                var colors = button.colors;
                colors.disabledColor = Color.white;
                button.colors = colors;
                gameObject.AddComponent<Shadow>().effectDistance = new Vector2(10, -10);
            }
        }
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
                //buttonReloadAnim.Play();
                boostImage.fillAmount = 0;
                EndGameManager.Instance.onMatchedBlock += FillReloadImage;

                audioSource.PlayOneShot(activateBoost);
                BonusManager.Instance.SetAllButtonsInterraction(false);
                StartCoroutine(WaitForBoostFinish());
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
            EndGameManager.Instance.onMatchedBlock -= FillReloadImage;
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
        Boost boostInfo = levelUp.boostInfo;
        BoostBase bonusToEquip = levelUp.bonus;
        if (bonusToEquip != null)
        {
            GetComponent<Image>().sprite = BonusManager.GetBoostImage(boostInfo);
            GetComponent<AudioSource>().Play();

            GameData.gameData.saveData.equipedBoosts[buttonIndex] = boostInfo.id;
            GameData.Save();
        }
    }
}