using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldScorePanel : MonoBehaviour
{
    public GameObject title;

    public void onPress()
    {
        title.SetActive(true);
    }

    public void onRelease()
    {
        title.SetActive(false);
    }
}
