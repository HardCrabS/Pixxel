using UnityEngine;

public class MSS_ScaleChange : MonoBehaviour
{
    public enum mode { Simple, Range, Random };
    public mode Mode;
    public bool Smoothing = true;
    public float Smoothness = 1;
    public int FreqMax;
    public int Freq;
    public Vector3 EffectMultiplier;
    private Vector3 StartHeight;
    private Vector3 Avarage;
    RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        StartHeight = rectTransform.sizeDelta;
        if (Smoothness != 0)
            Smoothness = 10 / Smoothness;
        else
            Smoothing = false;
        if (Mode == mode.Random)
            Freq = Random.Range(Freq, FreqMax + 1);
    }
    void Update()
    {
        if (Mode != mode.Range)
            if (!Smoothing)
                rectTransform.sizeDelta = StartHeight + (MSS_SpectrumManager.SpectrumData[Freq] * 10 * EffectMultiplier);
            else
                rectTransform.sizeDelta = Vector3.Lerp(rectTransform.sizeDelta, StartHeight 
                    + (MSS_SpectrumManager.SpectrumData[Freq] * 10 * EffectMultiplier), Smoothness * Time.deltaTime);
        else
        {
            Avarage = Vector3.zero;
            for (int i = Freq; i < FreqMax; i++)
            {
                Avarage += StartHeight + (MSS_SpectrumManager.SpectrumData[Freq] * 10 * EffectMultiplier);
            }
            Avarage /= FreqMax - Freq;
            if (!Smoothing)
                rectTransform.sizeDelta = Avarage;
            else
                rectTransform.sizeDelta = Vector3.Lerp(rectTransform.sizeDelta, Avarage, Smoothness * Time.deltaTime);
        }
    }
}