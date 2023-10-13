using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class replay_list_chess : MonoBehaviour
{

    public Button item;

    GameObject login_information_;
    ArrayList date1;
    // Start is called before the first frame update
    void Start()
    {
        login_information_ = GameObject.Find("Login_");
        Init();
    }

    public void Init()
    {
        date1 = login_information_.GetComponent<Login_information>().return_chessdate();
        int yValue = 0;
        for (int i = 0; i < 10; i++)
        {
            Button index = Instantiate(item, new Vector2(0, yValue), Quaternion.identity);
            if (date1.Count > i)
            {
                index.GetComponentInChildren<Text>().text = date1[i].ToString();
            }
            else
            {
                index.GetComponentInChildren<Text>().text = "Null";
                index.enabled = false;
            }
            index.transform.SetParent(GameObject.Find("Content").transform);
            yValue -= 200;
        }
        GameObject.Find("Content/item").SetActive(false);
    }
    public void back()
    {
        SceneManager.LoadScene("SelectChess");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
