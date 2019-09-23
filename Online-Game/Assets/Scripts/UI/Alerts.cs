using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Alerts 
{
    public IEnumerator CreateNewAlert(string alertMessage)
    {
        // SceneManager.LoadSceneAsync("Alerts", LoadSceneMode.Additive);
        yield return SceneManager.LoadSceneAsync("Alerts", LoadSceneMode.Additive);
        GameObject.FindObjectOfType<AlertObjects>().AlertText.text = alertMessage;
    }
}
