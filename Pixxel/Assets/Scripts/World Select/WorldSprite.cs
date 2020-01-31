using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSprite : MonoBehaviour           //DO NOT CHANGE CHILDREN OBJECT ORDER
{
    [SerializeField] int cost;
    [SerializeField] Text text;
    [SerializeField] string worldName;

    public int worldNumber;
    public bool isUnlocked = false;
    Animation anim;
    AnimationClip clip;
    OffsetHandler offsetHandler;
    WorldManager worldManager;

    Vector2 firstPos;

    void Start()
    {
        if (!isUnlocked)
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
            GetComponent<Button>().onClick.AddListener(LoadMyWorld);
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
        anim[clip.name].time = 0;
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
        
        anim[clip.name].time = 0.001f;
        anim.Play(clip.name);
    }

    void ChangeButtonFunc()
    {
        GetComponent<Button>().onClick.AddListener(CloseBuyPanel);
    }

    void CloseBuyPanel()
    {
        anim[clip.name].speed = -1;
        anim[clip.name].time = anim[clip.name].length - 0.001f;

        anim.Play(clip.name);

        GetComponent<Button>().onClick.AddListener(CreateAnimation);
        GetComponentInParent<ScrollRect>().enabled = true;
    }

    void CallAllButtonsActive()
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

    public void BuyWorld()
    {
        CoinsDisplay coinsDisplay = FindObjectOfType<CoinsDisplay>();
        if(coinsDisplay.GetCoins() >= cost)
        {
            coinsDisplay.DecreaseCoins(cost);
            SerializedLevel level = new SerializedLevel(worldNumber, 0, true);
            SaveSystem.SaveLocalLevelData(level);
            CloseBuyPanel();
            transform.GetChild(1).gameObject.SetActive(false);
            GetComponent<Button>().onClick.AddListener(LoadMyWorld);
        }
    }

    void LoadMyWorld()
    {
        FindObjectOfType<SceneLoader>().LoadConcreteWorld(worldName);
    }
}
