using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    [Header("UI References")]
    [SerializeField] private CanvasGroup mainMenuCanvas;
    [SerializeField] private GameObject LoadingIndicator;
    [SerializeField] private TextMeshProUGUI Username;
    [SerializeField] private GameObject SignInButton;
    [SerializeField] private GameObject ShutdownButton;
    [SerializeField] private GameObject LazyWork;



    [Header("Transition Settings")]
    [SerializeField] private float fadeDuration = 0.5f;

    private int loadingWaitCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        string username = System.Environment.UserName;
        Username.SetText(username);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SignIn()
    {
        StartCoroutine(LoadPlayScene());
    }

    public void Shutdown()
    {
        Application.Quit();
    }

    IEnumerator LoadPlayScene()
    {
        // AsyncOperation operation = SceneManager.LoadSceneAsync("PlayScene");
        // operation.allowSceneActivation = false;

        // LoadingIndicator.SetActive(true);
        // SignInButton.SetActive(false);
        // ShutdownButton.SetActive(false);
        // LazyWork.SetActive(false);

        // while (operation.progress < 0.9f)
        // {
        //     yield return null;
        // }

        // operation.allowSceneActivation = true;
        // // Wait a single frame to allow the new scene's Awake/Start methods to finish
        // yield return null;

        // yield return StartCoroutine(Fade(1f, 0f));
        // mainMenuCanvas.blocksRaycasts = false;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("PlayScene", LoadSceneMode.Additive);
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Scene nextScene = SceneManager.GetSceneByName("PlayScene");
        SceneManager.SetActiveScene(nextScene);

        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.blocksRaycasts = true;
            float timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                mainMenuCanvas.alpha = 1f - (timer / fadeDuration);
                yield return null;
            }

            mainMenuCanvas.alpha = 0f;
        }

        Scene currentScene = gameObject.scene;
        SceneManager.UnloadSceneAsync(currentScene);
    }

    // private IEnumerator Fade(float startAlpha, float endAlpha)
    // {
    //     float elapsedTime = 0f;
    //     mainMenuCanvas.alpha = startAlpha;

    //     while (elapsedTime < fadeDuration)
    //     {
    //         elapsedTime += Time.deltaTime;
    //         mainMenuCanvas.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
    //         yield return null;
    //     }

    //     mainMenuCanvas.alpha = endAlpha;
    // }
}
