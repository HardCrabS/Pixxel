using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShareButton : MonoBehaviour
{
    [SerializeField] string title;
    [TextArea(2, 4)] [SerializeField] string description;
    [SerializeField] Texture2D texture;
    public bool shareWithScreenshot = false;

    const string websiteLink = "https://culagames.com/";

    public UnityEvent OnShareSuccess = new UnityEvent();

    private void Start()
    {
        if (shareWithScreenshot)
            GetComponent<Button>().onClick.AddListener(ShareWithScreenshot);
        else
            GetComponent<Button>().onClick.AddListener(ShareWithTexture);       
    }
    public void ShareWithScreenshot()
    {
        if (description.Contains("[score]"))
        {
            description = description.Replace("[score]", Score.Instance.GetCurrentScore().ToString());
        }
        StartCoroutine(TakeScreenshotAndShare());
    }
    private IEnumerator TakeScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);

        new NativeShare().AddFile(filePath)
            .SetSubject(title).SetText(description).SetUrl(websiteLink)
            .SetCallback(ShareCallbask)//(result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget)
            .Share();


        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
    }

    public void ShareWithTexture()
    {
        new NativeShare().AddFile(texture).SetSubject(title).SetText(description).SetUrl(websiteLink)
        .SetCallback(ShareCallbask)
        .Share();
    }

    void ShareCallbask(NativeShare.ShareResult result, string shareTarget)
    {
        if (result == NativeShare.ShareResult.Shared)
        {
            OnShareSuccess.Invoke();
        }
    }
}