using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hole : MonoBehaviour
{
    public bool isFilled;
    public int requiredCount;

    [SerializeField] private Text countText; 
    
    private int _currentCount;
    private int currentCount
    {
        get => _currentCount;
        set
        {
            _currentCount = value;
            countText.text = _currentCount + " / " + requiredCount;

            if (_currentCount == requiredCount)
            {
                isFilled = true;
                GetComponent<Animator>().SetTrigger("Fill");
            }
        }
    }
    
    private void Awake()
    {
        countText = GetComponentInChildren<Text>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            currentCount++;
            other.tag = "Untagged";
        }
    }

#if UNITY_EDITOR
    public void UpdateText()
    {
        GetComponentInChildren<Text>().text = "0/" + requiredCount;
    }
#endif
    
}
