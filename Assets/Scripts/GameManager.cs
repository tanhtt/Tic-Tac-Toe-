using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static GameManager;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnGameStarted;
    public event EventHandler OnCurrentPlayablePlayerTypeChanged;
    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;
    public class OnClickedOnGridPositionEventArgs : EventArgs
    {
        public int x;
        public int y;
        public PlayerType playerType;
    }

    public event EventHandler<OnGameWinEventArgs> OnGameWin;
    public event EventHandler OnRematch;
    public class OnGameWinEventArgs : EventArgs
    {
        public Line line;
        public PlayerType winPlayerType;
    }

    public enum PlayerType
    {
        None,
        Cross,
        Circle,
    }

    public enum Orentation
    {
        Horizontal,
        Vertical,
        DiagonalA,
        DiagonalB,
    }

    public struct Line
    {
        public List<Vector2Int> gridVecter2IntList;
        public Vector2Int centerGridPosition;
        public Orentation orentation;
    }

    private PlayerType localPlayerType;
    private NetworkVariable<PlayerType> currentPlayablePlayerType = new NetworkVariable<PlayerType>();
    private PlayerType[,] playerTypeArray;
    private List<Line> lineList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("GameManager instance already exists. Destroying duplicate.");
            Destroy(gameObject);
        }

        playerTypeArray = new PlayerType[3, 3];
        lineList = new List<Line>
        {
            // Vertical
            new Line
            {
                gridVecter2IntList = new List<Vector2Int> { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) },
                centerGridPosition = new Vector2Int(0, 1),
                orentation = Orentation.Vertical,
            },
            new Line
            {
                gridVecter2IntList = new List<Vector2Int> { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2) },
                centerGridPosition = new Vector2Int(1, 1),
                orentation = Orentation.Vertical,
            },
            new Line
            {
                gridVecter2IntList = new List<Vector2Int> { new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2) },
                centerGridPosition = new Vector2Int(2, 1),
                orentation = Orentation.Vertical,
            },

            // Horizontal
            new Line
            {
                gridVecter2IntList = new List<Vector2Int> { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },
                centerGridPosition = new Vector2Int(1, 0),
                orentation = Orentation.Horizontal,
            },
            new Line
            {
                gridVecter2IntList = new List<Vector2Int> { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1) },
                centerGridPosition = new Vector2Int(1, 1),
                orentation = Orentation.Horizontal,
            },
            new Line
            {
                gridVecter2IntList = new List<Vector2Int> { new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2) },
                centerGridPosition = new Vector2Int(1, 2),
                orentation = Orentation.Horizontal,
            },

            // Diagonal
            new Line
            {
                gridVecter2IntList = new List<Vector2Int> { new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(2, 2) },
                centerGridPosition = new Vector2Int(1, 1),
                orentation = Orentation.DiagonalA,
            },
            new Line
            {
                gridVecter2IntList = new List<Vector2Int> { new Vector2Int(0, 2), new Vector2Int(1, 1), new Vector2Int(2, 0) },
                centerGridPosition = new Vector2Int(1, 1),
                orentation = Orentation.DiagonalB,
            }
        };
    }

    public override void OnNetworkSpawn()
    {
        if(NetworkManager.Singleton.LocalClientId == 0)
        {
            localPlayerType = PlayerType.Cross;
        }
        else
        {
            localPlayerType = PlayerType.Circle;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        }

        currentPlayablePlayerType.OnValueChanged += (PlayerType previousValue, PlayerType newValue) =>
        {
            OnCurrentPlayablePlayerTypeChanged?.Invoke(this, EventArgs.Empty);
        };
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        if(NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            currentPlayablePlayerType.Value = PlayerType.Cross;
            TriggerOnGameStartedRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.Server)]
    public void OnGridPositionSelectedRpc(int x, int y, PlayerType playerType)
    {
        if(playerType != currentPlayablePlayerType.Value)
        {
            return;
        }

        if (playerTypeArray[x, y] != PlayerType.None)
        {
            return;
        }

        playerTypeArray[x, y] = playerType;

        Debug.Log($"Grid position ({x}, {y}) selected.");
        OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionEventArgs { x = x, y = y, playerType = playerType});

        switch(currentPlayablePlayerType.Value)
        {
            case PlayerType.Cross:
                currentPlayablePlayerType.Value = PlayerType.Circle;
                break;
            case PlayerType.Circle:
                currentPlayablePlayerType.Value = PlayerType.Cross;
                break;
            default:
                break;
        }

        TestWinner();
    }

    private bool TestWinnerLine(PlayerType aPlayerType, PlayerType bPlayerType, PlayerType cPlayerType)
    {
        if (aPlayerType != PlayerType.None && aPlayerType == bPlayerType && bPlayerType == cPlayerType)
        {
            return true;
        }
        return false;
    }

    private void TestWinner()
    {
        for(int i = 0; i < lineList.Count; i++)
        {
            Line line = lineList[i];
            PlayerType aPlayerType = playerTypeArray[line.gridVecter2IntList[0].x, line.gridVecter2IntList[0].y];
            PlayerType bPlayerType = playerTypeArray[line.gridVecter2IntList[1].x, line.gridVecter2IntList[1].y];
            PlayerType cPlayerType = playerTypeArray[line.gridVecter2IntList[2].x, line.gridVecter2IntList[2].y];
            if (TestWinnerLine(aPlayerType, bPlayerType, cPlayerType))
            {
                currentPlayablePlayerType.Value = PlayerType.None;
                TriggerOnGameWinRpc(i, playerTypeArray[line.centerGridPosition.x, line.centerGridPosition.y]);
                break;
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameWinRpc(int lineIndex, PlayerType winPlayerType)
    {
        Line line = lineList[lineIndex];
        OnGameWin?.Invoke(this, new OnGameWinEventArgs { line = line, winPlayerType = winPlayerType });
    }

    [Rpc(SendTo.Server)]
    public void RematchRpc()
    {
        for (int x = 0; x < playerTypeArray.GetLength(0); x++)
        {
            for (int y = 0; y < playerTypeArray.GetLength(1); y++)
            {
                playerTypeArray[x, y] = PlayerType.None;
            }
        }
        currentPlayablePlayerType.Value = PlayerType.Cross;

        TriggerRematchRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerRematchRpc()
    {
        OnRematch?.Invoke(this, EventArgs.Empty);
    }

    public PlayerType GetLocalPlayerType()
    {
        return localPlayerType;
    }

    public PlayerType GetCurrentPlayablePlayerType()
    {
        return currentPlayablePlayerType.Value;
    }
}
