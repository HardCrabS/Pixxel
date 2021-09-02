using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class RewardedAdsButton : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsListener
{
    [SerializeField] Button _showAdButton;
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOsAdUnitId = "Rewarded_iOS";
    string _adUnitId;

    public UnityEvent RewardPlayer = new UnityEvent();

    void Awake()
    {
        // Get the Ad Unit ID for the current platform:
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsAdUnitId
            : _androidAdUnitId;

        Advertisement.AddListener(this);

        //Disable button until ad is ready to show
        _showAdButton.interactable = false;
        // Configure the button to call the ShowAd() method when clicked:
        _showAdButton.onClick.AddListener(ShowAd);
    }

    // Load content to the Ad Unit:
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);

        if (adUnitId.Equals(_adUnitId))
        {
            // Enable the button for users to click:
            _showAdButton.interactable = true;
        }
    }

    // Implement a method to execute when the user clicks the button.
    public void ShowAd()
    {
        // Disable the button: 
        _showAdButton.interactable = false;
        // Then show the ad:
        Advertisement.Show(_adUnitId, this);
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        /*Debug.Log("Unity Ads Rewarded Ad Completed");
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            // Grant a reward.
            RewardPlayer.Invoke();
            // Load another ad:
            Advertisement.Load(_adUnitId, this);
        }*/
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId) { print("Show start"); }
    public void OnUnityAdsShowClick(string adUnitId) { }

    public void OnUnityAdsReady(string placementId)
    {
        
    }

    public void OnUnityAdsDidError(string message)
    {
        
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        BannerAd.Instance.HideBannerAd();
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (!GameData.gameData.saveData.adsRemoved)
        {
            BannerAd.Instance.ShowBannerAd();
        }
        switch(showResult)
        {
            case ShowResult.Failed:
                {
                    print("Ad Failed");
                    break;
                }
            case ShowResult.Finished:
                {
                    print("Ad Finished");
                    RewardPlayer.Invoke();
                    break;
                }
            case ShowResult.Skipped:
                {
                    print("Ad Skipped");
                    break;
                }
        }
        //load another ad
        LoadAd();
    }
    void OnDestroy()
    {
        Advertisement.RemoveListener(this);
        // Clean up the button listeners:
        _showAdButton.onClick.RemoveAllListeners();
    }
}