using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : MonoBehaviour, IResettable
{
    [SerializeField] private PlayerController Player;
    [SerializeField] private Slider TapSlider;
    [SerializeField] private Text PercentText;
    
    private void Awake()
    {
        FindObjectOfType<GameManager>().OnReset += ResetToDefault;
        Player.OnFinish += StartLaunching;
        Player.OnLaunched += StopLaunching;
    }

    void Update()
    {
        if (TapSlider.gameObject.activeInHierarchy)
        {
            Player.SpeedModifier = TapSlider.value / 5;
            PercentText.text = (int)TapSlider.value + "%";
        
            if (Input.GetMouseButtonDown(0))
            {
                TapSlider.value += 12;
            }
            else TapSlider.value -= .15f;
        }
    }
    
    public void ResetToDefault()
    {
        TapSlider.minValue = 0;
        TapSlider.maxValue = 100;
        TapSlider.value = 0;
        ActivateBar(false);
    }

    private void StartLaunching() => ActivateBar(true);
    private void StopLaunching() => ActivateBar(false);
    private void ActivateBar(bool activate)
    {
        TapSlider.gameObject.SetActive(activate);
        enabled = activate;    
    }
}