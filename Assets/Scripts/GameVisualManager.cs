using System;
using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : MonoBehaviour
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
        Transform spawnedCrossTransform = Instantiate(crossPrefab);
        spawnedCrossTransform.GetComponent<NetworkObject>().Spawn(true);
        spawnedCrossTransform.position = GetGridWorldPosition(e.x, e.y);
    }

    private Vector2 GetGridWorldPosition(int x, int y)
    {
        return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
    }
}
