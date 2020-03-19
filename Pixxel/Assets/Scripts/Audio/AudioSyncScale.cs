using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSyncScale : AudioSyncer {
    public Vector2 beatScale;
    public Vector2 restScale;

    private IEnumerator MoveToScale(Vector2 _target)
    {
        Vector2 current = transform.localScale;
        Vector2 initial = current;
        float _timer = 0;

        while(current != _target)
        {
            current = Vector2.Lerp(initial, _target, _timer / timeToBeat);
            _timer += Time.deltaTime;

            transform.localScale = current;
            yield return null;
        }
        m_isBeat = false;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if(m_isBeat) { return; }

        transform.localScale = Vector2.Lerp(transform.localScale, restScale, restSmoothTime * Time.deltaTime);
    }
    public override void OnBeat()
    {
        base.OnBeat();

        StopCoroutine("MoveToScale");
        StartCoroutine("MoveToScale", beatScale);
    }
}
