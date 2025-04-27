using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;
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
        Hide();
    }

    private void GameManager_OnRestart(object sender, EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if (e.winPlayerType == GameManager.instance.GetCurrentPlayablePlayerType())
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
