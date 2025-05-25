using UnityEngine;
using UnityEngine.SceneManagement; 

public class ChangeSceneManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChangeScene();
        }
        if (SceneManager.GetActiveScene().name == "Ready")
        {
            Ready ready = FindObjectOfType<Ready>();
            if (ready != null && ready.changeSceneFlag == 1)
            {
                ChangeScene();
                ready.changeSceneFlag = 0; // シーン変更フラグをリセット
            }
        }
    }

    private void ChangeScene()
    {
        if (SceneManager.GetActiveScene().name == "Start")
        {
            WipeCircleController wipeController = FindObjectOfType<WipeCircleController>();
            if (wipeController != null)
            {
                wipeController.StartWipe();
                Invoke("LoadScene", 3f);
            }
            else
            {
                Debug.LogWarning("WipeCircleController not found in the scene.");
            }
        }else if (SceneManager.GetActiveScene().name == "Ready")
        {
            LoadScene();
        }else if (SceneManager.GetActiveScene().name == "Main")
        {
            LoadScene();
        }else if (SceneManager.GetActiveScene().name == "End")
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        if (SceneManager.GetActiveScene().name == "Start")
        {
            SceneManager.LoadScene("Ready");
        }else if (SceneManager.GetActiveScene().name == "Ready")
        {
            SceneManager.LoadScene("Main");
        }
        else if (SceneManager.GetActiveScene().name == "Main")
        {
            SceneManager.LoadScene("End");
        }
        else if (SceneManager.GetActiveScene().name == "End")
        {
            SceneManager.LoadScene("Start");
        }
    }
}
