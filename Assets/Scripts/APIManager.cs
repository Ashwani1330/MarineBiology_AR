using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    [SerializeField] private string gasUrl;
    [SerializeField] private TextMeshProUGUI prompt;
    [SerializeField] private TextMeshProUGUI responseText;

    public void SendPrompt()
    {
        StartCoroutine(SendDataToGas());
    }

    private IEnumerator SendDataToGas()
    {
        WWWForm form = new WWWForm();
        form.AddField("parameter", prompt.text);
        UnityWebRequest www = UnityWebRequest.Post(gasUrl, form);
        
        yield return www.SendWebRequest();
        string response;

        if (www.result == UnityWebRequest.Result.Success)
        {
            response = www.downloadHandler.text;
        }
        else
        {
            response = "There was an error";
        }
        
        Debug.Log(response);
        responseText.text = response;
    }
}
