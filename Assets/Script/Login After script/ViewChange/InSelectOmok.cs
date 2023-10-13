using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//SceneManagement
using UnityEngine.SceneManagement;

public class InSelectOmok : MonoBehaviour
{
    // Start is called before the first frame update
    public void StartAIOmok()
    {
        SceneManager.LoadScene("Omok");
    }
    public void StartPlayerOmok()
    {
        SceneManager.LoadScene("Lobby");
    }
    public void toMain()
    {
        SceneManager.LoadScene("Main");
    }
    public void replay()
    {
        SceneManager.LoadScene("replay_omok_list");
    }
}
