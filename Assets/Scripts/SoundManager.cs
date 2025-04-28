using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Transform placeSFXPrefab;
    [SerializeField] private Transform winSFXPrefab;
    [SerializeField] private Transform loseSFXPrefab;

    private void Start()
    {
        GameManager.instance.OnPlaceObject += GameManager_OnPlaceObject;
        GameManager.instance.OnGameWin += GameManager_OnGameWin;
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if(GameManager.instance.GetLocalPlayerType() == e.winPlayerType)
        {
            Instantiate(winSFXPrefab);
            Destroy(winSFXPrefab.gameObject, 5f);
        }
        else
        {
            Instantiate(loseSFXPrefab);
            Destroy(loseSFXPrefab.gameObject, 5f);
        }
    }

    private void GameManager_OnPlaceObject(object sender, EventArgs e)
    {
        Instantiate(placeSFXPrefab);
        Destroy(placeSFXPrefab.gameObject, 5f);
    }
}
