using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FPSCounter : MonoBehaviour
{
    private TextMeshProUGUI _fpsText;
    
    private void Awake()
    {
        _fpsText = GetComponent<TextMeshProUGUI>();
    }
    

    private void Update()
    {
        _fpsText.text = $"FPS: {1f / Time.deltaTime}";
        if (1f / Time.deltaTime < 60)
        {
            _fpsText.color = Color.yellow;
        }
        else if (1f / Time.deltaTime < 30)
        {
            _fpsText.color = Color.red;
        }
        else
        {
            _fpsText.color = Color.green;
        }
    }
}
