using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;
    [SerializeField] private Color tiedColor;
    [SerializeField] private Button rematchBtn;

    private void Awake()
    {
        rematchBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.RematchRpc();
        });
    }

    private void Start()
    {
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
        GameManager.Instance.OnGameTied += GameManager_OnGameTied;
        GameManager.Instance.OnRematch += GameManager_OnRematch;
        Hide();
    }

    private void GameManager_OnGameTied(object sender, EventArgs e)
    {
        resultText.text = "Tied!";
        resultText.color = tiedColor;
        Show();
    }

    private void GameManager_OnRematch(object sender, EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if(e.winPlayerType == GameManager.Instance.GetLocalPlayerType())
        {
            resultText.text = "You Win!";
            resultText.color = winColor;
        }
        else
        {
            resultText.text = "You Lose!";
            resultText.color = loseColor;
        }
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
