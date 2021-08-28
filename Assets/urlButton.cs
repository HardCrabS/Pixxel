using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class urlButton : MonoBehaviour
{
    [SerializeField] string url;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OpenURL);
    }

    public void OpenURL()
    {
        Application.OpenURL(url);
    }
}