using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IResettable
{
    public event Action OnReset;
    public static int DiamondCount;
    public int filledHoleAmount;
    
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameObject startUI;
    [SerializeField] private Text currentLevelText, nextLevelText;
    [SerializeField] private GameObject PartUI;
    [SerializeField] private GameObject WinUI;
    [SerializeField] private GameObject FailUI;
    [SerializeField] private Text DiamondText;

    [SerializeField] private GameObject LandingPath;
    public PlayerController Player;
  
    private void Start()
    {        
        Player = FindObjectOfType<PlayerController>();

        startUI.SetActive(true);
        OnReset += ResetToDefault;
        
        Player.OnWin += GameWin;
        Player.OnFailed += GameFailed;
        Player.OnFinish += LoadNextLevel;
        Player.OnFill += HoleFilled;
        
        OnReset?.Invoke();
    }

    private void HoleFilled()
    {
        PartUI.transform.GetChild(filledHoleAmount).gameObject.SetActive(true);
        filledHoleAmount++;
    }
    
    private void GameWin()
    {
        foreach (var item in LandingPath.GetComponentsInChildren<Collider>())
        {
            item.enabled = false;
            LandingPath.tag = "Untagged";
        }

        PlayerPrefs.SetInt("Diamond Count", DiamondCount);
        DiamondText.text = DiamondCount.ToString();
        Player.transform.DOMove(levelManager.playerStartPoint.position, 2).OnComplete(() => { OnReset?.Invoke();});
        Player.transform.DORotate(levelManager.playerStartPoint.eulerAngles, 1);
        
        WinUI.SetActive(true);
    }
    
    private void GameFailed()
    {
        FailUI.SetActive(true);
    }
    
    private void StartGame()
    {
        startUI.SetActive(false);
        
        Player.OnPlay += Player.VelocityControl;
        Player.OnTurnInput -= StartGame;
    }

    public void LoadNextLevel()
    {
        LandingPath = GameObject.FindGameObjectWithTag("Landing Path");
        LevelManager.currentLevel++;
        levelManager.LoadLevel();
    }

    public void ResetToDefault()
    {
        DiamondCount = PlayerPrefs.GetInt("Diamond Count");
        DiamondText.text = DiamondCount.ToString();
        currentLevelText.text = (LevelManager.currentLevel + 1).ToString();
        nextLevelText.text = (LevelManager.currentLevel + 2).ToString();

        filledHoleAmount = 0;
        for (var i = 0; i < PartUI.transform.childCount; i++)
            PartUI.transform.GetChild(i).gameObject.SetActive(false);
        
        Player.OnTurnInput += StartGame;
    }
    public void ContinueButton()
    {
        WinUI.SetActive(false);
        startUI.SetActive(true);
    }
    
    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
}
