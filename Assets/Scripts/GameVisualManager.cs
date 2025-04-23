using System;
using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    [SerializeField] private Transform crossPrefab;
    [SerializeField] private Transform circlePrefab;

    private const float GRID_SIZE = 3.1f;

    private void Start()
    {
        GameManager.instance.OnClickedOnGridPosition += GameManager_OnClickedOnGridPosition;
    }

    private void GameManager_OnClickedOnGridPosition(object sender, GameManager.OnClickedOnGridPositionEventArgs e)
    {
        Debug.Log("ClickedOnGridPosition" + e.x + ", " + e.y);
        SpawnObjectRpc(e.x, e.y);
    }

    [Rpc(SendTo.Server)]
    private void SpawnObjectRpc(int x , int y)
    {
        Debug.Log("SpawnObject");
        Transform spawnedCircleTransform = Instantiate(circlePrefab);
        spawnedCircleTransform.GetComponent<NetworkObject>().Spawn(true);
        spawnedCircleTransform.position = GetGridWorldPosition(x, y);
    }

    private Vector2 GetGridWorldPosition(int x, int y)
    {
        return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
    }
}
