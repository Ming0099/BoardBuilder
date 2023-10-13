using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//SceneManagement
using UnityEngine.SceneManagement;

public class InSelectChess : MonoBehaviour
{
    // Start is called before the first frame update
    public void StartAIChess()
    {
        SceneManager.LoadScene("Chess_DaeYoung");
    }
    public void StartPlayerChess()
    {
        SceneManager.LoadScene("Lobby");
    }
    public void toMain()
    {
        SceneManager.LoadScene("Main");
    }
    public void replay()
    {
        SceneManager.LoadScene("replay_chess_list");
    }
}
