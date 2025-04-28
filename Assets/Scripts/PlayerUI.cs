using System;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject crossArrowGameObject;
    [SerializeField] private GameObject circleArrowGameObject;
    [SerializeField] private GameObject crossYouTextGameObject;
    [SerializeField] private GameObject circleYouTextGameObject;
    [SerializeField] private TextMeshProUGUI playerCrossScoreTextMesh;
    [SerializeField] private TextMeshProUGUI playerCircleScoreTextMesh;

    private void Awake()
    {
        crossArrowGameObject.SetActive(false);
        circleArrowGameObject.SetActive(false);
        crossYouTextGameObject.SetActive(false);
        circleYouTextGameObject.SetActive(false);

        playerCrossScoreTextMesh.text = "";
        playerCircleScoreTextMesh.text = "";
    }

    private void Start()
    {
        GameManager.instance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.instance.OnCurrentPlayablePlayerTypeChange += GameManager_OnCurrentPlayablePlayerTypeChange;
        //GameManager.instance.OnGameWin += GameManager_OnPlayerScoreChange;
        GameManager.instance.OnScoreChanged += GameManager_OnPlayerScoreChange;
    }

    private void GameManager_OnPlayerScoreChange(object sender, EventArgs e)
    {
        GameManager.instance.GetScores(out int playerCrossScore, out int playerCircleScore);
        playerCrossScoreTextMesh.text = playerCrossScore.ToString();
        playerCircleScoreTextMesh.text = playerCircleScore.ToString();
    }

    //private void GameManager_OnPlayerScoreChange(object sender, GameManager.OnGameWinEventArgs e)
    //{
    //    GameManager.instance.GetScores(out int playerCrossScore, out int playerCircleScore);
    //    playerCrossScoreTextMesh.text = playerCrossScore.ToString();
    //    playerCircleScoreTextMesh.text = playerCircleScore.ToString();
    //}

    private void GameManager_OnCurrentPlayablePlayerTypeChange(object sender, EventArgs e)
    {
        UpdateCurrentArrow();
    }

    private void GameManager_OnGameStarted(object sender, EventArgs e)
    {
        if(GameManager.instance.GetLocalPlayerType() == GameManager.PlayerType.Cross)
        {
            crossYouTextGameObject.SetActive(true);
        }
        else
        {
            circleYouTextGameObject.SetActive(true);
        }

        playerCrossScoreTextMesh.text = "0";
        playerCircleScoreTextMesh.text = "0";

        UpdateCurrentArrow();
    }

    private void UpdateCurrentArrow()
    {
        if(GameManager.instance.GetCurrentPlayablePlayerType() == GameManager.PlayerType.Cross)
        {
            crossArrowGameObject.SetActive(true);
            circleArrowGameObject.SetActive(false);
        }
        else
        {
            crossArrowGameObject.SetActive(false);
            circleArrowGameObject.SetActive(true);
        }
    }
}
