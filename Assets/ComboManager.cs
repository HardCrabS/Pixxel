using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ComboManager : MonoBehaviour
{
    [SerializeField] float timeToResetCombo = 3f;
    [SerializeField] GameObject comboGraphics;
    [SerializeField] Image glowImage;
    [SerializeField] Color[] glowColors;
    [SerializeField] TextMeshProUGUI comboPreviousText;
    [SerializeField] TextMeshProUGUI comboCurrentText;

    List<GameObject> comboMatches = new List<GameObject>();
    MatchFinder matchFinder;

    int currCombo = 1;
    float timer;
    bool matchWasAdded = false;
    float glowIntensityDelta;

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
        glowImage.gameObject.SetActive(true);
        glowImage.color = new Color(1, 1, 1, 0);
        glowIntensityDelta = 1f / 50;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)  //reset combo
        {
            ResetCombo();
        }
    }
    void ResetCombo()
    {
        currCombo = 1;
        Color col = glowColors[Random.Range(0, glowColors.Length)];
        col.a = 0;
        glowImage.DOColor(col, 0.5f);
        comboMatches.Clear();
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
            AddGlowImageIntensity();
            if (currCombo % 5 == 0) //display only 5s combos
                StartCoroutine(SetComboText(currCombo));
        }
    }

    void AddGlowImageIntensity()
    {
        Color color = glowImage.color;
        color.a += glowIntensityDelta;
        glowImage.color = color;
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