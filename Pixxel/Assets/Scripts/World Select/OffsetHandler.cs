using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffsetHandler : MonoBehaviour {
    [SerializeField] ScrollRect scrollRect;
    Vector2 offset = new Vector2(0.5f, 1);

    public void ListenerForScrollChanges(Vector2 value)
    {
        offset = value;
    }

    public float GetOffset()
    {
        scrollRect.enabled = false;
        return (offset.x - 0.5f) * 350;
    }
}
