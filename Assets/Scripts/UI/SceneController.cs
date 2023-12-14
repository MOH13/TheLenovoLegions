using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }
    [SerializeField] Animator transitionAnim;

    string? currentlyLoading;

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

    public void NextLevel(string level)
    {
        SaveManager.Save();
        StartCoroutine(LoadLevel(level));
    }

    IEnumerator LoadLevel(string level)
    {
        if (currentlyLoading != null)
            yield break;

        currentlyLoading = level;
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(level);
        transitionAnim.SetTrigger("Start");
        currentlyLoading = null;
    }
}
