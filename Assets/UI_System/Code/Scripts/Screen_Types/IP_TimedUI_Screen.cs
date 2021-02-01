using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IndiePixel.UI
{
    public class IP_TimedUI_Screen : UI_Screen 
    {
        #region Variables
        [Header("Timed Screen Properties")]
        public float m_ScreenTime = 2f;
        public UnityEvent onTimeCompleted = new UnityEvent();

        #endregion

        #region Helper Methods
        public override void StartScreen()
        {
            base.StartScreen();

            StartCoroutine(WaitForTime());
        }

        IEnumerator WaitForTime()
        {
            yield return new WaitForSeconds(m_ScreenTime);

            if(onTimeCompleted != null)
            {
                onTimeCompleted.Invoke();
            }
        }
        #endregion
    }
}
