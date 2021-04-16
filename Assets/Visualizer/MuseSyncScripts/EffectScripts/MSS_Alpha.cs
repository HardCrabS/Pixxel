using UnityEngine;
using UnityEngine.UI;

public class MSS_Alpha : MonoBehaviour
{
    public enum mode { Simple, Range, Random };
    public mode Mode;
    public bool Smoothing = true;
    public float Smoothness = 1f;
    public int FreqMax;
    public int Freq;
    public float Intensity = 1f;
    private Vector3 StartColor;
    private Image image;
    private float Avarage;

    private void Start()
    {
        //image = GetComponent<Image>();
        //StartColor = new Vector3(image.color.r, image.color.g, image.color.b);
        if (Smoothness != 0)
            Smoothness = 10 / Smoothness;
        else
            Smoothing = false;
        if (Mode == mode.Random)
            Freq = Random.Range(Freq, FreqMax + 1);
    }
    /*void Update()
    {
        if (Mode != mode.Range)
            if (!Smoothing)
                image.color = new Color(StartColor.x, StartColor.y, StartColor.z, MSS_SpectrumManager.SpectrumData[Freq] * 10 * Intensity);
            else
                image.color = new Color(StartColor.x, StartColor.y, StartColor.z, Mathf.Lerp(image.color.a, MSS_SpectrumManager.SpectrumData[Freq] * 10 * Intensity, Smoothness * Time.deltaTime));
        else
        {
            Avarage = 0f;
            for (int i = Freq; i < FreqMax; i++)
            {
                Avarage += MSS_SpectrumManager.SpectrumData[i] * 10 * Intensity;
            }
            Avarage /= FreqMax - Freq;
            if (!Smoothing)
                image.color = new Color(StartColor.x, StartColor.y, StartColor.z, Avarage);
            else
                image.color = new Color(StartColor.x, StartColor.y, StartColor.z, Mathf.Lerp(image.color.a, Avarage, Smoothness * Time.deltaTime));
        }
    }*/
}
