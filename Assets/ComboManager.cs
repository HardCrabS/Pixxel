using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    [SerializeField] float timeToResetCombo = 3f;
    [SerializeField] GameObject comboGraphics;
    [SerializeField] TextMeshProUGUI comboPreviousText;
    [SerializeField] TextMeshProUGUI comboCurrentText;

    List<GameObject> comboMatches = new List<GameObject>();
    MatchFinder matchFinder;

    int currCombo = 1;
    float timer;
    bool matchWasAdded = false;

    public static ComboManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        matchFinder = MatchFinder.Instance;
        timer = timeToResetCombo;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)  //reset combo
        {
            currCombo = 1;
            comboMatches.Clear();
        }
    }
    public int GetCombo()
    {
        return currCombo;
    }
    public void CheckCombo()
    {
        matchWasAdded = false;
        //check every matched block
        foreach (var block in matchFinder.currentMatches)
        {
            //mark as combos by adding to the list
            if (!comboMatches.Contains(block))
            {
                comboMatches.Add(block);
                matchWasAdded = true;
                timer = timeToResetCombo; //update combo timer
            }
        }
        //increment combo after all matched blocks added
        if (matchWasAdded)
        {
            currCombo++;
            if (currCombo % 5 == 0) //display only 5s combos
                StartCoroutine(SetComboText(currCombo));
        }
    }

    IEnumerator SetComboText(int currCombo)
    {
        comboPreviousText.text = (currCombo - 1).ToString();
        comboCurrentText.text = currCombo.ToString();
        comboGraphics.SetActive(true);
        comboGraphics.GetComponent<Animator>().Play("Base Layer.Combo Counter", 0, 0);
        yield return null;
    }
}