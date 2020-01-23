using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AudioSyncColor : AudioSyncer {
    public Color[] colors;
    public Color restColor;
    Image image;
	
	void Start () {
        image = GetComponent<Image>();
	}

    private IEnumerator MoveToColor(Color _target)
    {
        Color current = image.color;
        Color initial = current;
        float _timer = 0;

        while (current != _target)
        {
            current = Color.Lerp(initial, _target, _timer / timeToBeat);
            _timer += Time.deltaTime;

            image.color = current;
            yield return null;
        }
        m_isBeat = false;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (m_isBeat) { return; }

        image.color = Color.Lerp(image.color, restColor, restSmoothTime * Time.deltaTime);
    }

    public override void OnBeat()
    {
        base.OnBeat();

        Color _c = GetColor();
        StopCoroutine("MoveToColor");
        StartCoroutine(MoveToColor(_c));
    }

    private Color GetColor()
    {
        int randIndex = Random.Range(0, colors.Length);
        return colors[randIndex];
    }
}
