using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] GameObject prabowoWindow;
    [SerializeField] GameObject browserWindow;

    void OnGUI()
    {
        if (GUILayout.Button("Prabowo"))
        {
            PrabowoWindow();
        }
        if (GUILayout.Button("Browser"))
        {
            BrowserWindow();
        }
    }

    public void PrabowoWindow()
    {
        var windowObject = Instantiate(prabowoWindow);
        var window = windowObject.GetComponent<MainWindow>();
        window.Open();
    }

    public void BrowserWindow()
    {
        var windowObject = Instantiate(browserWindow);
        var window = windowObject.GetComponent<MainWindow>();
        window.Open();
    }
}
