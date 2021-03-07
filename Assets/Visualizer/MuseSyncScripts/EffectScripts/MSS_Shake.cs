using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSS_Shake : MonoBehaviour
{
    public enum mode { Simple, Range, Random };
    public mode Mode;
    public float ShakeSpeed = 20f;
    public float ShakePower = 0.2f;
    public float Threshold = 0.2f;
    public int FreqMax;
    public int Freq;
    private float PowerMult;
    private Vector3 StartPosition;
    private Vector3 NextPosition;
    private float Avarage;
    public void Start()
    {
        StartPosition = transform.localPosition;
        if (Mode == mode.Random)
            Freq = Random.Range(Freq, FreqMax + 1);
    }
    private void Update()
    {
        if (Mode != mode.Range)
            PowerMult = MSS_SpectrumManager.SpectrumData[Freq] * 10;
        else
        {
            Avarage = 0f;
            for (int i = Freq; i < FreqMax; i++)
            {
                Avarage += MSS_SpectrumManager.SpectrumData[i] * 10;
            }
            Avarage /= FreqMax - Freq;
            PowerMult = Avarage;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, NextPosition, ShakeSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.localPosition, NextPosition) < Threshold)
            PickNextPosition();
    }
    void PickNextPosition()
    {
        NextPosition = StartPosition + new Vector3(Random.Range(-ShakePower, ShakePower) * PowerMult, Random.Range(-ShakePower, ShakePower) * PowerMult, 0f);
    }
}
