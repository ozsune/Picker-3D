using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;
using UnityEngine.Windows;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    private static int _currentlevel;
    public static int currentLevel
    {
        get => _currentlevel;
        set
        {
            _currentlevel = value;
            PlayerPrefs.SetInt("Level Complete Count", _currentlevel);
        }
    }
    public LevelData levelData;
    [SerializeField] private int levelCompleteCount;
    private GameObject currentLevelObject;
    [HideInInspector] public Transform playerStartPoint;
    [HideInInspector] public Vector3 levelSpawnPoint;
    
    private void Awake()
    {
        if(levelCompleteCount > 0 ) PlayerPrefs.SetInt("Level Complete Count", levelCompleteCount - 1);
        currentLevel = PlayerPrefs.GetInt("Level Complete Count");

        LoadLevel();
    }

    public void LoadLevel()
    {
        if (currentLevelObject != null)
            foreach (var item in currentLevelObject.GetComponentsInChildren<Transform>())
            {
                if (item.CompareTag("Level End"))
                {
                    levelSpawnPoint = item.position;
                    item.tag = "Untagged";
                }
            }
        
        currentLevelObject = Instantiate(levelData.CurrentLevels[IsLevelValid()], levelSpawnPoint, Quaternion.identity);
        Instantiate(levelData.Environments[Random.Range(0, levelData.Environments.Length)], levelSpawnPoint, Quaternion.identity, currentLevelObject.transform);

        foreach (var item in currentLevelObject.GetComponentsInChildren<Transform>())
        {
            if (item.CompareTag("Player Start"))
            {
                playerStartPoint = item;
                item.tag = "Untagged";
            }
        }
    }

    private int IsLevelValid()
    {
        return currentLevel >= levelData.CurrentLevels.Count
            ? Random.Range(0, levelData.CurrentLevels.Count)
            : currentLevel;
    }
}
