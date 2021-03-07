using UnityEngine;

public class MSS_SpectrumManager : MonoBehaviour
{
    public bool UseListener;
    public AudioSource AudioSource;
    public int Resolution = 9;
    public static float[] SpectrumData;

    void Awake()
    {
        if (PlayerPrefsController.GetMasterVisualizer() == 0)
        {
            GameObject canvas = GetComponentInParent<Canvas>().gameObject;
            canvas.SetActive(false);
        }
    }
    void Start()
    {
        if(AudioController.Instance == null)
        {
            GameObject canvas = GetComponentInParent<Canvas>().gameObject;
            canvas.SetActive(false);
        }
        AudioSource = AudioController.Instance.gameObject.GetComponent<AudioSource>();
        SpectrumData = new float[(int)Mathf.Pow(2,Resolution)];
    }

    private void Update()
    {
        if (UseListener)
            AudioListener.GetSpectrumData(SpectrumData, 0, FFTWindow.Hamming);
        else
            AudioSource.GetSpectrumData(SpectrumData, 0, FFTWindow.Hamming);
    }
}