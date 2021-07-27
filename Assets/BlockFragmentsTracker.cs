using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFragmentsTracker : MonoBehaviour
{
    [SerializeField] int maxFragmentsOnScreen = 20;
    [SerializeField] int fragmentsToScreenShake = 150;

    int potentialFragmentsCount = 0;
    Queue<GameObject> fragmentsQueue;

    public static BlockFragmentsTracker Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        fragmentsQueue = new Queue<GameObject>();
    }

    public void EnqueueFragment(GameObject frag)
    {
        fragmentsQueue.Enqueue(frag);
        StartCoroutine(IncrementFragmentCounter());

        if(fragmentsQueue.Count >= maxFragmentsOnScreen)
        {
            DestroyFragmet(fragmentsQueue.Dequeue());
        }
    }
    private void DestroyFragmet(GameObject frag)
    {
        Destroy(frag);
    }
    IEnumerator IncrementFragmentCounter()
    {
        potentialFragmentsCount++;
        if(potentialFragmentsCount > fragmentsToScreenShake)
        {
            StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake(0.1f, 0.2f));
            potentialFragmentsCount = 0;
        }
        yield return new WaitForSeconds(1f);
        potentialFragmentsCount = Mathf.Clamp(potentialFragmentsCount - 1, 0, potentialFragmentsCount);
    }
}