using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSS_RotationChange : MonoBehaviour
{
    public enum mode { Simple, Range, Random };
    public mode Mode;
    public bool Smoothing = true;
    public float Smoothness = 1f;
    public int FreqMax;
    public int Freq;
    public Vector3 EffectMultiplier;
    private Vector3 UncompressedRotation;
    private Vector3 StartRotation;
    private Vector3 Avarage;

    private void Start()
    {
        StartRotation = transform.localRotation.eulerAngles;
        UncompressedRotation = transform.localRotation.eulerAngles;
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
                UncompressedRotation = StartRotation + (MSS_SpectrumManager.SpectrumData[Freq] * 10 * EffectMultiplier);
            else
                UncompressedRotation = Vector3.Lerp(UncompressedRotation, StartRotation + (MSS_SpectrumManager.SpectrumData[Freq] * 10 * EffectMultiplier), Smoothness * Time.deltaTime);
        else
        {
            Avarage = Vector3.zero;
            for (int i = Freq; i < FreqMax; i++)
            {
                Avarage += StartRotation + (MSS_SpectrumManager.SpectrumData[Freq] * 10 * EffectMultiplier);
            }
            Avarage /= FreqMax - Freq;
            if (!Smoothing)
                UncompressedRotation = Avarage;
            else
                UncompressedRotation = Vector3.Lerp(UncompressedRotation, Avarage, Smoothness * Time.deltaTime);
        }
        transform.localRotation = Quaternion.Euler(UncompressedRotation);
    }
}
