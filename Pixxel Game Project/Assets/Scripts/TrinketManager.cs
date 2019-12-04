using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum tags
{
    Red,
    Yellow,
    Blue,
    Orange,
    Pink,
    Green
}
public class TrinketManager : MonoBehaviour
{
    [SerializeField] Text trinketText;
    [SerializeField] float minutesToPlay;
    [SerializeField] int trinketTotal = 25;
    public int boxesToDestroy = 10;
    private int trinketEarned = 0;
    public tags tagToDestroy;
    Animation panelAnim;
    GridA grid;
    bool timeIsUp = false;
    bool waitState = false;
    float sec;
    void Start()
    {
        panelAnim = GetComponent<Animation>();
        grid = FindObjectOfType<GridA>();
        sec = minutesToPlay * 60;
        UpdateText();
    }

    void Update()
    {
        PlayForTime();
        if(waitState)
        {
            grid.currState = GameState.wait;
        }
    }

    void UpdateText()
    {
        trinketText.text = trinketEarned.ToString() + "/" + trinketTotal.ToString();
    }

    public void TrinketIsEarned()
    {
        waitState = true;
        GetComponent<Image>().enabled = true;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        panelAnim.Play();
    }

    public void GameStateMove()
    {
        waitState = false;
        GetComponent<Image>().enabled = false;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        FindObjectOfType<GridA>().currState = GameState.move;
        trinketEarned++;
        UpdateText();
    }

    void PlayForTime()
    {
        if (!timeIsUp)
        {
            sec -= Time.deltaTime;
            if (sec <= 0)
            {
                TrinketIsEarned();
                timeIsUp = true;
            }
        }
    }

    public void DestroyBoxAmount()
    {
        boxesToDestroy--;
        if (boxesToDestroy < 0)
        {
            TrinketIsEarned();
            print("is earned");
        }
    }
}
