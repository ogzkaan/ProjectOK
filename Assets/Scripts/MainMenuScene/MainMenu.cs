using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private Slider loadingSlider;

    private bool camAnimationFinished;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Play()
    {
        Debug.Log("Play");
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);

    }
    public void Quit()
    {
        Application.Quit();
    }
    public void StartSceneCouroutine()
    {
        StartCoroutine(LoadLevelAsync());
    }
    IEnumerator LoadLevelAsync()
    {
       AsyncOperation loadOperation = SceneManager.LoadSceneAsync(1);
       while (!loadOperation.isDone)
       {
            float progressValue = Mathf.Clamp01(loadOperation.progress/0.9f);
            loadingSlider.value = progressValue;
            Debug.Log(loadingSlider.value);
            yield return null;
       }
    }
}
