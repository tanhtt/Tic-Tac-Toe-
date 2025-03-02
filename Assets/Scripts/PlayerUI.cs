using System;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject arrowCross;
    [SerializeField] private GameObject arrowCircle;
    [SerializeField] private GameObject crossYouTextMesh;
    [SerializeField] private GameObject circleYouTextMesh;

    [SerializeField] private TextMeshProUGUI playerCrossScoreText;
    [SerializeField] private TextMeshProUGUI playerCircleScoreText;

    private void Awake()
    {
        arrowCross.SetActive(false);
        arrowCircle.SetActive(false);
        crossYouTextMesh.SetActive(false);
        circleYouTextMesh.SetActive(false);

        playerCrossScoreText.text = "";
        playerCircleScoreText.text = "";
    }

    private void Start()
    {
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.Instance.OnScoreChanged += GameManager_OnScoreChanged;
        GameManager.Instance.OnCurrentPlayablePlayerTypeChanged += GameManager_OnCurrentPlayablePlayerTypeChanged;
    }

    private void GameManager_OnScoreChanged(object sender, EventArgs e)
    {
        GameManager.Instance.GetPlayerScores(out int crossScore, out int circleScore);
        playerCrossScoreText.text = crossScore.ToString();
        playerCircleScoreText.text = circleScore.ToString();
    }

    private void GameManager_OnCurrentPlayablePlayerTypeChanged(object sender, EventArgs e)
    {
        UpdateCurrentArrow();
    }

    private void GameManager_OnGameStarted(object sender, EventArgs e)
    {
        if(GameManager.Instance.GetLocalPlayerType() == GameManager.PlayerType.Cross)
        {
            crossYouTextMesh.SetActive(true);
        }
        else
        {
            circleYouTextMesh.SetActive(true);
        }

        playerCircleScoreText.text = "0";
        playerCrossScoreText.text = "0";

        UpdateCurrentArrow();
    }

    private void UpdateCurrentArrow()
    {
        if(GameManager.Instance.GetCurrentPlayablePlayerType() == GameManager.PlayerType.Cross)
        {
            arrowCross.SetActive(true);
            arrowCircle.SetActive(false);
        }
        else
        {
            arrowCross.SetActive(false);
            arrowCircle.SetActive(true);
        }
    }
}
