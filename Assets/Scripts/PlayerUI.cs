using System;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject arrowCross;
    [SerializeField] private GameObject arrowCircle;
    [SerializeField] private GameObject crossYouTextMesh;
    [SerializeField] private GameObject circleYouTextMesh;

    private void Awake()
    {
        arrowCross.SetActive(false);
        arrowCircle.SetActive(false);
        crossYouTextMesh.SetActive(false);
        circleYouTextMesh.SetActive(false);
    }

    private void Start()
    {
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.Instance.OnCurrentPlayablePlayerTypeChanged += GameManager_OnCurrentPlayablePlayerTypeChanged;
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
