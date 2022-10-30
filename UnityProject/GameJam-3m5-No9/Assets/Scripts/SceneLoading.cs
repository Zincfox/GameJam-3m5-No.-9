using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public interface ISceneLoader
{
    void OnLoadProgressChanged(float progress);
    void OnSceneLoadReady(AsyncOperation asyncOperation);
}

public static class SceneManagement
{
    public static IEnumerator LoadSceneAsync(string sceneName, ISceneLoader loader)
    {
        var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
 
        while (!asyncLoad.isDone)
        {
            var value = Mathf.InverseLerp(0.0f, 1.0f, asyncLoad.progress);
            loader.OnLoadProgressChanged(value);
            if (asyncLoad.progress == 0.9f)
            {
                loader.OnSceneLoadReady(asyncLoad);
                yield break;
            }

            yield return null;
        }
    }
}

public class SceneLoading : MonoBehaviour, ISceneLoader
{
    [SerializeField]
    private string sceneName;

    [SerializeField]
    private Button button;

    private void OnEnable()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        StartCoroutine(SceneManagement.LoadSceneAsync(sceneName,this));
    }

    public void OnLoadProgressChanged(float progress)
    {
        // TODO
    }

    public void OnSceneLoadReady(AsyncOperation asyncOperation)
    {
        asyncOperation.allowSceneActivation = true;
    }
}
