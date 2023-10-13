using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class user_information : MonoBehaviour
{
    GameObject login_information_;
    public Text name_text_;
    // Start is called before the first frame update
    void Start()
    {
        login_information_ = GameObject.Find("Login_");
        string nn = login_information_.GetComponent<Login_information>().return_name();
        name_text_.text = nn + " 님 환영합니다.";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
