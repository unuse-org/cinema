using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeMainScene : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeScene();
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
        }
    }

    private void LoadScene()
    {
        if (SceneManager.GetActiveScene().name == "Start")
        {
            SceneManager.LoadScene("Main");
        }
    }
}
