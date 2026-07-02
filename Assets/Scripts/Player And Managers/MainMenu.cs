using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject ContinueButton;
    public GameObject ResetPanel;

    public AudioClip ClickSound;
    public AudioSource ClickSource;

    public GameObject Fade;

    bool ThereIsASave;
    public void StartGame() {
        if(Saver.SaveExists()) {
            ResetPanel.SetActive(true);
            return;
        }

        GameInitiate();
    }

    public void GameInitiate() {
        if(ThereIsASave) {
            Saver.DeleteSave();
        }
        StartCoroutine(WaitToLoad());
    }

    public void ContinueGame() {
        GameHandler.Load();
        StartCoroutine(WaitToLoad());
    }

    void LoadCurrentScene() {
        SceneManager.LoadScene(GameHandler.CurrentScene);
    }

    public void Awake() {
        ThereIsASave = Saver.SaveExists();

        if(!ThereIsASave)
            ContinueButton.SetActive(false);

        ResetPanel.SetActive(false);
    }

    public void SetClip() {
        ClickSource.clip = ClickSound;
    }

    IEnumerator WaitToLoad() {
        Fade.SetActive(true);
        yield return new WaitForSeconds(3);
        LoadCurrentScene();
    }
}
