using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSprite : MonoBehaviour           //DO NOT CHANGE CHILDREN OBJECT ORDER
{
    [SerializeField] int cost;
    [SerializeField] Text text;
    [SerializeField] string worldName;

    [SerializeField] WorldInformation worldInformation;

    public int worldNumber;
    public bool isUnlocked = false;
    Animation anim;
    AnimationClip clip;
    OffsetHandler offsetHandler;
    WorldManager worldManager;
    WorldInfoDisplay infoDisplay;

    Vector2 firstPos;

    void Start()
    {
        if (worldInformation == null || worldInformation != null && !GameData.gameData.saveData.worldUnlocked[worldInformation.WorldIndex])
        {
            firstPos = GetComponent<RectTransform>().anchoredPosition;
            GetComponent<Button>().onClick.AddListener(CreateAnimation);
            if (transform.childCount > 1)
            {
                transform.GetChild(1).gameObject.SetActive(true);
            }
        }
        else
        {
            infoDisplay = FindObjectOfType<WorldInfoDisplay>();
            GetComponent<Button>().onClick.AddListener(OpenWorldInfoPanel);
        }
    }

    public void CreateAnimation()
    {
        worldManager = FindObjectOfType<WorldManager>();
        worldManager.ActivateBlockingPanel();
        offsetHandler = GetComponentInParent<OffsetHandler>();
        float offset = offsetHandler.GetOffset();
        Vector2 parentPos = GetComponentInParent<ScrollRect>().transform.localPosition;
        anim = GetComponent<Animation>();
        clip = anim.clip;
        anim[clip.name].speed = 1;
        anim[clip.name].time = 0.001f;
        if (clip == null)
        {
            Debug.LogError("No clip in the animation component found.");
            return;
        }
        text.text = "Cost: " + cost;

        AnimationCurve curve;
        clip.legacy = true;

        Keyframe[] keys;
        keys = new Keyframe[2];
        keys[0] = new Keyframe(0.0f, firstPos.x);
        keys[1] = new Keyframe(1, offset);
        curve = new AnimationCurve(keys);
        clip.SetCurve("", typeof(Transform), "m_AnchoredPosition.x", curve);

        Keyframe[] keys1;
        keys1 = new Keyframe[2];
        keys1[0] = new Keyframe(0.0f, firstPos.y);
        keys1[1] = new Keyframe(1, 0);
        curve = new AnimationCurve(keys1);
        clip.SetCurve("", typeof(Transform), "m_AnchoredPosition.y", curve);
        
        anim.Play(clip.name);
        print("CREATE ANIMATION");
    }

    void ButtonFuncClosePanel()
    {
        GetComponent<Button>().onClick.AddListener(CloseBuyPanel);
    }

    void ButtonFuncCreateAnim()
    {
        GetComponent<Button>().onClick.AddListener(CreateAnimation);
    }

    void CloseBuyPanel()
    {
        anim[clip.name].speed = -1;
        anim[clip.name].time = anim[clip.name].length;

        anim.Play(clip.name);
        GetComponentInParent<ScrollRect>().enabled = true;
        print("CLOSE PANEL");
    }

    void BlockinPanelOFF()
    {
        worldManager.DeactivatePanel();
    }

    void MakeButtonActive()
    {
        GetComponent<Button>().interactable = !GetComponent<Button>().interactable;
    }

    public void SetBuyPanelActive()
    {
        if (transform.childCount > 0)
        {
            bool active = transform.GetChild(0).gameObject.activeSelf;
            transform.GetChild(0).gameObject.SetActive(!active);
        }
    }

    IEnumerator DestroyAnimation()
    {
        CloseBuyPanel();
        print(anim[clip.name].length);
        yield return new WaitForSeconds(anim[clip.name].length+0.1f);
        Destroy(GetComponent<Animation>());
    }

    public void BuyWorld()
    {
        CoinsDisplay coinsDisplay = FindObjectOfType<CoinsDisplay>();
        if(coinsDisplay.GetCoins() >= cost)
        {
            coinsDisplay.DecreaseCoins(cost);
            GameData.gameData.UnlockWorld(worldNumber);
            StartCoroutine(DestroyAnimation());
            transform.GetChild(1).gameObject.SetActive(false);

            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(OpenWorldInfoPanel);
            infoDisplay = FindObjectOfType<WorldInfoDisplay>();
        }
    }

    void OpenWorldInfoPanel()
    {
        infoDisplay.SetInfoPanel(worldInformation);
    }
}
