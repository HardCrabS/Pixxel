using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XPBonus : MonoBehaviour, IConcreteBonus
{
    [SerializeField] bool isGameField = true;
    [SerializeField] float procentForXP = 2;
    [SerializeField] float timeForBonusLast = 10f;
    [SerializeField] string uniqueAbility;
    private int boostLevel = 1;
    private int spriteIndex = 0;
    bool needToResetXPprocent = false;
    float timer;
    GridA grid;

    void Update()
    {
        if (needToResetXPprocent)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                grid.SetXPpointsPerBoxByProcent(1/procentForXP);
                needToResetXPprocent = false;
            }
        }
    }

    public void ExecuteBonus()
    {
        grid = GridA.Instance;
        grid.SetXPpointsPerBoxByProcent(procentForXP);
        needToResetXPprocent = true;
        timer = timeForBonusLast;
    }

    public Sprite GetSprite()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }

    public int GetBoostLevel()
    {
        return boostLevel;
    }
    public Sprite GetSpriteFromImage()
    {
        return GetComponent<Image>().sprite;
    }
    public void LevelUpBoost()
    {
        boostLevel++;
    }
    public int GetSpiteIndex()
    {
        return spriteIndex;
    }
    public string GetUniqueAbility(int level)
    {
        return uniqueAbility;
    }
    public void SetBoostLevel(int lvl)
    {
        boostLevel = lvl;
        switch (boostLevel)
        {

        }
    }
}
