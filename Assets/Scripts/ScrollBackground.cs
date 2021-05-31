using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBackground : MonoBehaviour
{
    [SerializeField] float scrollSpeed = 0.5f;
    [SerializeField] float acceleration = 0f;
    Material myMaterial;
    Vector2 offset;

    void Start()
    {
        myMaterial = GetComponent<Image>().material;
        offset = new Vector2(scrollSpeed, 0);
    }

    // Update is called once per frame
    void Update()
    {
        offset += new Vector2(acceleration, 0);
        myMaterial.mainTextureOffset += offset * Time.deltaTime;
    }

    public void StopScrolling()
    {
        offset = Vector2.zero;
    }

    public void ResumeScrolling()
    {
        offset = new Vector2(scrollSpeed, 0);
    }
}