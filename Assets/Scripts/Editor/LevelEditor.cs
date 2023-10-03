using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(LevelCreator))][CanEditMultipleObjects]
    public class LevelEditor : UnityEditor.Editor
    {
        private static bool _levelEditingEnabled;
        private static GameObject _currentLevel;
        private static GameObject _currentLevelPrefab;
        
        public override void OnInspectorGUI()
        {
            var creator = (LevelCreator)target;
            
            #region MODIFYREGION
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            
            creator.GetLevel = EditorGUILayout.IntField("GET LEVEL", creator.GetLevel);
            GUI.enabled = creator.GetLevel <= creator.levelData.CurrentLevels.Count && creator.GetLevel >= 0;
            if (GUILayout.Button("MODIFY"))
            {
                creator.ModifyLevel();
                _levelEditingEnabled = true;
            }
            GUILayout.EndHorizontal();
            GUI.enabled = true;

            #endregion

            #region CREATEREGION
            GUILayout.Space(20);
            if (GUILayout.Button("CREATE NEW LEVEL",GUILayout.Height(30)))
            {
                creator.CreateNewLevel();
                _levelEditingEnabled = true;
            }
            #endregion
            
            #region SAVEREGION
            GUI.enabled = _levelEditingEnabled;

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("SAVE LEVEL"))
            {
                creator.SaveNewLevel();
                _levelEditingEnabled = false;
            }
            #endregion

            #region DELETEREGION
            if (GUILayout.Button("DELETE LEVEL"))
            {
                creator.DeleteLevel();
                _levelEditingEnabled = false;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(30);
            GUI.enabled = true;
            #endregion
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(50);
            EditorGUILayout.LabelField("NEW LEVEL COMPONENTS");
            GUILayout.Space(50);
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            DrawDefaultInspector();
        }
        
    }
}
