using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlameThrower : MonoBehaviour, IConcreteBonus
{
    [SerializeField] float lineSpeed = 5;
    [SerializeField] string uniqueAbility;

    private int boostLevel = 1;

    GameObject leadFirePrefab;
    GameObject shineParticles;
    GridA grid;
    Vector3 firstPos;
    Vector3 curr;
    Vector3 target;

    private bool boostActivated = false;
    private int linesToDestroy = 1;
    private int spriteIndex = 0;

    public void ExecuteBonus()
    {
        grid = GridA.Instance;
        grid.currState = GameState.wait;

        firstPos = new Vector3(10, 0, -5);
        curr = firstPos;
        target = new Vector3(-2, 0, -5);

        leadFirePrefab = Resources.Load<GameObject>("Sprites/BoostSprites/Flamethrower/Lead Fire");
        shineParticles = Resources.Load<GameObject>("Sprites/BoostSprites/Flamethrower/Shine Effect");
        leadFirePrefab = Instantiate(leadFirePrefab, new Vector3(firstPos.x, firstPos.y+0.5f, firstPos.z), Quaternion.Euler(0, 0, -90));
        shineParticles = Instantiate(shineParticles, new Vector3(firstPos.x, firstPos.y + 0.5f, firstPos.z), Quaternion.identity);
        boostActivated = true;
        StartCoroutine(LaunchLine());
    }

    IEnumerator LaunchLine()
    {
        while(boostActivated)
        {
            CreateLine();

            yield return null;
        }
    }

    void CreateLine()
    {
        leadFirePrefab.transform.position = Vector3.MoveTowards(curr, target, lineSpeed * Time.deltaTime);
        shineParticles.transform.position = Vector3.MoveTowards(shineParticles.transform.position, target, lineSpeed * Time.deltaTime);
        curr = Vector3.MoveTowards(curr, target, lineSpeed * Time.deltaTime);
        if (Mathf.Abs(curr.x - (int)curr.x) <= 0.2 && (int)curr.x >= 0 && (int)curr.x < 8)
            grid.FlameThrower((int)curr.x, (int)curr.y);

        if (curr == target)
        {
            if (firstPos.y == linesToDestroy - 1)
            {
                boostActivated = false;
                //Destroy(line);
                Destroy(leadFirePrefab);
                Destroy(shineParticles);
                StartCoroutine(grid.MoveBoxesDown());
                grid.currState = GameState.move;
            }
            else
            {
                firstPos.y++;
                leadFirePrefab.transform.position = new Vector3(leadFirePrefab.transform.position.x, leadFirePrefab.transform.position.y + 1f, firstPos.z);
                shineParticles.transform.position = new Vector3(shineParticles.transform.position.x, shineParticles.transform.position.y + 1, firstPos.z);
                leadFirePrefab.transform.rotation = Quaternion.Euler(0, leadFirePrefab.transform.rotation.eulerAngles.y + 180, -90);
                shineParticles.transform.rotation = Quaternion.Euler(0, shineParticles.transform.rotation.eulerAngles.y + 180, 0);
                target = firstPos;

                curr.y++;
                firstPos = curr;
                CreateLine();
            }
        }
    }
    public Sprite GetSprite()
    {
        return GetComponent<SpriteRenderer>().sprite;
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
        int currLines, nextLines;
        switch (level)
        {
            case 1:
                {
                    currLines = 1;
                    nextLines = 1;
                    break;
                }
            case 2:
                {
                    currLines = 1;
                    nextLines = 1;
                    break;
                }
            case 3:
                {
                    currLines = 1;
                    nextLines = 2;
                    break;
                }
            case 4:
                {
                    currLines = 2;
                    nextLines = 2;
                    break;
                }
            case 5:
                {
                    currLines = 2;
                    nextLines = 2;
                    break;
                }
            case 6:
                {
                    currLines = 2;
                    nextLines = 3;
                    break;
                }
            case 7:
                {
                    currLines = 3;
                    nextLines = 3;
                    break;
                }
            case 8:
                {
                    currLines = 3;
                    nextLines = 3;
                    break;
                }
            case 9:
                {
                    currLines = 3;
                    nextLines = 4;
                    break;
                }
            case 10:
                {
                    currLines = 4;
                    nextLines = -1;
                    break;
                }
            default:
                {
                    currLines = -1;
                    nextLines = -1;
                    break;
                }
        }
        return uniqueAbility + "<color=red>" + currLines + "</color>" + "|" + uniqueAbility + "<color=red>" + nextLines + "</color>";
    }
    public void SetBoostLevel(int lvl)
    {
        boostLevel = lvl;
        switch (boostLevel)
        {
            case 4:
                {
                    linesToDestroy = 2;
                    spriteIndex = 1;
                    break;
                }
            case 5:
                {
                    linesToDestroy = 2;
                    spriteIndex = 1;
                    break;
                }
            case 6:
                {
                    linesToDestroy = 2;
                    spriteIndex = 1;
                    break;
                }
            case 7:
                {
                    linesToDestroy = 3;
                    spriteIndex = 2;
                    break;
                }
            case 8:
                {
                    linesToDestroy = 3;
                    spriteIndex = 2;
                    break;
                }
            case 9:
                {
                    linesToDestroy = 3;
                    spriteIndex = 2;
                    break;
                }
            case 10:
                {
                    linesToDestroy = 4;
                    spriteIndex = 3;
                    break;
                }
        }
    }
}