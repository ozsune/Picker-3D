using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "All Levels", menuName = "Level Data")]
public class LevelData : ScriptableObject
{
   public List<GameObject> CurrentLevels;
   public GameObject[] Environments;
}
