using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static GameManager;

public class GameManager : NetworkBehaviour
{

    public static GameManager instance { get; private set; }

    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;


    public class OnClickedOnGridPositionEventArgs : EventArgs
    {
        public int x;
        public int y;
        public PlayerType playerType;
    }

    public event EventHandler OnGameStarted;
    public event EventHandler<OnGameWinEventArgs> OnGameWin;
    public class OnGameWinEventArgs : EventArgs
    {
        public Line line;
        public PlayerType winPlayerType;
    }
    public event EventHandler OnCurrentPlayablePlayerTypeChange;
    public event EventHandler OnRestart;
    public event EventHandler OnGameTied;
    public enum PlayerType
    {
        None,
        Cross,
        Circle
    }
    public enum Orientation
    {
        Horizontal,
        Vertical,
        DiagonalA,
        DiagonalB
    }
    public struct Line
    {
        public List<Vector2Int> gridVector2IntList;
        public Vector2Int centerGridPosition;
        public Orientation orientation;
    }

    private PlayerType localPlayerType;
    private NetworkVariable<PlayerType> currentPlayablePlayerType = new NetworkVariable<PlayerType>();
    private PlayerType[,] playerTypeArray;
    private List<Line> lineList;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        playerTypeArray = new PlayerType[3, 3];
        lineList = new List<Line>
        {
            // Horizontal Lines

            new Line
            {
                gridVector2IntList = new List<Vector2Int>
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(2, 0)
                },
                centerGridPosition = new Vector2Int(1, 0),
                orientation = Orientation.Horizontal
            },
            new Line
            {
                gridVector2IntList = new List<Vector2Int>
                {
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1),
                    new Vector2Int(2, 1)
                },
                centerGridPosition = new Vector2Int(1, 1),
                orientation = Orientation.Horizontal
            },
            new Line
            {
                gridVector2IntList = new List<Vector2Int>
                {
                    new Vector2Int(0, 2),
                    new Vector2Int(1, 2),
                    new Vector2Int(2, 2)
                },
                centerGridPosition = new Vector2Int(1, 2),
                orientation = Orientation.Horizontal
            },

            //Vertical Lines

            new Line
            {
                gridVector2IntList = new List<Vector2Int>
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, 2)
                },
                centerGridPosition = new Vector2Int(0, 1),
                orientation = Orientation.Vertical
            },

            new Line
            {
                gridVector2IntList = new List<Vector2Int>
                {
                    new Vector2Int(1, 0),
                    new Vector2Int(1, 1),
                    new Vector2Int(1, 2)
                },
                centerGridPosition = new Vector2Int(1, 1),
                orientation = Orientation.Vertical
            },

            new Line
            {
                gridVector2IntList = new List<Vector2Int>
                {
                    new Vector2Int(2, 0),
                    new Vector2Int(2, 1),
                    new Vector2Int(2, 2)
                },
                centerGridPosition = new Vector2Int(2, 1),
                orientation = Orientation.Vertical
            },
            
            //Diagonal Lines
            new Line
            {
                gridVector2IntList = new List<Vector2Int>
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 1),
                    new Vector2Int(2, 2)
                },
                centerGridPosition = new Vector2Int(1, 1),
                orientation = Orientation.DiagonalA
            },

            new Line
            {
                gridVector2IntList = new List<Vector2Int>
                {
                    new Vector2Int(0, 2),
                    new Vector2Int(1, 1),
                    new Vector2Int(2, 0)
                },
                centerGridPosition = new Vector2Int(1, 1),
                orientation = Orientation.DiagonalB
            }
        };
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("On Network Spawn : " + NetworkManager.Singleton.LocalClientId);
        if (NetworkManager.Singleton.LocalClientId == 0)
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
        //else
        //{
        //    currentPlayablePlayerType.Value = PlayerType.Circle;
        //}
        currentPlayablePlayerType.OnValueChanged += (PlayerType oldPlayerType, PlayerType newPlayerType) =>
        {
            Debug.Log("Current Playable Player Type Changed: " + oldPlayerType + " -> " + newPlayerType);
            OnCurrentPlayablePlayerTypeChange?.Invoke(this, EventArgs.Empty);
        };
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            currentPlayablePlayerType.Value = PlayerType.Cross;
            TriggerOnGameStartedRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.Server)]
    public void OnBeforeTransformParentChangedRpc(int x, int y, PlayerType playerType)
    {
        Debug.Log("ClickedOnGridPosition" + x + ", " + y);
        if (playerType != currentPlayablePlayerType.Value)
        {
            Debug.Log("Not your turn");
            return;
        }
        if (playerTypeArray[x, y] != PlayerType.None)
        {
            Debug.Log("Already occupied");
            return;
        }
        playerTypeArray[x, y] = playerType;

        OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionEventArgs
        {
            x = x,
            y = y,
            playerType = playerType,
        });

        switch (currentPlayablePlayerType.Value)
        {
            default:
            case PlayerType.Cross:
                currentPlayablePlayerType.Value = PlayerType.Circle;
                break;
            case PlayerType.Circle:
                currentPlayablePlayerType.Value = PlayerType.Cross;
                break;
        }

        TestWinner();
        //TriggerOnCurrentPlayablePlayerTypeChangedRpc();
    }

    //[Rpc(SendTo.ClientsAndHost)]
    //private void TriggerOnCurrentPlayablePlayerTypeChangedRpc()
    //{
    //    OnCurrentPlayablePlayerTypeChange?.Invoke(this, EventArgs.Empty);  
    //}

    private bool TestWinnerLine(Line line)
    {
        return TestWinnerLine(
            playerTypeArray[line.gridVector2IntList[0].x, line.gridVector2IntList[0].y],
            playerTypeArray[line.gridVector2IntList[1].x, line.gridVector2IntList[1].y],
            playerTypeArray[line.gridVector2IntList[2].x, line.gridVector2IntList[2].y]
            );
    }
    private bool TestWinnerLine(PlayerType aPlayerType, PlayerType bPlayerType, PlayerType cPlayerType)
    {
        return aPlayerType != PlayerType.None && aPlayerType == bPlayerType && bPlayerType == cPlayerType;
    }
    private void TestWinner()
    {
        for (int i = 0; i < lineList.Count; i++)
        //foreach(Line line in lineList)
        {
            Line line = lineList[i];
            if (TestWinnerLine(line))
            {
                Debug.Log("Winner: " + playerTypeArray[line.centerGridPosition.x, line.centerGridPosition.y]);
                currentPlayablePlayerType.Value = PlayerType.None;
                TriggerOnGameWinRpc(i, playerTypeArray[line.centerGridPosition.x, line.centerGridPosition.y]);
                return;
            }
        }

        bool hasTie = true;
        for (int x=0; x < playerTypeArray.GetLength(0); x++)
        {
            for (int y = 0; y < playerTypeArray.GetLength(1); y++)
            {
                if(playerTypeArray[x, y] == PlayerType.None)
                {
                    hasTie = false;
                    break;
                }
            }
        }

        if (hasTie)
        {
            TirggerOnGameTiedRpc();
            //Debug.Log("Tie");
            //currentPlayablePlayerType.Value = PlayerType.None;
            //TriggerOnGameWinRpc(-1, PlayerType.None);
        }
        /*if (TestWinnerLine(playerTypeArray[0, 0], playerTypeArray[1, 0], playerTypeArray[2, 0]))
        {
            Debug.Log("Winner: " + playerTypeArray[0, 0]);
            currentPlayablePlayerType.Value = PlayerType.None;
            OnGameWin?.Invoke(this, new OnGameWinEventArgs
            {
                centerGridPosition = new Vector2Int(1, 0)
            });
        }*/
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void TirggerOnGameTiedRpc()
    {
        OnGameTied?.Invoke(this, EventArgs.Empty);
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameWinRpc(int lineIndex, PlayerType winPlayerType)
    {
        Line line = lineList[lineIndex];
        OnGameWin?.Invoke(this, new OnGameWinEventArgs
        {
            line = line,
            winPlayerType = winPlayerType
        });
    }

    public PlayerType GetLocalPlayerType()
    {
        return localPlayerType;
    }

    public PlayerType GetCurrentPlayablePlayerType()
    {
        return currentPlayablePlayerType.Value;
    }

    [Rpc(SendTo.Server)]
    public void RestartRpc()
    {
        for (int x = 0; x < playerTypeArray.GetLength(0); x++)
        {
            for (int y = 0; y < playerTypeArray.GetLength(1); y++)
            {
                playerTypeArray[x, y] = PlayerType.None;
            }
        }
        currentPlayablePlayerType.Value = PlayerType.Cross;
        TriggerOnRestartRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnRestartRpc()
    {
        OnRestart?.Invoke(this, EventArgs.Empty);
    }
}

