using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour {

    public void LoadScene(int sceneId) {
        SceneManager.LoadScene(sceneId);
    }

    public void CloseGame() {
        Application.Quit();
    }
}
