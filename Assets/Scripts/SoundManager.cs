using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private GameObject placedSfx;
    [SerializeField] private GameObject winSfx;
    [SerializeField] private GameObject loseSfx;

    private void Start()
    {
        GameManager.Instance.OnPlacedObject += GameManager_OnPlacedObject;
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if(GameManager.Instance.GetLocalPlayerType() == e.winPlayerType)
        {
            SpawnSfx(winSfx);
        }
        else
        {
            SpawnSfx(loseSfx);
        }
    }

    private void GameManager_OnPlacedObject(object sender, System.EventArgs e)
    {
        SpawnSfx(placedSfx);
    }

    private void SpawnSfx(GameObject sfx)
    {
        GameObject newSfx = Instantiate(sfx, Vector3.zero, Quaternion.identity);
        Destroy(newSfx, 1.5f);
    }
}
