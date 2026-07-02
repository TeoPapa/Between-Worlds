using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject HealthBar;
    public Image HealthMeter;

    public GameObject FadeImage;
    public Animator FadeAnim;

    public GameObject PauseMenu;

    public GameObject OptionsMenu;
    public GameObject HelpMenu;

    public GameObject MainMenu;
    public GameObject Exit;

    public GameObject PlayerDeath;

    bool Paused = false;

    bool WasCursorVisible;

    private void Awake() {
        Paused = false;
        MainMenu.SetActive(false);
        Exit.SetActive(false);
        OptionsMenu.SetActive(false);
        HelpMenu.SetActive(false);
        PauseMenu.SetActive(false);
    }
    private void Start() {
        if (FadeAnim == null)
            FadeAnim = this.GetComponent<Animator>();

        HealthBar.SetActive(false);
        PlayerDeath.SetActive(false);
    }

    #region Fade
    public void FadeIn() {
        FadeImage.SetActive(true);
        FadeAnim.SetTrigger("FadeIn");
    }
    public void FadeOut() {
        FadeAnim.SetTrigger("FadeOut");
        if(PlayerDeath.activeSelf) PlayerDeath.GetComponent<Animator>().SetTrigger("Out");
    }

    public void Faded() {
        FadeImage.SetActive(false);
    }
    #endregion

    public void OpenPauseMenu() {
        WasCursorVisible = Cursor.visible;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Paused = true;
        PauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameManager.Instance.GetAudioManager().PauseAudio();
    }

    public void ClosePauseMenu() {
        if(!WasCursorVisible) {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        OptionsMenu.SetActive(false);
        HelpMenu.SetActive(false);
        Paused = false;
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameManager.Instance.GetAudioManager().UnPauseAudio();
    }

    public bool isPaused() {
        return PauseMenu.activeSelf;
    }

    public void PlayerDeathScreen() {
        FadeIn();
        PlayerDeath.SetActive(true);
        PlayerDeath.GetComponent<Animator>().SetTrigger("In");
    }
    public void UpdateHealthBar(int CurrentHealth, int MaxHealth) {
        HealthBar.SetActive(true);
        HealthMeter.fillAmount = (float)CurrentHealth / MaxHealth;
    }

    public void HideHealthBar() {
        HealthBar.SetActive(false);
    }

    public void Continue() {
        StartCoroutine(WaitSeconds(2f, PlayerDeath.GetComponent<Animator>(), "Out",0));
    }

    IEnumerator WaitSeconds(float seconds, Animator anim, string trigger, int id) {
        anim.gameObject.SetActive(true);
        anim.SetTrigger(trigger);
        yield return new WaitForSeconds(seconds);

        switch(id) {
            case 0:
                GameManager.Instance.PlayerDeath();
                break;
            case 1:
                SceneManager.LoadScene(0);
                break;
            case 2:
                Application.Quit();
                break;
        }
    }

    public void ChangeMasterVolume(float amount) {
        GameHandler.MasterVolume = amount;
        GameManager.Instance.GetAudioManager().ChangeVolume("Master", amount);
        GameHandler.Save();
    }

    public void ChangeMusicVolume(float amount) {
        GameHandler.MusicVolume = amount;
        GameManager.Instance.GetAudioManager().ChangeVolume("Music", amount);
        GameHandler.Save();
    }

    public void ChangeVoiceVolume(float amount) {
        GameHandler.DialogueVolume = amount;
        GameManager.Instance.GetAudioManager().ChangeVolume("Voice", amount);
        GameHandler.Save();
    }

    public void ChangeEffectsVolume(float amount) {
        GameHandler.EffectsVolume = amount;
        GameManager.Instance.GetAudioManager().ChangeVolume("Effects", amount);
        GameHandler.Save();
    }

    public void ChangeUIVolume(float amount) {
        GameHandler.UIVolume = amount;
        GameManager.Instance.GetAudioManager().ChangeVolume("UI", amount);
        GameHandler.Save();
    }

    public void GoToMainMenu() {
        StartCoroutine(WaitSeconds(2f, FadeAnim, "FadeIn", 1));
    }

    public void ExitGame() {
        StartCoroutine(WaitSeconds(2f, FadeAnim, "FadeIn", 2));
    }
}
