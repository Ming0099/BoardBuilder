using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//SceneManagement
using UnityEngine.SceneManagement;

public class SelecInMain : MonoBehaviour
{
    // Start is called before the first frame update
    public void SelectChess()
    {
        SceneManager.LoadScene("SelectChess");
    }
    public void SelectOmok()
    {
        SceneManager.LoadScene("SelectOmok");
    }
    public void GameQuit()
    {
        Application.Quit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            GameQuit();
    }
}
