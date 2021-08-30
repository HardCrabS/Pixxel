using UnityEngine;
using UnityEngine.Advertisements;

public class VideoAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsListener
{
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOsAdUnitId = "Rewarded_iOS";
    string _adUnitId;

    bool adLoadingCopleted = false;
    bool adFinished = false;

    public VideoAd()
    {
        // Get the Ad Unit ID for the current platform:
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsAdUnitId
            : _androidAdUnitId;

        Advertisement.AddListener(this);
    }
    // Load content to the Ad Unit:
    public void LoadAd()
    {
        adLoadingCopleted = false;
        adFinished = false;
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }
    public bool AdLoadingCompleted()
    {
        return adLoadingCopleted;
    }
    public bool IsFinished()
    {
        return adFinished;
    }
    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        adLoadingCopleted = true;
        Debug.Log("Ad Loaded: " + adUnitId);
    }

    // Implement a method to execute when the user clicks the button.
    public void ShowAd()
    {
        // Then show the ad:
        Advertisement.Show(_adUnitId, this);
    }
    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("Ads show complete");
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            // Grant a reward.

            // Load another ad:
            Advertisement.Load(_adUnitId, this);
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        adLoadingCopleted = true;
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    public void OnUnityAdsReady(string placementId)
    {
        adLoadingCopleted = true;
    }

    public void OnUnityAdsDidError(string message)
    {
        adLoadingCopleted = true;
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("Ads did start");
        BannerAd.Instance.HideBannerAd();
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        adFinished = true;
        BannerAd.Instance.ShowBannerAd();
        switch (showResult)
        {
            case ShowResult.Failed:
                {
                    Debug.Log("Ad Failed");
                    break;
                }
            case ShowResult.Finished:
                {
                    Debug.Log("Ad Finished");
                    break;
                }
            case ShowResult.Skipped:
                {
                    Debug.Log("Ad Skipped");
                    break;
                }
        }
        Advertisement.RemoveListener(this);
    }
}