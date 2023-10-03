using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelCreator : MonoBehaviour
{
    private static GameObject _currentLevel;
    private static GameObject _currentLevelPrefab;
    [Serializable]
    public class LevelComponents
    {
        [HideInInspector]public int LevelLength = 3;
        public int[] holeReqiuredBalls;
        public GameObject[] Collectibles;
        public GameObject platform;
        public GameObject finishRamp;
    }
    private int _getlevel;
    public int GetLevel
    {
        get => _getlevel;
        set => _getlevel = Mathf.Clamp(value, 1, levelData.CurrentLevels.Count == 0 ? 1 : levelData.CurrentLevels.Count);
    }

    public LevelData levelData;
    public LevelComponents levelComponents;
    public void CreateNewLevel()
    {
        Debug.Log("Level Created!!");
        
        var platformEndPoint = Vector3.zero;
        DestroyImmediate(_currentLevel, true);
        _currentLevel = new GameObject("Level");
                
        for (int i = 0; i < levelComponents.LevelLength; i++)
        {
            GameObject currentPlatform = (GameObject)PrefabUtility.InstantiatePrefab(levelComponents.platform);
            currentPlatform.transform.position = platformEndPoint;
            currentPlatform.transform.parent = _currentLevel.transform;

            if(i == 0)
                foreach (var item in currentPlatform.GetComponentsInChildren<Transform>())
                {
                    if (item.name == "Player Start Point") item.tag = "Player Start";
                }
            
            GameObject collectible = (GameObject)PrefabUtility.InstantiatePrefab(levelComponents.Collectibles[i]);
            collectible.transform.position = currentPlatform.transform.position;
            collectible.transform.parent = currentPlatform.transform;
            
            GetCurrentHole(currentPlatform, levelComponents.holeReqiuredBalls[i]);
                    
            foreach (var item in currentPlatform.GetComponentsInChildren<Transform>())
            {
                if (item.name == "Platform End Point") platformEndPoint = item.transform.position;
            }

            if (i == levelComponents.LevelLength - 1)
            {
                GameObject finishRamp = (GameObject)PrefabUtility.InstantiatePrefab(levelComponents.finishRamp);
                finishRamp.transform.position = platformEndPoint;
                finishRamp.transform.parent = _currentLevel.transform;
                
                foreach (var item in finishRamp.GetComponentsInChildren<Transform>())
                {
                    if (item.name == "Level End Point") item.tag = "Level End";
                }
            }
        }
    }

    public void ModifyLevel()
    {
        _currentLevelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Levels/Level " + GetLevel + ".prefab");
        _currentLevel = (GameObject)PrefabUtility.InstantiatePrefab(_currentLevelPrefab);
    }
    
    public void SaveNewLevel()
    {
        if(_currentLevelPrefab != null) PrefabUtility.SaveAsPrefabAssetAndConnect(_currentLevel, "Assets/Levels/" + _currentLevel.name + ".prefab", InteractionMode.AutomatedAction);
        else
        {
            _currentLevelPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(_currentLevel, "Assets/Levels/Level " + (levelData.CurrentLevels.Count + 1) + ".prefab", InteractionMode.AutomatedAction);
            levelData.CurrentLevels.Add(_currentLevelPrefab);
            EditorUtility.SetDirty(levelData);
        }

        DestroyImmediate(_currentLevel);
        _currentLevelPrefab = null;
    }
    
    public void DeleteLevel()
    {
        levelData.CurrentLevels.Remove(_currentLevelPrefab);
        EditorUtility.SetDirty(levelData);
        AssetDatabase.DeleteAsset("Assets/Levels/" + _currentLevel.name + ".prefab");
        DestroyImmediate(_currentLevel);
        RenameLevels();
        
        void RenameLevels()
        {
            for (int i = GetLevel; i < levelData.CurrentLevels.Count + 1; i++)
            {
                AssetDatabase.RenameAsset("Assets/Levels/Level " + (i + 1) + ".prefab", "Level " + (i));
            }
        }
    }
    
    public void GetCurrentHole(GameObject platform, int requiredCount)
    {
        Hole hole = platform.GetComponentInChildren<Hole>();

        hole.requiredCount = requiredCount;
        hole.UpdateText();
    }
}
