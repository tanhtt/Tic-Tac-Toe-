using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridPosition : MonoBehaviour
{
    [SerializeField] private int x;
    [SerializeField] private int y;

    private void OnMouseDown()
    {
        GameManager.Instance.OnGridPositionSelectedRpc(x, y, GameManager.Instance.GetLocalPlayerType());
    }
}
