using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    private const float GRID_SIZE = 3.1f;

    [SerializeField] private Transform crossPrefab;
    [SerializeField] private Transform circlePrefab;
    [SerializeField] private Transform lineCompletePrefab;

    private List<GameObject> visualGameObjectList;

    private void Awake()
    {
        visualGameObjectList = new List<GameObject>();
    }

    private void Start()
    {
        GameManager.Instance.OnClickedOnGridPosition += GameManager_OnClickedOnGridPosition;
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
        GameManager.Instance.OnRematch += GameManager_OnRematch;
    }

    private void GameManager_OnRematch(object sender, EventArgs e)
    {
        if(!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        foreach (GameObject gameobject in visualGameObjectList)
        {
            Destroy(gameobject);
        }
        visualGameObjectList.Clear();
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        float zEule = 0f;
        switch (e.line.orentation)
        {
            case GameManager.Orentation.Horizontal:
                zEule = 0f;
                break;
            case GameManager.Orentation.Vertical:
                zEule = 90f;
                break;
            case GameManager.Orentation.DiagonalA:
                zEule = 45f;
                break;
            case GameManager.Orentation.DiagonalB:
                zEule = -45f;
                break;
        }

        Transform lineComplete = Instantiate(lineCompletePrefab, GetGridWorldPosition(e.line.centerGridPosition.x, e.line.centerGridPosition.y), Quaternion.Euler(0,0,zEule));
        lineComplete.GetComponent<NetworkObject>().Spawn(true);
        visualGameObjectList.Add(lineComplete.gameObject);
    }

    private void GameManager_OnClickedOnGridPosition(object sender, GameManager.OnClickedOnGridPositionEventArgs e)
    {
        Debug.Log("OnClickeCreate");
        SpawnObjectRpc(e.x, e.y, e.playerType);
    }

    [Rpc(SendTo.Server)]
    private void SpawnObjectRpc(int x, int y, GameManager.PlayerType playerType)
    {
        Debug.Log("SpawnObjectRpc");
        Transform _prefab;
        switch (playerType)
        {
            default:
            case GameManager.PlayerType.Cross:
                _prefab = crossPrefab;
                break;
            case GameManager.PlayerType.Circle:
                _prefab = circlePrefab;
                break;
        }
        Transform newObject = Instantiate(_prefab, GetGridWorldPosition(x,y), Quaternion.identity);
        newObject.GetComponent<NetworkObject>().Spawn(true);
        visualGameObjectList.Add(newObject.gameObject);
    }

    private Vector2 GetGridWorldPosition(int x, int y)
    {
        return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
    }
}
