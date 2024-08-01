using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image loadingImage;
    [SerializeField] private GameObject player;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    public void UnloadScene(int sceneId)
    {
        StartCoroutine(UnloadSceneAsync(sceneId));
    }

    private IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Additive);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            loadingImage.fillAmount = progressValue;
            yield return null;
        }

        Scene newScene = SceneManager.GetSceneByBuildIndex(sceneId);
        SceneManager.SetActiveScene(newScene);
        PositionPlayerInNewScene();

        loadingScreen.SetActive(false);
        loadingImage.fillAmount = 0;
    }

    private IEnumerator UnloadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneId);

        while (!operation.isDone)
        {
            yield return null;
        }
    }

    private void PositionPlayerInNewScene()
    {
        Transform spawnPoint = GameObject.Find("SpawnPoint")?.transform;
        if (spawnPoint != null)
        {
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation;
        }
        else
        {
            Debug.LogWarning("SpawnPoint not found in the new scene");
        }
    }
}
