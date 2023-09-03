using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Game : MonoBehaviour {

    [Header("Pause")]

    [SerializeField] private KeyCode _pauseKey;
    [SerializeField] private MonoBehaviour[] _goToDisable;

    [Header("Health Display")]

    [SerializeField] private Image _healthDisplayFill;

    [Header("Menus")]

    [SerializeField] private GameObject _pauseMenu;
    private GameObject _menuCurrent;

    private void Update() {
        if (Input.GetKeyDown(_pauseKey)) PauseGame();
    }

    public void PauseGame() {
        bool b = Time.timeScale == 0;
        Time.timeScale = b ? 1 : 0;

        Cursor.lockState = b ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !b;
        foreach (MonoBehaviour behaviour in _goToDisable) behaviour.enabled = b;
        ShowMenu(b ? null : _pauseMenu);
    }

    public void ShowMenu(GameObject menuToOpen) {
        _menuCurrent?.SetActive(false);
        _menuCurrent = menuToOpen;
        _menuCurrent?.SetActive(true);
    }

    public void LoadScene(int sceneId) {
        SceneManager.LoadScene(sceneId);
    }

    public void ChangeHealthDisplay(float fillAMount) {
        _healthDisplayFill.fillAmount = fillAMount;
    }

}
