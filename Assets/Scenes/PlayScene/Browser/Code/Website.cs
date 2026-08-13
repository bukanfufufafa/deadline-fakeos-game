using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Website : MonoBehaviour
{
    [field: SerializeField] public WebsiteType WebsiteType { get; private set; }
    public BrowserManager BrowserManager { get; set; }

    [SerializeField] private string _url = "blank";
    public string URL
    {
        get => _url;
        set
        {
            _url = value;
            BrowserManager.NotifyWebsiteURLChange(this, value);
        }
    }
}
