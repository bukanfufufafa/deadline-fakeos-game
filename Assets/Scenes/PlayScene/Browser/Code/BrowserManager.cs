using System.Collections;
using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BrowserManager : MonoBehaviour
{
    [SerializeField] GameObject websiteViewport;
    [SerializeField] TextMeshProUGUI omnibarURL;
    [SerializeField] GameObject loadingIcon;
    [Header("Websites")]
    [SerializeField] GameObject dodolWeb;
    [SerializeField] GameObject shopperWeb;
    [SerializeField] GameObject lapangKerjaWeb;

    private Website currentWebsite;

    void Start()
    {
        NavigateWebsite(null, WebsiteType.Dodol);
    }

    // PUBLIC FUNCTIONS =====================================

    public void NavigateWebsite(Website website, WebsiteType websiteType)
    {
        if (website != null && website != currentWebsite) return;

        switch (websiteType)
        {
            case WebsiteType.Dodol:
                AttempSwitchWebsite(dodolWeb);
                break;
            case WebsiteType.Shopper:
                AttempSwitchWebsite(shopperWeb);
                break;
            case WebsiteType.LapangKerja:
                AttempSwitchWebsite(lapangKerjaWeb);
                break;
        }
    }

    public void NotifyWebsiteURLChange(Website website, string url)
    {
        if (website != currentWebsite) return;

        omnibarURL.SetText(url);
    }

    public void Back()
    {
        NavigateWebsite(currentWebsite, WebsiteType.Dodol);
    }

    // PRIVATE FUNCTIONS =====================================

    private void AttempSwitchWebsite(GameObject websitePrefab)
    {
        if (currentWebsite != null)
        {
            Destroy(currentWebsite.gameObject);
        }

        websiteViewport.SetActive(false);

        var websiteObject = Instantiate(websitePrefab, websiteViewport.transform);

        Website website = websiteObject.GetComponent<Website>();
        website.BrowserManager = this;
        currentWebsite = website;

        omnibarURL.SetText(website.URL);

        LSequence.Create()
           .Append(LMotion.Create(Vector3.zero, new Vector3(0, 0, -720), 2f)
               .WithEase(Ease.Linear)
               .WithOnComplete(() => websiteViewport.SetActive(true))
               .BindToEulerAngles(loadingIcon.transform))
           .Run();
    }
}
