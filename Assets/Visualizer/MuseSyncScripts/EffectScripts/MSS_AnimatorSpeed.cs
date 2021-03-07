using UnityEngine;

public class MSS_AnimatorSpeed : MonoBehaviour
{
    public enum mode { Simple, Range, Random };
    public mode Mode;
    public bool Smoothing = true;
    public float Smoothness = 1f;
    public int FreqMax;
    public int Freq;
    public float Intensity = 1f;
    private Animator animator;
    private float Avarage;

    private void Start()
    {
        animator = GetComponent<Animator>();
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
                animator.speed = MSS_SpectrumManager.SpectrumData[Freq] * 10 * Intensity;
            else
                animator.speed = Mathf.Lerp(animator.speed, MSS_SpectrumManager.SpectrumData[Freq] * 10 * Intensity, Smoothness * Time.deltaTime);
        else
        {
            Avarage = 0f;
            for (int i = Freq; i < FreqMax; i++)
            {
                Avarage += MSS_SpectrumManager.SpectrumData[i] * 10 * Intensity;
            }
            Avarage /= FreqMax - Freq;
            if (!Smoothing)
                animator.speed = Avarage;
            else
                animator.speed = Mathf.Lerp(animator.speed, Avarage, Smoothness * Time.deltaTime);
        }
    }
}
