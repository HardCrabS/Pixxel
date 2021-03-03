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
    bool interactable = false;

    void Start()
    {
        if (isGameButton)
        {
            bool slotUnlocked = GameData.gameData == null ? false : GameData.gameData.saveData.slotsForBoostsUnlocked[buttonIndex];
            if (slotUnlocked)
            {
                if (boostInfo != null)
                {
                    audioSource = GetComponent<AudioSource>();
                    GetComponent<Image>().color = disabledColor;
                    GetComponent<Image>().sprite = BonusManager.GetBoostImage(boostInfo);
                    buttonReloadAnim = GetComponent<Animation>();

                    int boostLevel = GameData.gameData.GetBoostLevel(boostInfo.id);
                    Type boostType = Type.GetType(boostInfo.BoostTypeString);
                    concreteBonus = gameObject.AddComponent(boostType) as BoostBase;
                    concreteBonus.SetBoostLevel(boostLevel);

                    foreach (AnimationState state in buttonReloadAnim)
                    {
                        state.speed = 1 / boostInfo.GetReloadSpeed(boostLevel);
                    }
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
        GetComponent<Image>().color = disabledColor;
        yield return new WaitForSeconds(3.5f);
        interactable = true;
        GetComponent<Image>().color = Color.white;
    }
    public void SetButtonForGame(Boost boost)
    {
        boostInfo = boost;
    }

    EquipButton equipButton;
    public void GetBoostInfo()
    {
        if(boostInfo == null)
        {
            Debug.LogError("No boostInfo found in button, assign it in the inspector");
            return;
        }
        int level = GameData.gameData.GetBoostLevel(boostInfo.id);

        BoostBase myBonus = GetComponent<BoostBase>();

        ClickOnBoost.Instance.ChangeBoostText(boostInfo);
        LevelUp levelUp = FindObjectOfType<LevelUp>();
        levelUp.boostInfo = boostInfo;
        levelUp.bonus = myBonus;

        if (equipButton == null)
            equipButton = FindObjectOfType<EquipButton>();
        equipButton.currentBonus = myBonus;
        equipButton.bonusSprite = GetComponent<Image>().sprite;
        GetComponentInParent<BonusManager>().currButtonSelected = this;
    }

    public void ActivateBonus()
    {
        if (interactable)
        {
            if (concreteBonus != null)
            {
                concreteBonus.ExecuteBonus();
                buttonReloadAnim.Play();
                audioSource.PlayOneShot(activateBoost);
                interactable = false;
                GetComponent<Image>().color = disabledColor;
            }
        }
        else
        {
            audioSource.PlayOneShot(inactiveBoost);
        }
    }

    public void MakeButtonActive()
    {
        GetComponent<Image>().color = Color.white;
        interactable = true;
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