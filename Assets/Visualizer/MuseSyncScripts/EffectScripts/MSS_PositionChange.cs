using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSS_PositionChange : MonoBehaviour
{
    public enum mode { Simple, Range, Random };
    public mode Mode;
    public bool Smoothing = true;
    public float Smoothness = 1;
    public int FreqMax;
    public int Freq;
    public Vector3 EffectMultiplier;
    private Vector3 StartPosition;
    private Vector3 Avarage;

    private void Start()
    {
        StartPosition = transform.localPosition;
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
                transform.localPosition = StartPosition + (MSS_SpectrumManager.SpectrumData[Freq] * 10 * EffectMultiplier);
            else
                transform.localPosition = Vector3.Lerp(transform.localPosition, StartPosition + (MSS_SpectrumManager.SpectrumData[Freq] * 10 * EffectMultiplier), Smoothness * Time.deltaTime);
        else
        {
            Avarage = Vector3.zero;
            for (int i = Freq; i < FreqMax; i++)
            {
                Avarage += StartPosition + (MSS_SpectrumManager.SpectrumData[Freq] * 10 * EffectMultiplier);
            }
            Avarage /= FreqMax - Freq;
            if (!Smoothing)
                transform.localPosition = Avarage;
            else
                transform.localPosition = Vector3.Lerp(transform.localPosition, Avarage, Smoothness * Time.deltaTime);
        }
    }
}
