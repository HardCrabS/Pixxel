using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBackground : MonoBehaviour
{
    public static ScrollBackground Instance;

    [SerializeField] float scrollSpeed = 0.5f;
    [SerializeField] float acceleration = 0f;
    Material myMaterial;
    Vector2 offset;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        myMaterial = GetComponent<Renderer>().material;
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