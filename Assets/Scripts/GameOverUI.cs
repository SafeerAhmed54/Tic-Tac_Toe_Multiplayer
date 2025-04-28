using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI crossScoreText;
    [SerializeField] private TextMeshProUGUI circleScoreText;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;
    [SerializeField] private Color tiedColor;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        restartButton.onClick.AddListener(() =>
        {
            restartButton.enabled = false; 
            GameManager.instance.RestartRpc();
        });
    }
    private void Start()
    {
        GameManager.instance.OnGameWin += GameManager_OnGameWin;
        GameManager.instance.OnRestart += GameManager_OnRestart;
        GameManager.instance.OnGameTied += GameManager_OnGameTied;
        Hide();
    }

    private void GameManager_OnGameTied(object sender, EventArgs e)
    {
        Show();
        resultText.text = "Tied";
        resultText.color = tiedColor;
    }

    private void GameManager_OnRestart(object sender, EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        Debug.Log("Current Player is : " + e.winPlayerType);
        Debug.Log("Current Playable Player is : " + GameManager.instance.GetCurrentPlayablePlayerType());
        Debug.Log("Boolean : " + (e.winPlayerType == GameManager.instance.GetCurrentPlayablePlayerType()));
        if (e.winPlayerType == GameManager.instance.GetLocalPlayerType())
        {
            resultText.text = "You Win!";
            resultText.color = winColor;
        }
        else 
        {
            resultText.text = "You Lose";
            resultText.color = loseColor;
        }
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
