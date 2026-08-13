using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodolWeb : MonoBehaviour
{
    Website website;

    void Awake()
    {
        website = GetComponent<Website>();
    }

    // PUBLIC FUNCTIONS =====================================

    public void GoToShopper()
    {
        website.BrowserManager.NavigateWebsite(website, WebsiteType.Shopper);
    }

    public void GotToLapangKerja()
    {
        website.BrowserManager.NavigateWebsite(website, WebsiteType.LapangKerja);
    }
}
