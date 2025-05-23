using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void LoadARScene()
    {
        SceneManager.LoadScene("AR_Spawn");
    }

    public void LoadUnderWaterScene()
    {
        SceneManager.LoadScene("UnderWater");
    }
}
