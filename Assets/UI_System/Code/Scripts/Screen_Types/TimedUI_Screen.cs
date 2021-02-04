using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedUI_Screen : UI_Screen
{
    #region Variables
    [Header("Timed Screen Properties")]
    public float m_timeBeforeShow = 2f;
    public float m_ScreenTime = 2f;
    public UnityEvent onTimeCompleted = new UnityEvent();

    #endregion

    #region Helper Methods
    public override void StartScreen()
    {
        StartCoroutine(WaitForTime());
    }

    IEnumerator WaitForTime()
    {
        yield return new WaitForSeconds(m_timeBeforeShow);
        base.StartScreen();

        yield return new WaitForSeconds(m_ScreenTime);

        if (onTimeCompleted != null)
        {
            onTimeCompleted.Invoke();
        }
    }
    #endregion
}
