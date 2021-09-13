using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI_System : MonoBehaviour
{
    #region Variables
    [Header("Main Properties")]
    public UI_Screen m_StartScreen;

    [Header("System Events")]
    public UnityEvent onSystemStart = new UnityEvent();
    public UnityEvent onSwitchedScreen = new UnityEvent();

    [Header("Fader Properties")]
    public Image m_Fader;
    public float m_FadeInDuration = 1f;
    public float m_FadeOutDuration = 1f;

    private Component[] screens = new Component[0];

    private UI_Screen prevPreviousScreen;//to avoid looping between previous screens
    public UI_Screen previousScreen;
    public UI_Screen PreviousScreen { get { return previousScreen; } }

    private UI_Screen currentScreen;
    public UI_Screen CurrentScreen { get { return currentScreen; } }

    public static UI_System Instance;
    #endregion


    #region Main Methods
    private void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start()
    {
        if (onSystemStart != null)
            onSystemStart.Invoke();

        screens = GetComponentsInChildren<UI_Screen>(true);
        InitializeScreens();

        if (m_StartScreen)
        {
            SwitchScreens(m_StartScreen);
        }

        if (m_Fader)
        {
            m_Fader.gameObject.SetActive(true);
        }
        FadeIn();
    }
    #endregion

    #region Helper Methods
    public void SwitchScreens(UI_Screen aScreen)
    {
        if (aScreen)
        {
            if (currentScreen == aScreen) return;

            if (currentScreen)
            {
                currentScreen.CloseScreen();
                previousScreen = currentScreen;
            }

            currentScreen = aScreen;
            currentScreen.gameObject.SetActive(true);
            currentScreen.StartScreen();

            if (onSwitchedScreen != null)
            {
                onSwitchedScreen.Invoke();
            }
        }
    }
    IEnumerator SwitchScreensDelayed(UI_Screen aScreen, float time)
    {
        if (aScreen)
        {
            if (currentScreen)
            {
                currentScreen.CloseScreen();
                previousScreen = currentScreen;
            }
            float animCloseTime = currentScreen.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animCloseTime * 0.5f);

            currentScreen = aScreen;
            currentScreen.gameObject.SetActive(true);
            currentScreen.StartScreen();

            if (onSwitchedScreen != null)
            {
                onSwitchedScreen.Invoke();
            }
        }
    }
    public void FadeIn()
    {
        if (m_Fader)
        {
            m_Fader.CrossFadeAlpha(0f, m_FadeInDuration, false);
        }
    }

    public void FadeOut()
    {
        if (m_Fader)
        {
            m_Fader.CrossFadeAlpha(1f, m_FadeOutDuration, false);
        }
    }

    //timer to prevent back button spam
    float backButtonTimer = 0;
    IEnumerator ButtonTimer()
    {
        backButtonTimer = 1;
        while (backButtonTimer > 0)
        {
            backButtonTimer -= Time.deltaTime;
            yield return null;
        }
    }

    public void GoToPreviousScreen()
    {
        if(backButtonTimer > 0)
        {
            return;
        }

        if (previousScreen)
        {
            StartCoroutine(ButtonTimer());
            if (m_StartScreen && previousScreen == m_StartScreen)
            {
                StartCoroutine(SwitchScreensDelayed(previousScreen, 0.5f));
            }
            else if(prevPreviousScreen == currentScreen)
            {
                StartCoroutine(SwitchScreensDelayed(m_StartScreen, 0.5f));
            }
            else
            {
                prevPreviousScreen = previousScreen;
                SwitchScreens(previousScreen);
            }
        }
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(WaitToLoadScene(sceneIndex));
    }

    IEnumerator WaitToLoadScene(int sceneIndex)
    {
        yield return null;
    }

    void InitializeScreens()
    {
        foreach (var screen in screens)
        {
            screen.gameObject.SetActive(true);
        }
    }
    #endregion
}
