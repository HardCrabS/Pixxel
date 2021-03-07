using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSS_Line : MonoBehaviour
{
    public bool Smoothing = true;
    public float Smoothness = 1f;
    public int StartFreq;
    public int FreqIncrement = 1;
    public int Points = 20;
    public float PositionOffset;
    public float EffectMultiplier = 1;
    private LineRenderer lineRenderer;
    private Vector3 Avarage;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = Points;
        for (int i = 0; i < Points; i++)
        {
            lineRenderer.SetPosition(i, Vector3.right * i * PositionOffset);
        }
        if (Smoothness != 0)
            Smoothness = 10 / Smoothness;
        else
            Smoothing = false;
    }
    void Update()
    {
        if (!Smoothing)
            for(int i = 0; i < Points; i++)
            {
                lineRenderer.SetPosition(i, (Vector3.up * MSS_SpectrumManager.SpectrumData[StartFreq + (i * FreqIncrement)] * 10 * EffectMultiplier) + (Vector3.right * i * PositionOffset));
            }
        else
            for (int i = 0; i < Points; i++)
            {
                lineRenderer.SetPosition(i, Vector3.Lerp(lineRenderer.GetPosition(i), (Vector3.up * MSS_SpectrumManager.SpectrumData[StartFreq + (i * FreqIncrement)] * 10 * EffectMultiplier) + (Vector3.right * i * PositionOffset), Smoothness * Time.deltaTime));
            }
    }
}
