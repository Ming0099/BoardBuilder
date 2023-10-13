using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class chess_enter : MonoBehaviour
{
    public string d_date;
    // Start is called before the first frame update
    void Start()
    {
    }
    public void click_on()
    {
        GameObject clickObj = EventSystem.current.currentSelectedGameObject;
        d_date = clickObj.GetComponentInChildren<Text>().text;
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("in_chess_replay");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
