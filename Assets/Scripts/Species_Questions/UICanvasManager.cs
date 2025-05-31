using UnityEngine;

public class UICanvasManager : MonoBehaviour
{
    public GameObject infoCanvas;
    public GameObject quizCanvas;

    public void ShowInfoCanvas()
    {
        infoCanvas.SetActive(true);
    }

    public void HideInfoCanvas()
    {
        infoCanvas.SetActive(false);
    }

    public void ShowQuizCanvas()
    {
        infoCanvas.SetActive(true);
    }

    public void HideQuizCanvas()
    {
        infoCanvas.SetActive(false);
    }
}