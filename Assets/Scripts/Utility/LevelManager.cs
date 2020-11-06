using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    public Canvas MainCanvas;
    public Canvas ControlsCanvas;


	public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
   
    public void QuitApplication()
    {
        Application.Quit();
    }

    public void GoToControls()
    {
        MainCanvas.gameObject.SetActive(false);
        ControlsCanvas.gameObject.SetActive(true);
    }
    public void GoBack()
    {
        MainCanvas.gameObject.SetActive(true);
        ControlsCanvas.gameObject.SetActive(false);
    }
}
