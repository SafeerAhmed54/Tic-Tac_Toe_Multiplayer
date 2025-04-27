using System;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class GameVisualManager : NetworkBehaviour
{
    private const float GRID_SIZE = 3.1f;

    [SerializeField] private Transform crossPrefab;
    [SerializeField] private Transform circlePrefab;

    [SerializeField] private Transform completeLinePrefeb;

    private List<GameObject> visualGameObjectList;

    private void Awake()
    {
        visualGameObjectList = new List<GameObject>();
    }

    private void Start()
    {
        GameManager.instance.OnClickedOnGridPosition += GameManager_OnClickedOnGridPosition;
        GameManager.instance.OnGameWin += GameManager_OnGameWin;
        GameManager.instance.OnRestart += GameManager_OnRestart;
    }

    private void GameManager_OnRestart(object sender, EventArgs e)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        foreach (GameObject visualGameObject in visualGameObjectList)
        {
            Destroy(visualGameObject);
        }
        visualGameObjectList.Clear();
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        float eularZ = 0f;
        switch (e.line.orientation)
        {
            default:
            case GameManager.Orientation.Horizontal:
                Debug.Log("Horizontal");
                eularZ = 0f;
                break;
            case GameManager.Orientation.Vertical:
                Debug.Log("Vertical");
                eularZ = 90f;
                break;
            case GameManager.Orientation.DiagonalA:
                Debug.Log("DiagonalA");
                eularZ = 45f;
                break;
            case GameManager.Orientation.DiagonalB:
                Debug.Log("DiagonalB");
                eularZ = -45f;
                break;
        }
        Transform completeLineTransform =
        Instantiate(completeLinePrefeb, GetGridWorldPosition(e.line.centerGridPosition.x, e.line.centerGridPosition.y), Quaternion.Euler(0.0f, 0.0f, eularZ));
        completeLineTransform.GetComponent<NetworkObject>().Spawn(true);

        visualGameObjectList.Add(completeLineTransform.gameObject);

    }

    private void GameManager_OnClickedOnGridPosition(object sender, GameManager.OnClickedOnGridPositionEventArgs e)
    {
        Debug.Log("ClickedOnGridPosition" + e.x + ", " + e.y);
        SpawnObjectRpc(e.x, e.y, e.playerType);
    }

    [Rpc(SendTo.Server)]
    private void SpawnObjectRpc(int x, int y, GameManager.PlayerType playerType)
    {
        Debug.Log("SpawnObject");
        Transform prefab;
        switch (playerType)
        {
            default:
            case GameManager.PlayerType.Cross:
                prefab = crossPrefab;
                break;
            case GameManager.PlayerType.Circle:
                prefab = circlePrefab;
                break;
        }
        Transform spawnedCircleTransform = Instantiate(prefab, GetGridWorldPosition(x, y), Quaternion.identity);
        spawnedCircleTransform.GetComponent<NetworkObject>().Spawn(true);
        //spawnedCircleTransform.position = GetGridWorldPosition(x, y);

        visualGameObjectList.Add(spawnedCircleTransform.gameObject);
    }

    private Vector2 GetGridWorldPosition(int x, int y)
    {
        return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
    }
}
