using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldScorePanel : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Image profileImage;
    [SerializeField] Image bannerImage;
    [SerializeField] Text titleText;
    public GameObject title;

    private void Start()
    {
        
    }

    public void onPress()
    {
        title.SetActive(true);
    }

    public void onRelease()
    {
        title.SetActive(false);
    }
    public void SetScorePanel(string _name, Sprite _profileImage = null, string bannerPath = null, string _title = null)
    {
        nameText.text = _name;
        if (_profileImage != null)
            profileImage.sprite = _profileImage;
        if (!string.IsNullOrEmpty(_title))
            titleText.text = _title;
        if(!string.IsNullOrEmpty(bannerPath))
            ProfileHandler.SetBannerFromString(bannerImage, bannerPath);
    }
}