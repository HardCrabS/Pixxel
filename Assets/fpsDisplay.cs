﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class fpsDisplay : MonoBehaviour
{
    int m_frameCounter = 0;
    float m_timeCounter = 0.0f;
    float m_lastFramerate = 0.0f;
    public float m_refreshTime = 0.5f;
    Text fpsText;

    private void Start()
    {
        fpsText = GetComponent<Text>();
    }

    void Update()
    {
        if (m_timeCounter < m_refreshTime)
        {
            m_timeCounter += Time.deltaTime;
            m_frameCounter++;
        }
        else
        {
            //This code will break if you set your m_refreshTime to 0, which makes no sense.
            m_lastFramerate = m_frameCounter / m_timeCounter;
            fpsText.text = Mathf.RoundToInt(m_lastFramerate).ToString();
            m_frameCounter = 0;
            m_timeCounter = 0.0f;
        }
    }
}