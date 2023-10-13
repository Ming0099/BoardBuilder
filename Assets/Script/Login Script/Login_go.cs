using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Login_go : MonoBehaviour
{
    public void OnClickButton()
    {
        Destroy(GameObject.Find("Login_file"));
        SceneManager.LoadScene("SampleScene");
    }
}
