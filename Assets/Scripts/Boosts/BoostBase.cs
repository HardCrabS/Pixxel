using UnityEngine;
using UnityEngine.UI;

public abstract class BoostBase : MonoBehaviour
{
    protected int boostLevel = 1;
    protected int spriteIndex = 0;

    protected AudioSource audioSource;

    protected const string resourcesFolder = "Sprites/BoostSprites/";

    public virtual void Start()
    {
        audioSource = GetComponent<AudioSource>(); 
    }
    public virtual void ExecuteBonus()
    {

    }
    public void LevelUpBoost()
    {
        boostLevel++;
    }
    public Sprite GetSprite() => GetComponent<SpriteRenderer>().sprite;
    public Sprite GetSpriteFromImage() => GetComponent<Image>().sprite;
    public int GetSpiteIndex() => spriteIndex;
    public virtual void SetBoostLevel(int lvl)
    {
        boostLevel = lvl;
        if (boostLevel >= 4 && boostLevel <= 6)
        {
            spriteIndex = 1;
        }
        else if (boostLevel >= 7 && boostLevel <= 9)
        {
            spriteIndex = 2;
        }
        else if(boostLevel == 10)
        {
            spriteIndex = 3;
        }
    }
}