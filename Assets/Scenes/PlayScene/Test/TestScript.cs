using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] GameObject prabowoWindow;

    void OnGUI()
    {
        if (GUILayout.Button("Prabowo"))
        {
            PrabowoWindow();
        }
    }

    public void PrabowoWindow()
    {
        var windowObject = Instantiate(prabowoWindow);
        var window = windowObject.GetComponent<MainWindow>();
        window.Open();
    }
}
