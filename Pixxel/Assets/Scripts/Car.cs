using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {
    [SerializeField] private float speed = 0.05f;
    BackgroundActivity background;
    float screenWidth;
    float timer = 0;
    void Start()
    {
        timer = Time.time + 2*screenWidth / speed;
    }
    public void SetBackActivity(BackgroundActivity background, float width)
    {
        this.background = background;
        screenWidth = width;
    }

	void Update () 
    {
        transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.left * transform.localScale.x * 10, speed * Time.deltaTime);
        if(Time.time > timer)
        {
            background.totalBackroundObjects--;
            Destroy(gameObject);
        }
    }
}
